using Unity.AI.Navigation;

namespace Hsenl {
    public class ProcedureCodeClipping : AProcedureState {
        protected override void OnEnter(IFsm fsm, IFsmState prev) {
            // 防止打包IL2CPP时的代码裁剪
            var navMeshSurface = UnityEngine.Object.FindObjectOfType<Unity.AI.Navigation.NavMeshSurface>();
            fsm.ChangeState<ProcedurePreloadAssets>();
        }

        protected override void OnLeave(IFsm fsm, IFsmState next) { }
    }
}