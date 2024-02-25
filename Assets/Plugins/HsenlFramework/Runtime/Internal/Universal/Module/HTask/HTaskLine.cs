using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
#endif

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
     * 结合HTask一起使用, 可以让多线程逻辑不再并行执行, 而是按照我们的需要的执行顺序执行, 从而避免多线程带来的数据修改冲突问题, 同时代码也确实是在多线程上运行, 不占用主线程算力资源.
     */

    public interface ITaskLineWaiter : IDisposable {
        bool IsDisposed { get; }
    }

    public class HTaskLine {
        // 这里其实不用对字段进行线程安全处理, 因为这个taskline在使用时就是为了避免多线程竞争, 但为了避免误用, 所以还是加上安全处理.
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, PropertySpace]
#endif
        private readonly ConcurrentDictionary<int, WaitBox> _waits = new();

        private volatile int _lowestPosition = int.MaxValue;
        private volatile int _maxinumPosition = int.MinValue;

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="position"></param>
        /// <param name="cover">覆盖之前的tcs，也就代表之前的wait将不在起效果，但如果已经正在执行了，则没有效果</param>
        /// <param name="includeSelf">如果自己的编号处于挂起状态，那么再调用自己的编号也会被Wait</param>
        /// <param name="clearOutdated">清除过时的wait，例如 1 2 3 4 5，每次调用1，都会把 2 3 4 5中的tcs给清理掉</param>
        /// <returns></returns>
        public async HTask<ITaskLineWaiter> Wait(int position, bool cover = true, bool includeSelf = true, bool clearOutdated = false) {
            if (position > this._maxinumPosition)
                Interlocked.Exchange(ref this._maxinumPosition, position);

            if (!this._waits.TryGetValue(position, out var waitBox)) {
                waitBox = WaitBox.Create(); // 这里其实用不用池都行，因为box添加后，在整个wait line生命周期里，都不会删除了
                this._waits[position] = waitBox;
            }

            waitBox.waitSelf = includeSelf;

            if (clearOutdated) {
                // wait 1的同时，把23456都给取消掉
                this.Cancel(position + 1, this._maxinumPosition);
            }

            if (position < this._lowestPosition) {
                // 小于就直接走
                waitBox.status = WaitBoxStatus.Pending;
                Interlocked.Exchange(ref this._lowestPosition, position);
                goto LEAVE;
            }

            // include self为true，代表会自己等自己，比如重复的wait 1，那么第二次wait就会等第一次wait结束再执行
            var intercept = includeSelf ? position >= this._lowestPosition : position > this._lowestPosition;
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
                    waitBox.CancelAll();
                }

                // 如果之前的状态是结束了，那么就再次进入等待状态，之所以要判断，是因为有一种情况，连续wait 1两次，这种情况下，
                // 第二次wait 1的时候，其实1的状态是挂起状态，此时如果把1设置为waiting，会导致第一次的wait，无法顺利的notify
                // 导致后面的链断掉
                if (waitBox.status == WaitBoxStatus.Notify) {
                    waitBox.status = WaitBoxStatus.Waiting;
                }

                var tcs = HTask.Create();
                waitBox.Tasks.Enqueue(tcs);
                if (waitBox.Tasks.Count > 9) Log.Warning("logic wait too many times, check whether it is normal"); // 可根据实际使用过程中调整警告阈值

                await tcs;

                // 到这里说明等待完毕了
                // 例如 await LogicLine.Wait<T>(2); 这句代码里的await，等的就是上面的这个await tcs
                // 接下来进入挂起状态，也就是执行函数体的阶段
                // 同时标志着，当前的定位在2这里
                waitBox.status = WaitBoxStatus.Pending;
                Interlocked.Exchange(ref this._lowestPosition, position);
            }

            LEAVE:
            var waiter = TaskLineWaiter.Create(this, position);
            return waiter;
        }

        private void Notify(int position) {
            if (position > this._maxinumPosition) {
                throw new Exception($"<未知的Notify, 或TaskLine已经被清除了> Position:{position}'");
            }

            if (position > this._lowestPosition) {
                // 说明在自己挂起的这个过程中，又有比自己编号靠前的编号被wait了，这种情况，把自己状态设置下，然后自己走开就行了，把自己的任务
                // 交给他处理
                // 注意，这里自己的任务指的是触发下一个编号的tcs，而自己的tcs其实是空的，因为自己既然已经执行到notify这一步，
                // 说明自己的tcs一定已经被调用过了

                // 这里必须要设置状态，因为此时的状态一定是挂起状态，如果不设置的话，靠前编号执行完开始轮询的时候，会发现这里仍然在挂起，他就会
                // 不管不问，直接退出，从而导致链条在这里断掉
                var waitBox = this._waits[position];
                waitBox.status = WaitBoxStatus.Notify;
                return;
            }

            CirculationSetResult(this, position, this._maxinumPosition);
        }

        private void Cancel(int startPosition, int endPosition) {
            var position = startPosition;
            while (position <= endPosition) {
                if (this._waits.TryGetValue(position, out var logicBox)) {
                    logicBox.CancelAll();
                }

                position++;
            }
        }

        private static void CirculationSetResult(HTaskLine taskLine, int position, int max) {
            var own = true; // 第一次要获得是自己的tcs
            while (position <= max) {
                if (taskLine._waits.TryGetValue(position, out var waitBox)) {
                    try {
                        // 自己的状态，只有自己能设置为 Notify，也必然会设置
                        if (own) {
                            waitBox.status = WaitBoxStatus.Notify;
                        }

                        // tcss不为空，这次的循环就到这里为止
                        if (waitBox.Tasks.Count != 0) {
                            // 如果是own，则状态一定会是notify，说明当前tcs肯定不是own，不是自己，且还在挂起状态，说明这是这种情况如下
                            // 例如 abc 逻辑线
                            // 先调用了b，再调用了a，导致b和a一起同时开始执行了，然后等a执行完了，b还再执行中，此时这种情况，
                            // 什么都不用做，就直接退出就好了，等b执行完后，会接着往下处理
                            if (waitBox.status == WaitBoxStatus.Pending) {
                                // logicLine.lowestPositions[type] = ++position;
                                return;
                            }

                            // 如果是wait self，那就一个个执行，例如
                            // a b b b这样调用，a执行完毕后，轮询到第二个b这，然后触发第二个b的wait，然后再等到第二个b执行完，
                            // 再触发第三个b的wait，这么依次触发
                            if (waitBox.waitSelf) {
                                waitBox.SetResult();
                                return;
                            }

                            // 而如果不是wait self的话，那么就直接把上面 三个b的wait全部触发了，因为理论上，这三个b也确实等待a完成了
                            waitBox.SetResultAll();

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
            Interlocked.Exchange(ref taskLine._lowestPosition, position);
        }

        public void Dispose() {
            this._lowestPosition = int.MaxValue;
            this._maxinumPosition = int.MinValue;
            foreach (var kv in this._waits) {
                kv.Value.Dispose();
                ObjectPoolManager.Instance.Return(kv.Value);
            }

            this._waits.Clear();
        }

        private enum WaitBoxStatus {
            Waiting, // 等待执行
            Pending, // 正在执行中，处于挂起状态
            Notify, // 执行完了
        }

        [Serializable]
        private class WaitBox : IDisposable {
            public WaitBoxStatus status;
            public bool waitSelf;
            private Queue<HTask> _tasks;

            public Queue<HTask> Tasks => this._tasks ??= new Queue<HTask>();

            public static WaitBox Create() {
                var box = ObjectPool.Rent<WaitBox>();
                return box;
            }

            public void SetResult() {
                this.Tasks.Dequeue().SetResult();
            }

            public void SetResultAll() {
                while (this.Tasks.Count > 0) {
                    this.Tasks.Dequeue().SetResult();
                }
            }

            public void CancelAll() {
                while (this.Tasks.Count > 0) {
                    this.Tasks.Dequeue().Abort();
                }
            }

            public void Dispose() {
                this.status = 0;
                this.waitSelf = false;
                this._tasks?.Clear();
            }
        }

        private class TaskLineWaiter : ITaskLineWaiter {
            public HTaskLine taskLine;
            public int position;
            public bool IsDisposed { get; private set; }

            public static TaskLineWaiter Create(HTaskLine taskLine, int position) {
                var lineWaiter = ObjectPoolManager.Instance.Rent<TaskLineWaiter>();
                lineWaiter.taskLine = taskLine;
                lineWaiter.position = position;
                lineWaiter.IsDisposed = false;
                return lineWaiter;
            }

            public void Dispose() {
                this.IsDisposed = true;
                this.taskLine.Notify(this.position);
                this.taskLine = null;
                this.position = 0;
                ObjectPoolManager.Instance.Return(this);
            }
        }
    }
}