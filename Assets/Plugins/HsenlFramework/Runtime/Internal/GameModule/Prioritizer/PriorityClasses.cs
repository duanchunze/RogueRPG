using System;

namespace Hsenl {
    public enum PriorityStateEnterFailType {
        // 管理器为空
        PrioritiesIsNull,

        // 不许重进
        NoReentry,

        // 权限不够
        PriorityLow,

        // 被指定标签拦截
        SpecialIntercept,
    }

    public interface IPriorityStateEnterFailDetails : IDisposable {
        public PriorityStateEnterFailType FailType { get; set; }
        public IPriorityState Blocker { get; set; } // 阻挡者
    }

    public class PriorityStateEnterFailDetails : IPriorityStateEnterFailDetails {
        public PriorityStateEnterFailType FailType { get; set; }
        public IPriorityState Blocker { get; set; }

        public void Dispose() {
            this.FailType = 0;
            this.Blocker = null;
        }
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
        public PriorityStateLeaveType leaveType;

        /// <summary>
        /// 导致离开的始作俑者
        /// </summary>
        public IPriorityState initiator;
    }
}