using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

// ReSharper disable all

namespace Hsenl {
    public class Launch : MonoBehaviour {
        private Dictionary<string, AssetBundle> _assetBundles = new();
        private Dictionary<string, Dictionary<string, UnityEngine.Object>> _assets = new();

        private void Start() {
            // var dict = LoadBundle("code.unity3d");
            //
            // var assBytes = ((TextAsset)dict["Entry.dll"]).bytes;
            // var pdbBytes = ((TextAsset)dict["Entry.pdb"]).bytes;
            //
            // var assembly = Assembly.Load(assBytes, pdbBytes);
            // UnloadBundle("code.unity3d", true);
            // var method = assembly.GetType("Entry").GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            // method?.Invoke(null, null);

            // todo 暂时使用这种方式
            LoadBundle("entry.unity3d");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Entry");
        }

        private Dictionary<string, UnityEngine.Object> LoadBundle(string assetBundleName) {
            assetBundleName = assetBundleName.ToLower();
            if (this._assets.TryGetValue(assetBundleName, out var result)) {
                return result;
            }

            var objects = new Dictionary<string, UnityEngine.Object>();

            var p = Path.Combine(AppHotfixResPath, assetBundleName);
            AssetBundle assetBundle;
            if (File.Exists(p)) {
                assetBundle = AssetBundle.LoadFromFile(p);
            }
            else {
                p = Path.Combine(AppResPath, assetBundleName);
                assetBundle = AssetBundle.LoadFromFile(p);
            }

            if (assetBundle == null) {
                Debug.LogError($"assets bundle not found: {assetBundleName}");
                return objects;
            }

            if (!assetBundle.isStreamedSceneAssetBundle) {
                var assets = assetBundle.LoadAllAssets();
                foreach (var asset in assets) {
                    objects.Add(asset.name, asset);
                }
            }

            this._assetBundles[assetBundleName] = assetBundle;
            this._assets[assetBundleName] = objects;
            return objects;
        }

        public void UnloadBundle(string assetBundleName, bool unloadAll) {
            assetBundleName = assetBundleName.ToLower();
            if (this._assetBundles.TryGetValue(assetBundleName, out var bundle)) {
                bundle.Unload(unloadAll);
            }

            this._assets.Remove(assetBundleName);
        }

        /// <summary>
        ///应用程序外部资源路径存放路径(热更新资源路径)
        /// </summary>
        private static string AppHotfixResPath {
            get {
                var game = Application.productName;
                var path = AppResPath;
                if (Application.isMobilePlatform) {
                    path = $"{Application.persistentDataPath}/{game}/";
                }

                return path;
            }
        }

        /// <summary>
        /// 应用程序内部资源路径存放路径
        /// </summary>
        private static string AppResPath => Application.streamingAssetsPath;

        /// <summary>
        /// 应用程序内部资源路径存放路径(www/webrequest专用)
        /// </summary>
        private static string AppResPath4Web {
            get {
#if UNITY_IOS || UNITY_STANDALONE_OSX
                return $"file://{Application.streamingAssetsPath}";
#else
                return Application.streamingAssetsPath;
#endif
            }
        }
    }
}