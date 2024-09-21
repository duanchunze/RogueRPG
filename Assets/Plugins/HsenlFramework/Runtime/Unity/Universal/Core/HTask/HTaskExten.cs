using UnityEngine;
using YooAsset;

namespace Hsenl {
    public static class HTaskExten {
        public static async HTask GetAwaiter(this AsyncOperation asyncOperation) {
            var task = HTask.Create();
            asyncOperation.completed += _ => { task.SetResult(); };
            await task;
        }

        public static HTask<YooAsset.AssetHandle>.Awaiter GetAwaiter(this YooAsset.AssetHandle assetHandle) {
            var task = HTask<YooAsset.AssetHandle>.Create();

            assetHandle.Completed += OnAssetHandleOnCompleted;
            return task.GetAwaiter();

            void OnAssetHandleOnCompleted(AssetHandle x) {
                assetHandle.Completed -= OnAssetHandleOnCompleted;
                task.SetResult(x);
            }
        }

        public static HTask<YooAsset.SceneHandle>.Awaiter GetAwaiter(this YooAsset.SceneHandle sceneHandle) {
            var task = HTask<YooAsset.SceneHandle>.Create();
            sceneHandle.Completed += x => { task.SetResult(x); };
            return task.GetAwaiter();
        }
    }
}