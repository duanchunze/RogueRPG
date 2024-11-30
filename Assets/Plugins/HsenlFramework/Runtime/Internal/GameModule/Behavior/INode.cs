using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface INode {
        NodeType NodeType { get; }
        NodeStatus NowStatus { get; }
        INode Parent { get; internal set; }
        void AwakeNode(IBehaviorTree tree);
        void OpenNode();
        NodeStatus TickNode();
        void CloseNode();
        void DestroyNode();
        void StartNode();
        void AbortNode();
        Iterator<INode> ForeachChildren();
        T GetNodeInParent<T>();
        T GetNodeInChildren<T>();
        T[] GetNodesInChildren<T>();
        void GetNodesInChildren<T>(List<T> cache);
    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface INode<out T> : INode where T : IBehaviorTree { }
}