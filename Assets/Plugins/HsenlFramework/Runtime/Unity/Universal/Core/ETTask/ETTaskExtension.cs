using UnityEngine;

namespace Hsenl {
    public static class ETTaskExtension {
        // 有了这个方法，就可以直接await Unity的AsyncOperation了
        public static async ETTask GetAwaiter(this AsyncOperation asyncOperation) {
            var task = ETTask.Create(true);
            asyncOperation.completed += _ => { task.SetResult(); };
            await task;
        }
        
        public static async ETTask<YooAsset.AssetHandle> GetAwaiter(this YooAsset.AssetHandle assetHandle) {
            var task = ETTask<YooAsset.AssetHandle>.Create(true);
            assetHandle.Completed += x => { task.SetResult(x); };
            return await task;
        }

        public static async ETTask<YooAsset.SceneHandle> GetAwaiter(this YooAsset.SceneHandle sceneHandle) {
            var task = ETTask<YooAsset.SceneHandle>.Create(true);
            sceneHandle.Completed += x => { task.SetResult(x); };
            return await task;
        }
    }
}