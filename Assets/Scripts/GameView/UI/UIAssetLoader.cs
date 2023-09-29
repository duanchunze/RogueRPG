namespace Hsenl.View {
    public class UIAssetLoader : IAssetsLoader {
        public UnityEngine.Object GetAssetSync(string bundleName, string assetName) {
            bundleName ??= Constant.UIBundleName;

            if (!ResourcesManager.Contains(bundleName)) {
                ResourcesManager.LoadBundle(bundleName);
            }

            return ResourcesManager.GetAsset(bundleName, assetName);
        }

        public async ETTask<UnityEngine.Object> GetAssetAsync(string bundleName, string assetName) {
            bundleName ??= Constant.UIBundleName;

            if (!ResourcesManager.Contains(bundleName)) {
                await ResourcesManager.LoadBundleAsync(bundleName);
            }

            return ResourcesManager.GetAsset(bundleName, assetName);
        }
    }
}