﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HybridCLR;
using UnityEngine;

public static class Entry {
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

    public static void Start() {
        LoadMetadataForAOTAssemblies();
        LoadHotUpdateDlls();
    }

    private static void LoadMetadataForAOTAssemblies() {
        var bundleName = "dllmetadatas.unity3d";
        var assetBundle = LoadAssetBundle(bundleName);
        if (assetBundle == null) throw new Exception($"AssetBunle '{bundleName}' cannot found!");
        var dllAssets = CacheAssetBundle<TextAsset>(assetBundle);
        LoadAllMetadataForAOTAssemblyFromBundleCache(dllAssets);

        Debug.Log("Load all metadata succ!");
    }

    private static void LoadHotUpdateDlls() {
        var dlls = new List<string>() {
            "HsenlFramework.ThirdParty",
            "HsenlFramework.Runtime",
            "GameModel",
            "GameHotReload",
            "GameView",
        };

#if UNITY_EDITOR
        UnityEngine.SceneManagement.SceneManager.LoadScene("Entry", UnityEngine.SceneManagement.LoadSceneMode.Single);
        Debug.Log("Entry start succ!");
#else
        // 尝试加载包
        string bundleName = "dlls.unity3d";
        var dllAssetBundle = LoadAssetBundle(bundleName);
        if (dllAssetBundle == null) throw new Exception($"Asset bundle not found: '{bundleName}'!");

        try {
            // 缓存包里所有的程序集资源
            var assets = CacheAssetBundle<TextAsset>(dllAssetBundle);

            // 评估程序集是否有遗漏
            foreach (var dll in dlls) {
                if (!assets.ContainsKey($"{dll}.dll")) throw new Exception($"Assembly '{dll}' cannot found!");
            }

            // 加载所有程序集
            foreach (var dll in dlls) {
                LoadAssemblyFromAssetBundleCache(assets, dll);
                Debug.Log($"Load assembly '{dll}' succ!");
            }

            Debug.Log("Load all assemblies succ!");
        }
        catch (Exception e) {
            throw e;
        }
        finally {
            dllAssetBundle.Unload(true);
        }

        // 加载入口场景包
        var sceneBundleName = "Entry.unity3d";
        var sceneAssetBundle = LoadAssetBundle(sceneBundleName);
        if (sceneAssetBundle == null) throw new Exception($"Asset bundle not found: '{sceneBundleName}'!");

        // 加载入口场景
        UnityEngine.SceneManagement.SceneManager.LoadScene("Entry", UnityEngine.SceneManagement.LoadSceneMode.Single);
        Debug.Log("Entry start succ!");
#endif
    }

    private static AssetBundle LoadAssetBundle(string bundleName) {
        var path = Path.Combine(AppHotfixResPath, bundleName);
        if (!File.Exists(path)) {
            path = Path.Combine(AppResPath, bundleName);
        }

        var assetBundle = AssetBundle.LoadFromFile(path);
        return assetBundle;
    }

    private static Dictionary<string, UnityEngine.Object> CacheAssetBundle(AssetBundle assetBundle) {
        return CacheAssetBundle<UnityEngine.Object>(assetBundle);
    }

    private static Dictionary<string, T> CacheAssetBundle<T>(AssetBundle assetBundle) {
        var dict = new Dictionary<string, T>();
        foreach (var asset in assetBundle.LoadAllAssets()) {
            if (asset is not T t) continue;
            dict.Add(asset.name, t);
        }

        return dict;
    }

    private static Assembly LoadAssemblyFromAssetBundleCache(Dictionary<string, TextAsset> cache, string assemblyName) {
        byte[] dllBytes = null;
        byte[] pdbBytes = null;
        if (!cache.TryGetValue($"{assemblyName}.dll", out var dllText)) {
            return null;
        }

        dllBytes = dllText.bytes;
        if (cache.TryGetValue($"{assemblyName}.pdb", out var pdbText)) {
            pdbBytes = pdbText.bytes;
        }

        var assembly = Assembly.Load(dllBytes, pdbBytes);
        return assembly;
    }

    private static void LoadAllAssemblyFromAssetBundleCache(Dictionary<string, TextAsset> cache) {
        foreach (var kv in cache) {
            if (kv.Key.EndsWith(".dll")) {
                var pdbName = kv.Key.Replace(".dll", ".pdb");
                byte[] dllBytes = null;
                byte[] pdbBytes = null;
                dllBytes = kv.Value.bytes;
                if (cache.TryGetValue(pdbName, out var pdbText)) pdbBytes = pdbText.bytes;

                Assembly.Load(dllBytes, pdbBytes);
            }
        }
    }

    private static void LoadAllMetadataForAOTAssemblyFromBundleCache(Dictionary<string, TextAsset> cache) {
        foreach (var kv in cache) {
            if (kv.Key.EndsWith(".dll")) {
                var dllBytes = kv.Value.bytes;
                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly:{kv.Key}. ret:{err}");
            }
        }
    }
}