using SimpleJSON;
using UnityEngine;

namespace Hsenl {
    public class ProcedurePreloadAssets : AProcedureState {
        protected override async void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            await ETTask.CompletedTask;

            // 预加载资源
            await ResourcesManager.LoadBundleAsync(Constant.ConfigBundleName);
            await ResourcesManager.LoadBundleAsync(Constant.UIBundleName);
            // await ResourcesManager.LoadBundleAsync(Constant.InputBundleName);
            // await ResourcesManager.LoadBundleAsync(Constant.AbilityBundleName);
            // await ResourcesManager.LoadBundleAsync(Constant.StatusBundleName);
            await ResourcesManager.LoadBundleAsync(Constant.AudioBundleName);
            await ResourcesManager.LoadBundleAsync(Constant.FxBundleName);

            // 加载资源清单, 用于智能匹配资源
            {
                var textAsset = (TextAsset)ResourcesManager.GetAsset(Constant.ConfigBundleName, "AssetsManifest");
                AssetsManifest.Instance = SerializeHelper.Deserialize(typeof(AssetsManifest), textAsset.bytes, 0, textAsset.bytes.Length) as AssetsManifest;
            }

            // 加载配置文件
            Tables.Instance = new Tables(s => {
                var textAsset = (TextAsset)ResourcesManager.GetAsset(Constant.ConfigBundleName, $"{s}");
                return JSON.Parse(textAsset.text);
            });

            manager.ChangeState<ProcedurePreprocessing>();
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
        }
    }
}