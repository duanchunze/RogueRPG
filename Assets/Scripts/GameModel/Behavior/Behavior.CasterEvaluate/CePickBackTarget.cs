using System.Collections.Generic;
using Hsenl.casterevaluate;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class CePickBackTarget : CePickTargetBase<PickBackTarget> {
        public override NodeStatus ExtraCheckTarget_Front(ref SelectionTargetDefault target) {
            if (!Shortcut.IsBackForTarget(target.transform, this.tran)) {
                this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.NotBackForMe;
                return NodeStatus.Failure;
            }

            return NodeStatus.Success;
        }

        protected override ASelectionsSelect SelectTarget(SelectorDefault selector, float range, IReadOnlyBitlist constrainsTags, IReadOnlyBitlist exclusiveTags, int count) {
            return GameAlgorithm.SelectBackForMeTargets(selector, range, constrainsTags, exclusiveTags, count);
        }
    }
}