using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface INode {
        NodeType NodeType { get; }
        NodeStatus NowStatus { get; }
        INode Parent { get; internal set; }
        IEnumerable<INode> ForeachChildren();
        void StartNode(IBehaviorTree tree);
        void OpenNode();
        NodeStatus TickNode();
        void CloseNode();
        void DestroyNode();
        void ResetNode();
        void AbortNode();
        T GetNodeInParent<T>(bool once = false) where T : INode;
        T GetNodeInChildren<T>(bool once = false) where T : INode;
    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface INode<out T> : INode where T : IBehaviorTree { }
}