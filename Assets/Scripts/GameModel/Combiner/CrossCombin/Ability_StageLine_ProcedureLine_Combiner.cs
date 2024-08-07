﻿using System;

namespace Hsenl.CrossCombiner {
    [CombinerOptions(crossSplitPosition = 2)]
    public class Ability_StageLine_ProcedureLine_Combiner : CrossCombiner<Ability, StageLine, ProcedureLine> {
        protected override void OnCombin(Ability arg1, StageLine arg2, ProcedureLine arg3) {
            arg2.onStageChanged += this.EnqueueAction<Action<int, int>>((prev, curr) => {
                var form = new PliAbilityStageChangedForm() {
                    attachedBodied = arg3.Bodied,
                    ability = arg1,
                    currStage = curr,
                    stageLine = arg2,
                };

                var result = arg3.StartLine(ref form);
                if (result == ProcedureLineHandleResult.Break) {
                    // arg2.Stop(); // 不主动停止, 依靠下一个技能自己来顶掉当前技能
                }
            });
        }

        protected override void OnDecombin(Ability arg1, StageLine arg2, ProcedureLine arg3) {
            arg2.onStageChanged -= this.DequeueAction<Action<int, int>>();
        }
    }
}