using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePreloadAssets : AProcedureState {
        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await this.OnEnterShadow(fsm, prev);
        }
        
        protected override void OnUpdate(IFsm fsm, float deltaTime) {
            fsm.ChangeState<ProcedurePreprocessing>();
        }

        [ShadowFunction]
        protected override HTask OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
            return default;
        }
    }
}