using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    /* 流水线系统的核心就是在原本的Event系统的基础上实现热插拔，并以此衍生的一些附带功能
     * 所以，如果不使用热插拔功能，那你完全可以把理解成一个Event来使用
     *
     * 何时使用Event，又何时使用Pl？
     * 牵扯到游戏逻辑的尽量都使用Pl，牵扯到代码逻辑的去使用Event
     * 例如伤害流程、死亡流程、增加经验、拾取金币、打造装备等，使用Pl
     * 创建role，场景切换、加载外观等，使用Event
     * 模棱两可的情况：比如掉血UI数字，按道理来说，这种事情应该交给Event，但因为掉血时，需要用到许多掉血的数据，所以应该采用Pl+Event的方式，但如果嫌麻烦，则都交给Pl处理也行
     * 且注意，Event只在跨层（model、view）的时候，才有必要使用，其他时候，直接调用方法效率会更高一些
     * 但Pl不需要考虑这些，因为Pl的意义不在于纯粹发布事件。同时又因为Pl是用来处理游戏逻辑的，而游戏内的逻辑一般不需要每帧去运行，多是触发了就处理一下
     *
     * 流水线系统结构简介：
     * 组成：处理对象（item）、工人（worker）、工作台（handler）
     * item是区别不同流水线的唯一key。handler则是用来逻辑处理，如果仅仅到这里，也就和Event系统没什么区别，所以又引入了Worker来对流水线的作业增加了热插拔的功能
     * 例如：计算伤害流水线
     * 假如，现在需要对伤害计算进行一整条逻辑处理，首先要把伤害的各项数据先确定 -> 交给仲裁庭进行暴击率命中率攻防比等仲裁 -> 拿着仲裁结果对敌方进行扣血 -> 发布一条造成伤害的消息
     * 在这个过程中，我们需要一份伤害清单（item），而每个步骤都是一个Handler
     * 我们可以发现，这个过程中，我们并没有用到 worker，因为这几个步骤都是固定存在的，只要做伤害判定，就一定会经历这几个步骤
     * 但如果现在，有这么一个情况，某个装备上有一个词条A，词条效果是，每次攻击后，增加5%的攻速，那么此时，就需要用到我们的worker了
     * 我们创建一个Handler<Item, A>，然后我们把这个worker的实例，添加到我们的流水线组件，那么下次再进行伤害处理时，该词条就会被执行了
     * 同样的，如果有两个装备都有该词条，那就添加两次，同样的，计算伤害时，也会被执行两次，也就是攻击后，会一共增加10%的攻速
     */
    [Serializable]
    [MemoryPackable()]
    public partial class ProcedureLine : Unbodied {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, PropertySpace]
