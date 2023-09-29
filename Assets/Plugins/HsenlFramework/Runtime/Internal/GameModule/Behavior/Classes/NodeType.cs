using System;

namespace Hsenl {
    [Flags]
    public enum NodeType {
        Composite = 1 << 0,
        Decorator = 1 << 1,
        Condition = 1 << 2,
        Action = 1 << 3,
        
        Bifurcation = Composite | Decorator, // 分叉类节点 (带有子节点的节点)
        Leaf = Condition | Action, // 末节类节点 (不带子节点的节点)
    }
}