using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class CeManaCheck : CeInfo<casterevaluate.ManaCheckInfo> {
        private Numerator _numerator;

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._numerator = ability.GetHolder()?.GetComponent<Numerator>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    if (ability.manaCost > 0) {
                        var mana = this._numerator.GetValue(NumericType.Mana);
                        if (mana < ability.manaCost) {
                            this.manager.status = CastEvaluateStatus.Mana;
                            return NodeStatus.Failure;
                        }
                    }

                    return NodeStatus.Success;
                }
            }

            return NodeStatus.Success;
        }
    }
}