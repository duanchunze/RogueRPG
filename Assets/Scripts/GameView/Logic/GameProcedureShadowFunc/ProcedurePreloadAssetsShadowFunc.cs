using System;
using SimpleJSON;
using UnityEngine;
using YooAsset;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedurePreloadAssets))]
    public static partial class ProcedurePreloadAssetsShadowFunc {
        [ShadowFunction]
        private static async HTask OnEnter(ProcedurePreloadAssets self, ProcedureManager manager, FsmState<ProcedureManager> prev) {
            await HTask.Completed;

            // 加载配置文件
            Tables.Instance = new Tables(s => {
                var textAsset = YooAssets.LoadAssetSync<TextAsset>(s)?.AssetObject as TextAsset;
                return JSON.Parse(textAsset!.text);
            });

            manager.ChangeState<ProcedurePreprocessing>();
        }

        [ShadowFunction]
        private static void OnLeave(ProcedurePreloadAssets self, ProcedureManager manager, FsmState<ProcedureManager> prev) { }
    }
}