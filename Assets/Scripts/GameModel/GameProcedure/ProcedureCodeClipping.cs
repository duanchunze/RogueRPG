using Unity.AI.Navigation;

namespace Hsenl {
    public class ProcedureCodeClipping : AProcedureState {
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            // 防止打包IL2CPP时的代码裁剪
            var navMeshSurface = UnityEngine.Object.FindObjectOfType<NavMeshSurface>();

            manager.ChangeState<ProcedureMainInterface>();
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) { }
    }
}