#endif
        private readonly MultiList<Type, IProcedureLineWorker> _workerDict = new();

        private readonly List<ProcedureLineNode> _attaches = new();

        public void AddWorker(IProcedureLineWorker worker) {
            this.AddWorker_Internal(worker);
            worker.OnAddToProcedureLine(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddWorker_Internal(IProcedureLineWorker worker) {
            this._workerDict.Add(worker.GetType(), worker);
        }

        public void RemoveWorker(IProcedureLineWorker worker) {
            this.RemoveWorker_Internal(worker);
            worker.OnRemoveFromProcedureLine(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveWorker_Internal(IProcedureLineWorker worker) {
            this._workerDict.Remove(worker.GetType(), worker);
        }

        public bool Attach(ProcedureLineNode node) {
            if (this._attaches.Contains(node)) return false;
            this._attaches.Add(node);
            var workers = node.Workers;
            for (int i = 0, len = workers.Count; i < len; i++) {
                var worker = workers[i];
                this.AddWorker_Internal(worker);
            }

            node.LinkProcedureLine(this);
            return true;
        }

        public bool Detach(ProcedureLineNode node) {
            var succ = this._attaches.Remove(node);
            if (!succ)
                return false;
            var workers = node.Workers;
            for (int i = 0, len = workers.Count; i < len; i++) {
                var worker = workers[i];
                this.RemoveWorker_Internal(worker);
            }

            node.UnlinkProcedureLine(this);
            return true;
        }


        internal override void OnDisposedInternal() {
            base.OnDisposedInternal();
            foreach (var node in this._attaches) {
                node.UnlinkProcedureLine(this);
            }

            this._workerDict.Clear();
            this._attaches.Clear();
        }

        /// <summary>
        /// 开启一条流水线
        /// 遇到T内的字段很多时，可以使用ref版Start
        /// </summary>
        /// <param name="item">这条流水线的作业物，也是该流水线的标志</param>
        /// <param name="throwIfNull"></param>
        /// <param name="userToken"></param>
        /// <typeparam name="T">作业物的类型</typeparam>
        /// <returns></returns>
        public ProcedureLineHandleResult StartLine<T>(ref T item, bool throwIfNull = true, object userToken = null) {
            if (!_handlerDict.TryGetValue(typeof(T), out var concurrentQueue)) {
                if (throwIfNull)
                    throw new InvalidOperationException($"procedure line handler 'Hsen.AProcedureLineHandler`1{typeof(T)}' is not unrealized");

                return ProcedureLineHandleResult.Fail;
            }

            foreach (var handlerPair in concurrentQueue) {
                ProcedureLineHandleResult result;
                switch (handlerPair.handler) {
                    case IProcedureLineHandlerOfWorker<T> handler:
                        if (this._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = handler.Run(this, ref item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        break;

                    // no worker就和普通的event系统没多大区别
                    case IProcedureLineHandlerNoWorker<T> handler:
                        result = handler.Run(this, ref item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;

                    default:
                        throw new ArgumentException($"{handlerPair.handler} is async, but start line is sync");
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        /// <summary>
        /// 合并两个pl的workers, 并开启一条流水线
        /// </summary>
        /// <param name="other"></param>
        /// <param name="item"></param>
        /// <param name="throwIfNull"></param>
        /// <param name="userToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ProcedureLineHandleResult MergeStartLine<T>(ProcedureLine other, ref T item, bool throwIfNull = true, object userToken = null) {
            if (!_handlerDict.TryGetValue(typeof(T), out var concurrentQueue)) {
                if (throwIfNull)
                    throw new InvalidOperationException($"procedure line handler 'Hsen.AProcedureLineHandler`1{typeof(T)}' is not unrealized");

                return ProcedureLineHandleResult.Fail;
            }

            foreach (var handlerPair in concurrentQueue) {
                ProcedureLineHandleResult result;
                switch (handlerPair.handler) {
                    case IProcedureLineHandlerOfWorker<T> handler:
                        if (this._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var worker in workers) {
                                result = handler.Run(this, ref item, worker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        if (other._workerDict.TryGetValue(handlerPair.workerType, out workers)) {
                            foreach (var worker in workers) {
                                result = handler.Run(this, ref item, worker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        break;

                    // no worker就和普通的event系统没多大区别
                    case IProcedureLineHandlerNoWorker<T> handler:
                        result = handler.Run(this, ref item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;

                    default:
                        throw new ArgumentException($"{handlerPair.handler} is async, but start line is sync");
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        public static ProcedureLineHandleResult MergeStartLine<T>(IList<ProcedureLine> procedureLines, ref T item, bool throwIfNull = true, object userToken = null) {
            if (procedureLines == null || procedureLines.Count == 0)
                return ProcedureLineHandleResult.Fail;

            var mainline = procedureLines[0];

            if (!_handlerDict.TryGetValue(typeof(T), out var concurrentQueue)) {
                if (throwIfNull)
                    throw new InvalidOperationException($"procedure line handler 'Hsen.AProcedureLineHandler`1{typeof(T)}' is not unrealized");

                return ProcedureLineHandleResult.Fail;
            }

            foreach (var handlerPair in concurrentQueue) {
                ProcedureLineHandleResult result;
                switch (handlerPair.handler) {
                    case IProcedureLineHandlerOfWorker<T> handler:
                        if (mainline._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var worker in workers) {
                                result = handler.Run(mainline, ref item, worker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        for (int i = 1, len = procedureLines.Count; i < len; i++) {
                            var other = procedureLines[i];
                            if (other._workerDict.TryGetValue(handlerPair.workerType, out workers)) {
                                foreach (var worker in workers) {
                                    result = handler.Run(mainline, ref item, worker, userToken);
                                    if (result == ProcedureLineHandleResult.Break) {
                                        return result;
                                    }
                                }
                            }
                        }

                        break;

                    // no worker就和普通的event系统没多大区别
                    case IProcedureLineHandlerNoWorker<T> handler:
                        result = handler.Run(mainline, ref item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;

                    default:
                        throw new ArgumentException($"{handlerPair.handler} is async, but start line is sync");
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        public ProcedureLineHandleResult StartLine<T>(T item, bool throwIfNull = true, object userToken = null) {
            return this.StartLine(ref item, throwIfNull, userToken);
        }

        public ProcedureLineHandleResult MergeStartLine<T>(ProcedureLine other, T item, bool throwIfNull = true, object userToken = null) {
            return this.MergeStartLine(other, ref item, throwIfNull, userToken);
        }

        public static ProcedureLineHandleResult MergeStartLine<T>(IList<ProcedureLine> procedureLines, T item, bool throwIfNull = true, object userToken = null) {
            return MergeStartLine(procedureLines, ref item, throwIfNull, userToken);
        }

        public async HTask<ProcedureLineHandleResult> StartLineAsync<T>(T item, bool throwIfNull = true, object userToken = null) {
            if (!_handlerDict.TryGetValue(typeof(T), out var concurrentQueue)) {
                if (throwIfNull)
                    throw new InvalidOperationException($"procedure line handler 'Hsen.AProcedureLineHandler`1{typeof(T)}' is not unrealized");

                return ProcedureLineHandleResult.Fail;
            }

            foreach (var handlerPair in concurrentQueue) {
                ProcedureLineHandleResult result;
                switch (handlerPair.handler) {
                    case IProcedureLineHandlerOfWorker<T> handler: {
                        if (this._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = handler.Run(this, ref item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        break;
                    }

                    case IProcedureLineHandlerNoWorker<T> handler: {
                        result = handler.Run(this, ref item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;
                    }

                    case IProcedureLineHandlerOfWorkerAsync<T> handler: {
                        if (this._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = await handler.Run(this, item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        break;
                    }

                    case IProcedureLineHandlerNoWorkerAsync<T> handler: {
                        result = await handler.Run(this, item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;
                    }
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        public async HTask<ProcedureLineHandleResult> MergeStartLineAsync<T>(ProcedureLine other, T item, bool throwIfNull = true, object userToken = null) {
            if (!_handlerDict.TryGetValue(typeof(T), out var concurrentQueue)) {
                if (throwIfNull)
                    throw new InvalidOperationException($"procedure line handler 'Hsen.AProcedureLineHandler`1{typeof(T)}' is not unrealized");

                return ProcedureLineHandleResult.Fail;
            }

            foreach (var handlerPair in concurrentQueue) {
                ProcedureLineHandleResult result;
                switch (handlerPair.handler) {
                    case IProcedureLineHandlerOfWorker<T> handler: {
                        if (this._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = handler.Run(this, ref item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        if (other._workerDict.TryGetValue(handlerPair.workerType, out workers)) {
                            foreach (var productLineWorker in workers) {
                                result = handler.Run(this, ref item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        break;
                    }

                    case IProcedureLineHandlerNoWorker<T> handler: {
                        result = handler.Run(this, ref item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;
                    }

                    case IProcedureLineHandlerOfWorkerAsync<T> handler: {
                        if (this._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = await handler.Run(this, item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        if (other._workerDict.TryGetValue(handlerPair.workerType, out workers)) {
                            foreach (var productLineWorker in workers) {
                                result = await handler.Run(this, item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        break;
                    }

                    case IProcedureLineHandlerNoWorkerAsync<T> handler: {
                        result = await handler.Run(this, item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;
                    }
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        public static async HTask<ProcedureLineHandleResult> MergeStartLineAsync<T>(IList<ProcedureLine> procedureLines, T item, bool throwIfNull = true, object userToken = null) {
            if (procedureLines == null || procedureLines.Count == 0)
                return ProcedureLineHandleResult.Fail;

            var mainline = procedureLines[0];

            if (!_handlerDict.TryGetValue(typeof(T), out var concurrentQueue)) {
                if (throwIfNull)
                    throw new InvalidOperationException($"procedure line handler 'Hsen.AProcedureLineHandler`1{typeof(T)}' is not unrealized");

                return ProcedureLineHandleResult.Fail;
            }

            foreach (var handlerPair in concurrentQueue) {
                ProcedureLineHandleResult result;
                switch (handlerPair.handler) {
                    case IProcedureLineHandlerOfWorker<T> handler: {
                        if (mainline._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = handler.Run(mainline, ref item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        for (int i = 1, len = procedureLines.Count; i < len; i++) {
                            var other = procedureLines[i];
                            if (other._workerDict.TryGetValue(handlerPair.workerType, out workers)) {
                                foreach (var productLineWorker in workers) {
                                    result = handler.Run(mainline, ref item, productLineWorker, userToken);
                                    if (result == ProcedureLineHandleResult.Break) {
                                        return result;
                                    }
                                }
                            }
                        }

                        break;
                    }

                    case IProcedureLineHandlerNoWorker<T> handler: {
                        result = handler.Run(mainline, ref item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;
                    }

                    case IProcedureLineHandlerOfWorkerAsync<T> handler: {
                        if (mainline._workerDict.TryGetValue(handlerPair.workerType, out var workers)) {
                            foreach (var productLineWorker in workers) {
                                result = await handler.Run(mainline, item, productLineWorker, userToken);
                                if (result == ProcedureLineHandleResult.Break) {
                                    return result;
                                }
                            }
                        }

                        for (int i = 1, len = procedureLines.Count; i < len; i++) {
                            var other = procedureLines[i];
                            if (other._workerDict.TryGetValue(handlerPair.workerType, out workers)) {
                                foreach (var productLineWorker in workers) {
                                    result = await handler.Run(mainline, item, productLineWorker, userToken);
                                    if (result == ProcedureLineHandleResult.Break) {
                                        return result;
                                    }
                                }
                            }
                        }

                        break;
                    }

                    case IProcedureLineHandlerNoWorkerAsync<T> handler: {
                        result = await handler.Run(mainline, item, userToken);
                        if (result == ProcedureLineHandleResult.Break) {
                            return result;
                        }

                        break;
                    }
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        private class HandlerPair {
            public readonly Type workerType;
            public readonly IProcedureLineHandler handler;

            public HandlerPair(Type workerType, IProcedureLineHandler handler) {
                this.workerType = workerType;
                this.handler = handler;
            }
        }
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("所有处理者 (静态变量)"), PropertyOrder(-1)]
#endif
        private static readonly ConcurrentMultiQueue<Type, HandlerPair> _handlerDict = new();

        [OnEventSystemInitialized, OnEventSystemReload]
        private static void CacheHandles() {
            _handlerDict.Clear();
            Dictionary<Type, SortedDictionary<int, IProcedureLineHandler>> sortedDict = new();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(ProcedureLineHandlerAttribute))) {
                var obj = Activator.CreateInstance(type);
                if (obj is not IProcedureLineHandler handler) {
                    throw new InvalidOperationException($"type '{type}' is not procedure line handler");
                }

                object attribute = type.GetCustomAttribute(typeof(ProcedureLineHandlerPriorityAttribute), false);
                if (attribute is not ProcedureLineHandlerPriorityAttribute priorityAttribute) {
                    // handler 必须实现优先级属性
                    throw new InvalidCastException($"procedure line handler '{type}' must be implementations [ProcedureLineHandlePriorityAttribute]");
                }

                if (sortedDict.TryGetValue(handler.ItemType, out var dict)) {
                    if (dict.TryGetValue(priorityAttribute.priority, out var repeater)) {
                        var t1 = repeater.GetType();
                        var t2 = handler.GetType();
                        // 在同一个流水线中，不能出现优先级重复的处理者
                        throw new InvalidCastException(
                            $"procedure line '{handler.ItemType}' is already has key(priority) '{priorityAttribute.priority}', pair: '{t1}' '{t2}'");
                    }

                    dict.Add(priorityAttribute.priority, handler);
                }
                else {
                    sortedDict[handler.ItemType] = new SortedDictionary<int, IProcedureLineHandler>() { { priorityAttribute.priority, handler } };
                }
            }

            foreach (var kv in sortedDict) {
                foreach (var handlerKv in kv.Value) { // kv.Value 是已经按照从小到大排好顺序的字典
                    _handlerDict.Enqueue(kv.Key, new HandlerPair(handlerKv.Value.WorkerType, handlerKv.Value));
                }
            }
        }
    }
}