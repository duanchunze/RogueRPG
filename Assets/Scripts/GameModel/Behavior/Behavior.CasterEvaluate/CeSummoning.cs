using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class CeSummoning : CeInfo<casterevaluate.CasterSummoningInfo> {
        private List<Numerator> _numerators = new(2);

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = owner?.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var stageline = ability.GetComponent<StageLine>();
                    // 获得允许的最大召唤数
                    var msn = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Msn);
                    var list = stageline.Blackboard.GetOrCreateData<List<Minion>>("MaxSummoningNum");
                    if (list.Count < msn) {
                        return NodeStatus.Success;
                    }

                    this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.MoreThanMaxSummoningNum;
                    break;
                }
            }

            return NodeStatus.Failure;
        }
    }
}