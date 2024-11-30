using System;

namespace Hsenl {
    public enum PriorityStateEnterSuccType {
        
    }
    
    public enum PriorityStateEnterFailType {
        // 没对比者,
        NoContraster,
        
        // 权限足够
        PriorityOk,

        // 被指定放行
        SpecialPass,
        
        // ----- 上面是成功的类型
        
        // 管理器为空
        PrioritiesIsNull,

        // 已经存在
        AlreadyExits,

        // 权限不够
        PriorityLow,

        // 被指定标签拦截
        SpecialIntercept,
    }

    public struct PriorityStateEnterDetails {
        /// <summary>
        /// 失败原因
        /// </summary>
        public PriorityStateEnterFailType FailType { get; set; }
        
        /// <summary>
        /// 谁阻止我进入的
        /// </summary>
        public IPriorityState Blocker { get; set; }
    }

    public enum PriorityStateLeaveType {
        ReEnter, // 重新进入，完全相同的一个行为重新进入
        // Replace, // 替换，相同名字的行为视为替换 // 现在去掉替换这个功能了, 所以这个类型也没了
        TimeOut, // 超时退出
        InitiativeInvoke, // 主动调用
        Exclusion, // 被排挤离开
    }

    public struct PriorityStateLeaveDetails {
        /// <summary>
        /// 离开的原因
        /// </summary>
        public PriorityStateLeaveType LeaveType { get; set; }

        /// <summary>
        /// 导致离开的始作俑者
        /// </summary>
        public IPriorityState Initiator { get; set; }
    }
}