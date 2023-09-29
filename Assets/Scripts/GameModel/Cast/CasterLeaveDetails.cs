namespace Hsenl {
    public enum CasterLeaveType {
        Complated, // 完成后正常退出
        InitiativeInvoke, // 主动调用
        Exclusion, // 被顶掉
    }

    public struct CasterLeaveDetails {
        /// <summary>
        /// 离开的原因
        /// </summary>
        public CasterLeaveType leaveType;

        /// <summary>
        /// 导致离开的始作俑者, 可能是其他技能, 可能是状态, 也可能是任何东西...
        /// </summary>
        public Object initiator;
    }
}