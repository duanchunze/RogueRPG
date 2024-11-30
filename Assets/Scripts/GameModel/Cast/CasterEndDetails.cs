namespace Hsenl {
    public enum CasterEndType {
        Complated, // 完成后正常退出
        InitiativeInvoke, // 主动调用
        Exclusion, // 被顶掉
        Break, // 打断
    }

    public struct CasterEndDetails {
        /// <summary>
        /// 结束的原因
        /// </summary>
        public CasterEndType endType;

        /// <summary>
        /// 导致结束的始作俑者, 可能是其他技能, 可能是状态, 也可能是任何东西...
        /// </summary>
        public Object initiator;
    }
}