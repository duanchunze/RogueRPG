namespace Hsenl {
    public struct PliStatusChangesForm {
        public int changeType; // 0: infliction, 1: termination

        public Bodied inflictor; // 施加者
        public Bodied target; // 被施加目标
        public string statusAlias; // 状态名
        public float duration; // 持续时间

        public StatusFinishDetails finishDetails; // 结束的细节

        public Status status; // 状态本体
    }
}