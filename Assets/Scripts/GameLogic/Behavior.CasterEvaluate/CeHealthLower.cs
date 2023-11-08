using Hsenl.casterevaluate;
using MemoryPack;

namespace Hsenl {
    // 血量低于一定值
    [MemoryPackable()]
    public partial class CeHealthLower : CeInfo<HealthLowerInfo> {
        private Numerator _numerator;

        protected override void OnNodeOpen() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._numerator = ability.Owner.GetComponent<Numerator>();        
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            var pct = Shortcut.GetHealthPct(this._numerator);
            if (pct < this.info.Threshold) {
                return NodeStatus.Success;
            }
            
            return NodeStatus.Failure;
        }
    }
}