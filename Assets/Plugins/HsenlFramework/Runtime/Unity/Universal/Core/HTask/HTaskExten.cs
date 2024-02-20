using UnityEngine;

namespace Hsenl {
    public static class HTaskExten {
        public static async HTask GetAwaiter(this AsyncOperation asyncOperation) {
            var task = HTask.Create();
            asyncOperation.completed += _ => { task.SetResult(); };
            await task;
        }

        public static HTask<YooAsset.AssetHandle>.Awaiter GetAwaiter(this YooAsset.AssetHandle assetHandle) {
            var task = HTask<YooAsset.AssetHandle>.Create();
            assetHandle.Completed += x => { task.SetResult(x); };
            return task.GetAwaiter();
        }

        public static HTask<YooAsset.SceneHandle>.Awaiter GetAwaiter(this YooAsset.SceneHandle sceneHandle) {
            var task = HTask<YooAsset.SceneHandle>.Create();
            sceneHandle.Completed += x => { task.SetResult(x); };
            return task.GetAwaiter();
        }
    }
}