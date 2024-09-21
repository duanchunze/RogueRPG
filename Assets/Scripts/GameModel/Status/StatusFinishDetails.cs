namespace Hsenl {
    public enum StatusFinishType {
        // 正常结束
        NormalFinish,
        
        // 主动调用结束, 也是正常的
        InitiativeInvoke,

        // 被迫结束
        ForcedFinish,
    }

    public struct StatusFinishDetails {
        public StatusFinishType finishType;

        /// <summary>
        /// 导致结束的始作俑者, 可能是其他技能, 可能是状态, 也可能是任何东西...
        /// </summary>
        public Object initiator;
    }
}