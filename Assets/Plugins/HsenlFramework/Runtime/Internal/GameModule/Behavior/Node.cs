using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Node<T> : INode<T> where T : IBehaviorTree {
        [MemoryPackIgnore]
        public abstract NodeType NodeType { get; }

        [MemoryPackIgnore]
        private NodeStatus _nowStatus; // 节点的很多事件是依赖于当前状态来决定是否执行的, 所以, 暴露在其他地方修改是非常危险的

        [MemoryPackIgnore]
        protected T manager;

        [MemoryPackIgnore]
        public NodeStatus NowStatus {
            get => this._nowStatus;
            internal set => this._nowStatus = value;
        }

        [MemoryPackIgnore]
        public IBlackboard Blackboard => this.manager.Blackboard;

        [MemoryPackIgnore]
        private INode _parent;

        [MemoryPackIgnore]
        INode INode.Parent {
            get => this._parent;
            set => this._parent = value;
        }

        [MemoryPackIgnore]
        public INode Parent => this._parent;

        // 行为树的初始行为, 对其下所有子节点进行激活
        public abstract void StartNode(IBehaviorTree tree);

        public abstract void OpenNode();

        public NodeStatus TickNode() {
            if (!this.InternalEvaluate()) {
                if (this.NowStatus == NodeStatus.Running) {
                    this.InternalExit();
                }

                this.NowStatus = NodeStatus.Continue;
                return NodeStatus.Continue;
            }

            if (this.NowStatus != NodeStatus.Running) {
                this.InternalEnter();
            }

            var oldStatus = this.NowStatus;
            this.NowStatus = this.InternalTick();

            if (this.NowStatus == NodeStatus.Running) {
                if (oldStatus != NodeStatus.Running) {
                    this.InternalRunStart();
                }

                this.InternalRunning();
            }
            else {
                if (oldStatus == NodeStatus.Running) {
                    this.InternalRunEnd();
                }
            }

            if (this.NowStatus != NodeStatus.Running) {
                this.InternalExit();
            }

            return this.NowStatus;
        }

        public abstract void CloseNode();

        public abstract void DestroyNode();

        public abstract void ResetNode();

        // 终止, 非自然终止, 由外部调用
        public abstract void AbortNode();

        internal void InternalStart() {
            try {
                this.OnAwake();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOpen() {
            try {
                this.OnEnable();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal bool InternalEvaluate() {
            try {
                return this.OnNodeEvaluate();
            }
            catch (Exception e) {
                Log.Error(e);
                return false;
            }
        }

        internal void InternalEnter() {
            this.manager.CurrentNode = this;
            try {
                this.OnNodeEnter();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal NodeStatus InternalTick() {
            try {
                return this.OnNodeTick();
            }
            catch (Exception e) {
                Log.Error(e);
                return NodeStatus.Continue; // 注意看: 这里返回的是无效, 也就是异常的节点, 会被视为是无效节点, 假装其不存在
            }
        }

        internal void InternalRunStart() {
            this.manager.CurrentNode = this;
            try {
                this.OnNodeRunStart();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalRunning() {
            this.manager.CurrentNode = this;
            try {
                this.OnNodeRunning();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalRunEnd() {
            this.manager.CurrentNode = this;
            try {
                this.OnNodeRunEnd();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalExit() {
            try {
                this.OnNodeExit();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            if (this.manager.CurrentNode == this) {
                this.manager.CurrentNode = null;
            }
        }

        internal void InternalClose() {
            try {
                this.OnDisable();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalReset() {
            try {
                this.OnReset();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalAbort() {
            if (this.NowStatus == NodeStatus.Running) {
                this.NowStatus = NodeStatus.Return;

                try {
                    this.InternalRunEnd();
                }
                catch (Exception e) {
                    Log.Error($"<node InternalRunEnd error> {e}");
                }

                try {
                    this.InternalExit();
                }
                catch (Exception e) {
                    Log.Error($"<node InternalExit error> {e}");
                }
            }

            this.OnAbort();
        }
        
        internal void InternalDestroy() {
            try {
                this.OnDestroy();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        /* 对比下面几个回调函数
         *
         * 由外部驱动的
         * OnNodeStart:
         * OnNodeOpen:
         * OnNodeClose:
         * OnNodeReset:
         * OnNodeAbort:
         * OnNodeDestroy: 
         *
         * 由Tick时驱动的
         * OnNodeEvaluate
         * OnNodeEnter
         * OnNodeTick
         * OnNodeRunStart
         * OnNodeRunning
         * OnNodeRunEnd
         * OnNodeExit
         */

        /// 可以看做是Awake, 当节点被添加到行为树的时候触发
        protected virtual void OnAwake() { }

        /// 当节点被外部激活, 类比 OnEnable, 可以把初始化, 例如获取组件的代码写在这. 该函数在关键的时候被调用, 比如父级改变, 重新激活等情况时
        protected virtual void OnEnable() { }

        /// 当节点被外部关闭, 类比 OnDisable
        protected virtual void OnDisable() { }

        /// 当节点被外部重置, 比Open调用的频繁, 适合重置数据, 比如技能cd, 比如每次进入技能的时候, 调用该函数.
        protected virtual void OnReset() { }

        /// 当节点被终止时
        protected virtual void OnAbort() { }
        
        /// 当节点从行为树移除时触发
        protected virtual void OnDestroy() { }


        // 当节点评估, 返回的结果将决定还是否需要进入该节点
        // 如果评估为假, 则该节点会被跳过, 状态会被认为是 continue
        // 与直接在 Tick 里返回 continue 相比, 这种方式会让该节点不被进入, 也就不需要再返回给管理器一个状态, 同时也不会再触发节点离开
        protected abstract bool OnNodeEvaluate();

        /// 每次节点被Tick前触发(但如果节点是Runing状态, 那么在下次Runing前, 只会触发一次)
        protected virtual void OnNodeEnter() { }

        /// 当节点执行
        protected abstract NodeStatus OnNodeTick();

        /// 当节点刚进入Running状态时触发, 如果节点没有进入Running状态, 那么即使被Tick也不会触发
        protected virtual void OnNodeRunStart() { }

        /// 当节点为Running状态时持续触发
        protected virtual void OnNodeRunning() { }

        /// 当节点刚离开Running状态时触发
        protected virtual void OnNodeRunEnd() { }

        /// 与OnNodeEnter对应
        protected virtual void OnNodeExit() { }
        
        public abstract Iterator<INode> ForeachChildren();

        public TNode GetNodeInParent<TNode>(bool once = false) {
            var parent = this.Parent;
            while (parent != null) {
                if (parent is TNode t) {
                    return t;
                }

                if (once) break;
                parent = parent.Parent;
            }

            return default;
        }

        public abstract TNode GetNodeInChildren<TNode>(bool once = false);

        public abstract TNode[] GetNodesInChildren<TNode>(bool once = false);

        public abstract void GetNodesInChildren<TNode>(List<TNode> cache, bool once = false);

        public override string ToString() {
            return this.GetType().Name;
        }
    }
}