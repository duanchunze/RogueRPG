using Hsenl.casterevaluate;
using MemoryPack;

namespace Hsenl {
    // 血量低于一定值
    [MemoryPackable()]
    public partial class CeHealthLower : CeInfo<HealthLowerInfo> {
        private Numerator _holderNumerator;

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._holderNumerator = ability.GetHolder().GetComponent<Numerator>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            var pct = Shortcut.GetHealthPct(this._holderNumerator);
            if (pct < this.info.Threshold) {
                return NodeStatus.Success;
            }
            
            return NodeStatus.Failure;
        }
    }
}