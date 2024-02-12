using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            const string assemblyName = "HsenlFramework.Entry";
            const string typeName = "Entry";
            const string methodName = "Start";
#if UNITY_EDITOR
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName);
            if (assembly == null) throw new Exception($"Assembly '{assembly}' cannot be found!");

            Type entryType = entryType = assembly.GetTypes().FirstOrDefault(type => type.Name == typeName);
            if (entryType == null) throw new Exception($"Type '{typeName}' cannot be found from assembly '{assembly}'!");

            var method = entryType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null) throw new Exception($"Method '{methodName}' cannot be found from type '{typeName}'!");

            Debug.Log($"Launch Success!");
            method.Invoke(null, null);
#else
            // 加载入口包
            var bundleName = "dllentry.unity3d";
            var path = Path.Combine(AppHotfixResPath, bundleName);
            if (!File.Exists(path)) {
                path = Path.Combine(AppResPath, bundleName);
            }

            var assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null) {
                throw new Exception($"Asset bundle '{bundleName}' can not found!");
            }

            try {
                // 加载入口程序集
                byte[] dllBytes = null;
                byte[] pdbBytes = null;
                foreach (var asset in assetBundle.LoadAllAssets()) {
                    if (asset.name == $"{assemblyName}.dll") {
                        dllBytes = ((TextAsset)asset).bytes;
                    }
                    else if (asset.name == $"{assemblyName}.pdb") {
                        pdbBytes = ((TextAsset)asset).bytes;
                    }
                }

                if (dllBytes == null) throw new Exception($"Asset '{assemblyName}.dll' cannot found from bundle '{bundleName}'!");

                var assembly = Assembly.Load(dllBytes, pdbBytes);

                // 找到入口类
                Type entryType = entryType = assembly.GetTypes().FirstOrDefault(type => type.Name == typeName);
                if (entryType == null) throw new Exception($"Type '{typeName}' cannot be found from assembly '{assemblyName}'!");

                // 找到入口函数
                var method = entryType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method == null) throw new Exception($"Method '{methodName}' cannot be found from type '{typeName}'!");

                // 执行入口函数
                Debug.Log($"Launch Success!");
                method.Invoke(null, null);
            }
            catch (Exception e) {
                UnityEngine.Debug.LogError(e);
            }
            finally {
                assetBundle.Unload(true);
            }
#endif
        }
    }
}