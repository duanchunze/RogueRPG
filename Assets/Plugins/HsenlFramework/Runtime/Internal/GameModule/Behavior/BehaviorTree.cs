using System;
using MemoryPack;
using Sirenix.OdinInspector;
using Unity.Mathematics;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class BehaviorTree<T> : Unbodied, IBehaviorTree where T : INode {
        [ShowInInspector]
        [MemoryPackIgnore]
        protected IBlackboard blackboard;

        [ShowInInspector]
        [MemoryPackInclude]
        protected T entryNode;

        [MemoryPackIgnore]
        protected INode currentNode;

        [MemoryPackIgnore]
        public virtual IBlackboard Blackboard => this.blackboard ??= new Blackboard();

        [MemoryPackIgnore]
        public T EntryNode => this.entryNode;

        [MemoryPackIgnore]
        INode IBehaviorTree.CurrentNode {
            get => this.currentNode;
            set => this.currentNode = value;
        }

        [MemoryPackIgnore]
        public float DeltaTime { get; protected set; }

        void IBehaviorTree.SetEntryNode(INode node) {
            this.SetEntryNode((T)node);
        }

        public virtual void SetEntryNode(T node) {
            this.entryNode?.DestroyNode();
            this.entryNode = node;
            if (node == null)
                return;

            this.entryNode.StartNode(this);
            if (this.RealEnable) {
                // 刚设置的时候, 如果tree已经enable过了, 则需要补一次
                this.entryNode.OpenNode();
            }
        }

        protected override void OnDeserializedOverall() {
            this.entryNode?.StartNode(this);
            if (this.RealEnable) {
                // 因为OnDeserialized在Enable之后触发, 所以, 需要补一次
                this.entryNode?.OpenNode();
            }
        }

        protected internal override void OnDestroyFinish() {
            base.OnDestroyFinish();
            this.blackboard = null;
            this.entryNode = default;
            this.currentNode = null;
        }

        protected override void OnEnable() {
            this.entryNode?.OpenNode();
        }

        protected override void OnDisable() {
            this.Abort(); // 因为Disable后, Tick就不执行了, 所以Exit函数也不会被驱动了. 为了确保Enter的节点一定会Exit, 所以要Abort一下
            this.entryNode?.CloseNode();
        }

        protected override void OnDestroy() {
            this.SetEntryNode(default);
        }

        public virtual NodeStatus Tick() {
            if (this.entryNode == null)
                return NodeStatus.Success; // 如果没有节点需要执行, 则默认为执行成功

            var status = this.entryNode.TickNode();
            return status;
        }

        protected override void OnReset() {
            this.entryNode?.ResetNode();
        }

        public virtual void Abort() {
            this.entryNode?.AbortNode();
        }
    }

    [Serializable]
    [MemoryPackable()]
    public partial class BehaviorTree : BehaviorTree<INode>, IBehaviorTree<BehaviorTree>, IUpdate {
        public void Update() {
            this.DeltaTime = TimeInfo.DeltaTime;
            this.Tick();
        }
    }
}