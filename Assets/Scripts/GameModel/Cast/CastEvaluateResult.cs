namespace Hsenl {
    public class CastEvaluateResult {
        public CastEvaluateState CastEvaluateState { get; set; }

        public IReadOnlyBitlist ConstrainsTags { get; set; }
        public float CastRange { get; set; }
        public float DetectRange { get; set; }
        public SelectionTargetDefault CastedTarget { get; set; } // 评估器提供的目标, 比如与目标距离不够时, 评估器就把目标赋值到这里, 方可供外部处理

        public void Reset() {
            this.CastEvaluateState = default;
            this.ConstrainsTags = null;
            this.CastRange = 0;
            this.DetectRange = 0;
            this.CastedTarget = null;
        }
    }
}