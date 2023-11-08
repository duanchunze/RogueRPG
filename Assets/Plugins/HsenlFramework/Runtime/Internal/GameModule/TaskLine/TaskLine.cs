using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Hsenl {
    /* 逻辑线组件（核心思路，针对异步逻辑的时间不确定性，只要给这些逻辑打上逻辑线，那么代码在很大程度上都会按照正确的执行顺序去执行）
     * 它的目的是让一整条异步代码的逻辑，转换成更像同步代码，让我们在调用它们的时候，不用太过纠结于他们的执行顺序
     *
     * 现在有 a b c三个执行指令，顺序就是abc顺序，对应的执行编号为 1 2 3
     * 同步调用情况
     * a b c   -- 最标准的方式，a直接开始执行，b等待a执行完，c等b执行完
     * b c a   -- a会直接执行，而b因为调用在a前，导致他也已经开始执行了，待b执行完调用Notify时，如果a还没执行完，他会把自己的任务交给a，待a执行完后，会帮b去触发c
     * b b c a -- 跟上面一样，a执行完后，会帮b触发第二个b的wait，然后第二个b完成，再去触发c的wait
     * a c b c -- 因为 cb被调用时，a还没执行完毕，所以后面无论怎么调用，都无所谓，最终还是会按照正确的顺序执行
     * c a b   -- a和c会同时执行，但b会等到a执行完
     *
     * 和ObjectWait的适用点不同
     * ObjectWait相当于是定一个等待的事件，待该事件完成后，再处理什么事情，我们一开始就能确定必须要等待
     * 你也可以把他当成一个ObjectWait去使用
     */

    public interface ITaskLineType {
        int Min { get; }
        int Max { get; }
    }

    public interface ITaskLineWaiter : IDisposable {
        bool IsDisposed { get; }
    }

    [Serializable]
    public class TaskLine : Unbodied {
        [ShowInInspector, ReadOnly, LabelText("所有任务线类型 (静态变量)")]
        private static readonly Dictionary<Type, ITaskLineType> _lineTypesDict = new();

        [ShowInInspector, ReadOnly, PropertySpace]
        private readonly MultiDictionary<Type, int, WaitBox> _tcss = new();

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<Type, int> _lowestPositions = new();

        protected override void OnDestroy() {
            foreach (var kv in this._tcss) {
                foreach (var boxKv in kv.Value) {
                    boxKv.Value.Dispose();
                    ObjectPoolManager.Instance.Return(boxKv.Value);
                }
            }
        }

        // 缓存每个ILogicLineType的实体对象，用于获得其最大最小值
        private static ITaskLineType GetLogicLineType(Type type) {
            if (!_lineTypesDict.TryGetValue(type, out var result)) {
                result = Activator.CreateInstance(type) as ITaskLineType;
                _lineTypesDict[type] = result;
            }

            return result;
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="position"></param>
        /// <param name="cover">覆盖之前的tcs，也就代表之前的wait将不在起效果，但如果已经正在执行了，则没有效果</param>
        /// <param name="includeSelf">如果自己的编号处于挂起状态，那么再调用自己的编号也会被Wait</param>
        /// <param name="clearOutdated">清除过时的wait，例如 1 2 3 4 5，每次调用1，都会把 2 3 4 5中的tcs给清理掉</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async ETTask<ITaskLineWaiter> Wait<T>(int position, bool cover = true, bool includeSelf = true, bool clearOutdated = false)
            where T : ITaskLineType {
            var type = typeof(T);

            if (!this._tcss.TryGetValue(type, position, out var logicBox)) {
                logicBox = WaitBox.Create(); // 这里其实用不用池都行，因为box添加后，在整个wait line生命周期里，都不会删除了
                this._tcss[type, position] = logicBox;
            }

            logicBox.waitSelf = includeSelf;

            if (clearOutdated) {
                // wait 1的同时，把23456都给取消掉
                this.Cancel(type, position + 1, GetLogicLineType(type).Max);
            }

            if (!this._lowestPositions.TryGetValue(type, out var lowestPosition)) {
                // 没有说明是该类型的第一个wait，直接走
                lowestPosition = position;
                this._lowestPositions[type] = lowestPosition;
                logicBox.waitBoxStatus = WaitBoxStatus.Pending;
                goto LEAVE;
            }

            if (position < lowestPosition) {
                // 小于就直接走
                logicBox.waitBoxStatus = WaitBoxStatus.Pending;
                this._lowestPositions[type] = position;
                goto LEAVE;
            }

            // include self为true，代表会自己等自己，比如重复的wait 1，那么第二次wait就会等第一次wait结束再执行
            var intercept = includeSelf ? position >= lowestPosition : position > lowestPosition;
            if (intercept) {
                // 如果选择覆盖，则第二次wait，会把第一次wait给取消掉，也就是上一个wait后面的所有回调内容都不会再被执行，彻底哑火
                // 但如果覆盖之前，这些回调内容就已经被执行了，那这段就相当于是无效代码
                // 例如下面这个完整的示例
                // await LogicLine  -- 1
                // do...            -- 2
                // do...            -- 3
                // await 其他未知内容 -- 4
                // do...            -- 5
                // notify LogicLine -- 6
                // 假设我们现在连续调用了2次该示例
                // 如果选择不覆盖，当第一步的await等待完成后，第二步到后面的内容会被执行2次
                // 如果选择覆盖，那么只会执行一次，也就是第二次我们调用的那次
                // 然而，如果我们第二次调用该示例时，第一次的await已经等待完毕，并且执行到了第四步的那个await了，
                // 那么第二次所谓的覆盖自然也就无效了，因为上次的tcss已经被执行并被清理过了，但是第三次的覆盖会生效
                // 
                // cover对于一些指令来说，很实用，比如播放动画，我现在先后调用了播放idle、attack两个动画，等执行的时候，idle便不再有必要去
                // 播放一次了，直接覆盖为播放attack就可以了
                if (cover) {
                    logicBox.CancelAll();
                }

                // 如果之前的状态是结束了，那么就再次进入等待状态，之所以要判断，是因为有一种情况，连续wait 1两次，这种情况下，
                // 第二次wait 1的时候，其实1的状态是挂起状态，此时如果把1设置为waiting，会导致第一次的wait，无法顺利的notify
                // 导致后面的链断掉
                if (logicBox.waitBoxStatus == WaitBoxStatus.Notify) {
                    logicBox.waitBoxStatus = WaitBoxStatus.Waiting;
                }

                var tcs = ETTask.Create(true);
                logicBox.Tcss.Enqueue(tcs);
                if (logicBox.Tcss.Count > 9) Log.Warning("logic wait too many times, check whether it is normal"); // 可根据实际使用过程中调整警告阈值

                await tcs;

                // 到这里说明等待完毕了
                // 例如 await LogicLine.Wait<T>(2); 这句代码里的await，等的就是上面的这个await tcs
                // 接下来进入挂起状态，也就是执行函数体的阶段
                // 同时标志着，当前的定位在2这里
                logicBox.waitBoxStatus = WaitBoxStatus.Pending;
                this._lowestPositions[type] = position;
            }

            LEAVE:
            var waiter = TaskLineWaiter.Create(this, type, position);
            return waiter;
        }

        public void Notify<T>(int position) where T : ITaskLineType {
            var type = typeof(T);
            this.Notify(type, position);
        }

        private void Notify(Type type, int position) {
            if (!this._lowestPositions.TryGetValue(type, out var lowestPosition)) {
                throw new Exception($"<该类型从没被wait> Type:'{type}' Position:{position}'");
            }

            if (position > lowestPosition) {
                // 说明在自己挂起的这个过程中，又有比自己编号靠前的编号被wait了，这种情况，把自己状态设置下，然后自己走开就行了，把自己的任务
                // 交给他处理
                // 注意，这里自己的任务指的是触发下一个编号的tcs，而自己的tcs其实是空的，因为自己既然已经执行到notify这一步，
                // 说明自己的tcs一定已经被调用过了

                // 这里必须要设置状态，因为此时的状态一定是挂起状态，如果不设置的话，靠前编号执行完开始轮询的时候，会发现这里仍然在挂起，他就会
                // 不管不问，直接退出，从而导致链条在这里断掉
                var waitBox = this._tcss[type, position];
                waitBox.waitBoxStatus = WaitBoxStatus.Notify;
                return;
            }

            var waitLineType = GetLogicLineType(type);
            CirculationSetResult(this, type, position, waitLineType.Max);
        }

        private void Cancel(Type type, int startPosition, int endPosition) {
            if (!this._tcss.TryGetDict(type, out var dict)) return;
            var position = startPosition;
            while (position <= endPosition) {
                if (dict.TryGetValue(position, out var logicBox)) {
                    logicBox.CancelAll();
                }

                position++;
            }
        }

        private static void CirculationSetResult(TaskLine taskLine, Type type, int position, int max) {
            if (!taskLine._tcss.TryGetDict(type, out var dict)) return;
            var own = true; // 第一次要获得是自己的tcs
            while (position <= max) {
                if (dict.TryGetValue(position, out var logicBox)) {
                    try {
                        // 自己的状态，只有自己能设置为 Notify，也必然会设置
                        if (own) {
                            logicBox.waitBoxStatus = WaitBoxStatus.Notify;
                        }

                        // tcss不为空，这次的循环就到这里为止
                        if (logicBox.Tcss.Count != 0) {
                            // 如果是own，则状态一定会是notify，说明当前tcs肯定不是own，不是自己，且还在挂起状态，说明这是这种情况如下
                            // 例如 abc 逻辑线
                            // 先调用了b，再调用了a，导致b和a一起同时开始执行了，然后等a执行完了，b还再执行中，此时这种情况，
                            // 什么都不用做，就直接退出就好了，等b执行完后，会接着往下处理
                            if (logicBox.waitBoxStatus == WaitBoxStatus.Pending) {
                                // logicLine.lowestPositions[type] = ++position;
                                return;
                            }

                            // 如果是wait self，那就一个个执行，例如
                            // a b b b这样调用，a执行完毕后，轮询到第二个b这，然后触发第二个b的wait，然后再等到第二个b执行完，
                            // 再触发第三个b的wait，这么依次触发
                            if (logicBox.waitSelf) {
                                logicBox.SetResult();
                                return;
                            }

                            // 而如果不是wait self的话，那么就直接把上面 三个b的wait全部触发了，因为理论上，这三个b也确实等待a完成了
                            logicBox.SetResultAll();

                            return;
                        }
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }
                }

                // 当前的position没有可以执行的tcs，所以++到下个编号，继续尝试
                own = false;
                position++;
            }

            // 确保position>max时，也不会漏掉这次的最低位置设置
            taskLine._lowestPositions[type] = position;
        }
        
        private enum WaitBoxStatus {
            Waiting, // 等待执行
            Pending, // 正在执行中，处于挂起状态
            Notify, // 执行完了
        }

        [Serializable]
        private class WaitBox : IDisposable {
            public WaitBoxStatus waitBoxStatus;
            public bool waitSelf;
            private Queue<ETTask> _tcss;

            public Queue<ETTask> Tcss => this._tcss ??= new Queue<ETTask>();

            public static WaitBox Create() {
                var box = ObjectPool.Rent<WaitBox>();
                return box;
            }

            public void SetResult() {
                this._tcss.Dequeue().SetResult();
            }

            public void SetResultAll() {
                while (this._tcss.Count > 0) {
                    this._tcss.Dequeue().SetResult();
                }
            }

            public void CancelAll() {
                while (this._tcss.Count > 0) {
                    this._tcss.Dequeue().Cancel();
                }
            }

            public void Dispose() {
                this.waitBoxStatus = 0;
                this.waitSelf = false;
                this._tcss?.Clear();
            }
        }

        private class TaskLineWaiter : ITaskLineWaiter {
            public TaskLine taskLine;
            public Type type;
            public int position;
            public bool IsDisposed { get; private set; }

            public static TaskLineWaiter Create(TaskLine taskLine, Type type, int position) {
                var lineWaiter = ObjectPoolManager.Instance.Rent<TaskLineWaiter>();
                lineWaiter.taskLine = taskLine;
                lineWaiter.type = type;
                lineWaiter.position = position;
                lineWaiter.IsDisposed = false;
                return lineWaiter;
            }

            public void Dispose() {
                this.IsDisposed = true;
                this.taskLine.Notify(this.type, this.position);
                this.taskLine = null;
                this.type = null;
                this.position = 0;
                ObjectPoolManager.Instance.Return(this);
            }
        }
    }
}