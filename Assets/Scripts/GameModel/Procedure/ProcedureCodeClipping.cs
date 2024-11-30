using Unity.AI.Navigation;

namespace Hsenl {
    public class ProcedureCodeClipping : AProcedureState {
        protected override HTask OnEnter(IFsm fsm, IFsmState prev) {
            // 防止打包IL2CPP时的代码裁剪
            var navMeshSurface = UnityEngine.Object.FindObjectOfType<Unity.AI.Navigation.NavMeshSurface>();
            return default;
        }

        protected override void OnUpdate(IFsm fsm, float deltaTime) {
            fsm.ChangeState<ProcedurePreloadAssets>();
        }

        protected override HTask OnLeave(IFsm fsm, IFsmState next) {
            return default;
        }
    }
}