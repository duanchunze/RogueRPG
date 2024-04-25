using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePreloadAssets : AProcedureState {
        [ShadowFunction]
        protected override async void OnEnter(IFsm fsm, IFsmState prev) {
            await HTask.Completed;
            await this.OnEnterShadow(fsm, prev);
            fsm.ChangeState<ProcedurePreprocessing>();
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
        }
    }
}