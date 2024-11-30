using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class CeManaCheck : CeInfo<casterevaluate.ManaCheckInfo> {
        private Numerator _numerator;
        private Numerator _abiNumerator;

        protected override void OnEnable() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._numerator = ability.MainBodied?.GetComponent<Numerator>();
                    this._abiNumerator = ability.GetComponent<Numerator>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var manaCost = this._abiNumerator.GetValue(NumericType.ManaCost);
                    if (manaCost > 0) {
                        var mana = this._numerator.GetValue(NumericType.Mana);
                        if (mana < manaCost) {
                            this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.Mana;
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