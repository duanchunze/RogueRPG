using System.Collections.Generic;

namespace Hsenl {
    public static class ResourcesHelper {
        private static readonly List<string> _optimalBundleNameCache = new();

        public static bool Contains(string bundleName) {
            return false;
        }

        public static void LoadBundle(string bundleName) {
        }

        public static async HTask LoadBundleAsync(string bundleName) {
            await HTask.Completed;
        }
        
        public static UnityEngine.Object GetAsset(string bundleName, string assetName) {
            return default;
        }

        public static T GetAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object {
            // if (!ResourcesManager.Contains(bundleName)) {
            //     ResourcesManager.LoadBundle(bundleName);
            // }
            //
            // var asset = ResourcesManager.GetAsset(bundleName, assetName);
            // var t = (T)asset;
            // return t;
            return default;
        }

        public static UnityEngine.Object[] GetAssets(string bundleName) {
            // if (!ResourcesManager.Contains(bundleName)) {
            //     ResourcesManager.LoadBundle(bundleName);
            // }
            //
            // var assets = ResourcesManager.GetAllAssets(bundleName);
            // return assets;
            return default;
        }

        public static void Clear() {
            _optimalBundleNameCache.Clear();
        }

        public static void Append(string content) {
            if (string.IsNullOrEmpty(content))
                return;

            _optimalBundleNameCache.Add(content);
        }

        public static string GetOptimalBundleName() {
            return default;
            // if (!AssetsManifest.Instance.GetOptimalBundleName(_optimalBundleNameCache, out var bundleName))
            //     return null;
            //
            // return bundleName;
        }

        public static T GetOptimalAsset<T>(string assetName) where T : UnityEngine.Object {
            return default;
            // if (!AssetsManifest.Instance.GetOptimalBundleName(_optimalBundleNameCache, out var bundleName))
            //     return null;
            //
            // var asset = GetAsset<T>(bundleName, assetName);
            // return asset;
        }
    }
}