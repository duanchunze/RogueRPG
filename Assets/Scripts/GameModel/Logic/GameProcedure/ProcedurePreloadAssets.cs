using SimpleJSON;
using UnityEngine;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePreloadAssets : AProcedureState {
        [ShadowFunction]
        protected override async void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            await ETTask.CompletedTask;
            this.OnEnterShadow(manager, prev);
        }

        [ShadowFunction]
        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.OnLeaveShadow(manager, next);
        }
    }
}