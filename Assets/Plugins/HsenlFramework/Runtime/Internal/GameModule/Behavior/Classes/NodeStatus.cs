using System;

namespace Hsenl {
    [Flags]
    public enum NodeStatus {
        // 标准状态
        Failure = 1 << 0, // 执行失败 (三个标准核心状态之一)
        Success = 1 << 1, // 执行成功 (三个标准核心状态之一)
        Running = 1 << 2, // 正在执行 (三个标准核心状态之一)
        
        // 特殊状态
        Continue = 1 << 3, // 由内部返回时会跳过当前节点, 参考函数里的 continue. 该状态表达出该节点一种"不参与、不粘锅"的态度, 在或逻辑中, 他代表失败, 在与逻辑中, 他代表成功
        Break = 1 << 4,   // 由内部返回时会终止当前这一脉复合分支继续向下执行, 参考函数中的 break. 它与上面的Continue是一种对应关系, 在或逻辑中, 他代表成功, 在与逻辑中, 他代表失败
        Return = 1 << 5, // 由内部返回时会终止整个行为树继续向下执行, 参考函数里的 return. 而由外部Abort时, 会默认用该状态作为返回状态
        
        NormalStatus = Success | Failure | Running,
        SpecialStatus = Continue | Break | Return,
        AbortStatus = Break | Return, // 这两个状态都会起到中断的作用
    }
}