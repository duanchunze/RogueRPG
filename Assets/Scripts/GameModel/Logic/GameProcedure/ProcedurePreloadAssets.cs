using SimpleJSON;
using UnityEngine;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePreloadAssets : AProcedureState {
        [ShadowFunction]
        protected override async void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            await HTask.Completed;
            this.OnEnterShadow(manager, prev).Tail();
        }

        [ShadowFunction]
        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.OnLeaveShadow(manager, next);
        }
    }
}