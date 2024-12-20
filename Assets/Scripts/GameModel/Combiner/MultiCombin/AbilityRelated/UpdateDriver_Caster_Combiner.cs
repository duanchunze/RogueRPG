﻿using System;

namespace Hsenl.MultiCombiner {
    public class UpdateDriver_Caster_Combiner : MultiCombiner<UpdateDriver, Caster> {
        protected override void OnCombin(UpdateDriver arg1, Caster arg2) {
            arg1.updateInvoke += this.EnqueueAction<Action>(() => {
                arg2.StartCast();
            });
        }

        protected override void OnDecombin(UpdateDriver arg1, Caster arg2) {
            arg1.updateInvoke -= this.DequeueAction<Action>();
        }
    }
}