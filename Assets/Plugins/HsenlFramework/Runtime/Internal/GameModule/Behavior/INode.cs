using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface INode {
        NodeType NodeType { get; }
        NodeStatus NowStatus { get; }
        INode Parent { get; internal set; }
        void StartNode(IBehaviorTree tree);
        void OpenNode();
        NodeStatus TickNode();
        void CloseNode();
        void DestroyNode();
        void ResetNode();
        void AbortNode();
        void ForeachChildren(Action<INode> callback);
        T GetNodeInParent<T>(bool once = false);
        T GetNodeInChildren<T>(bool once = false);
        T[] GetNodesInChildren<T>(bool once = false);
        void GetNodesInChildren<T>(List<T> cache, bool once = false);
    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface INode<out T> : INode where T : IBehaviorTree { }
}