using System.Collections.Generic;

namespace Hsenl {
    public static class ResourcesHelper {
        public static T GetAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object {
            if (!ResourcesManager.Contains(bundleName)) {
                ResourcesManager.LoadBundle(bundleName);
            }

            var asset = ResourcesManager.GetAsset(bundleName, assetName);
            var t = (T)asset;
            return t;
        }

        public static UnityEngine.Object[] GetAssets(string bundleName) {
            if (!ResourcesManager.Contains(bundleName)) {
                ResourcesManager.LoadBundle(bundleName);
            }

            var assets = ResourcesManager.GetAllAssets(bundleName);
            return assets;
        }
    }
}