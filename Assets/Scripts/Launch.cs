using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

// ReSharper disable all

namespace Hsenl {
    // Launch只负责一件事, 在启动时, 动态加载Entry程序集, 调用里面的入口, 然后剩下的就交给它了
    public class Launch : MonoBehaviour {
        /// <summary>
        /// 应用程序外部资源路径存放路径(热更新资源路径)
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

        private void Start() {
            var bundleName = "aotdllentry.unity3d";
            var path = Path.Combine(AppHotfixResPath, bundleName);
            if (File.Exists(path)) {
                path = Path.Combine(AppResPath, bundleName);
            }

            var assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null) {
                Debug.LogError("asset bundle not found: 'aotdllentry.unity3d'");
                return;
            }

            try {
                byte[] dllBytes = null;
                byte[] pdbBytes = null;
                foreach (var asset in assetBundle.LoadAllAssets()) {
                    switch (asset.name) {
                        case "HsenlFramework.Entry.dll":
                            dllBytes = ((TextAsset)asset).bytes;
                            break;

                        case "HsenlFramework.Entry.pdb":
                            pdbBytes = ((TextAsset)asset).bytes;
                            break;
                    }
                }

                var assembly = Assembly.Load(dllBytes, pdbBytes);

                var entryType = assembly.GetType("Entry");
                var method = entryType.GetMethod("Start", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                method.Invoke(null, null);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
            finally {
                assetBundle.Unload(true);
            }
        }
    }
}