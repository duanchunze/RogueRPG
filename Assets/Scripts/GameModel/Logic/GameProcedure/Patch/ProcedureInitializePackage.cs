using YooAsset;

namespace Hsenl {
    public class ProcedureInitializePackage : AProcedureState {
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            var packageName = "DefaultPackage";
            // 创建资源包裹类
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
                package = YooAssets.CreatePackage(packageName);
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) { }
    }
}