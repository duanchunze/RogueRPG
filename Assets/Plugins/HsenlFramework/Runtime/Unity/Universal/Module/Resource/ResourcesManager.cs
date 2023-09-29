using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class ResourcesManager : UnitySingleton<ResourcesManager> {
        public bool editorMode;

        private readonly Dictionary<string, string> _bundleNameToLowerCache = new();
        public readonly Dictionary<string, string> stringToBundleCache = new();

        // 依赖、包、资源，三个缓存
        private readonly Dictionary<string, string[]> _dependenciesCache = new();
        private readonly Dictionary<string, BundleInfo> _bundleCache = new();
        [ShowInInspector, ReadOnly] private readonly MultiDictionary<string, string, UnityEngine.Object> _assetCache = new();
        private UnityEngine.AssetBundleManifest _assetBundleManifest;

        private bool UseEditor => Define.IsEditor && this.editorMode;

        protected override void Awake() {
            base.Awake();
            if (!this.UseEditor) {
                LoadOneBundle("StreamingAssets");
                this._assetBundleManifest = (UnityEngine.AssetBundleManifest)GetAsset("StreamingAssets", "AssetBundleManifest");
                UnloadOneBundle("StreamingAssets", false);
            }
        }

        private void OnDestroy() {
            foreach (var bundleInfo in this._bundleCache) {
                bundleInfo.Value.Unload();
            }

            this._bundleCache.Clear();
            this._bundleNameToLowerCache.Clear();
            this._dependenciesCache.Clear();
            this._assetCache.Clear();
            if (this._assetBundleManifest != null) {
                UnityEngine.Object.DestroyImmediate(this._assetBundleManifest, true);
                this._assetBundleManifest = null;
            }
        }

        public static bool Contains(string bundleName) {
            return Instance._bundleCache.ContainsKey(bundleName);
        }

        public static UnityEngine.Object GetAsset(string bundleName, string assetName) {
            // bundleName = BundleNameToLower(bundleName);
            if (!Instance._assetCache.TryGetValue(bundleName, out var dict)) {
                throw new Exception($"not found asset (bundle not find): {bundleName} {assetName}");
            }

            if (!dict.TryGetValue(assetName, out var asset)) {
                throw new Exception($"not found asset (asset not find): {bundleName} {assetName}");
            }

            return asset;
        }

        public static UnityEngine.Object[] GetAllAssets(string bundleName) {
            // bundleName = BundleNameToLower(bundleName);
            if (!Instance._assetCache.TryGetValue(bundleName, out var dict)) {
                throw new Exception($"not found asset (bundle not find): {bundleName}");
            }

            return dict.Values.ToArray();
        }

        public static void LoadBundle(string assetBundleName) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            var dependencies = GetSortedDependencies(assetBundleName);
            foreach (var dependency in dependencies) {
                if (string.IsNullOrEmpty(dependency)) continue;

                LoadOneBundle(dependency);
            }
        }

        // 获得一个包的所有依赖包，并按照依赖信息排列好顺序，越末节的包排的越靠前
        private static string[] GetSortedDependencies(string assetBundleName) {
            var info = new Dictionary<string, int>();
            var parents = new List<string>();
            CollectDependencies(assetBundleName, parents, info);
            var ss = info.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
            return ss;
        }

        // 递归收集所有的依赖信息（每个包所依赖的个数）
        private static void CollectDependencies(string assetBundleName, List<string> parents, Dictionary<string, int> info) {
            parents.Add(assetBundleName);
            var deps = GetDependencies(assetBundleName);
            foreach (var parent in parents) {
                if (!info.ContainsKey(parent)) {
                    info[parent] = 0;
                }

                info[parent] += deps.Length;
            }

            foreach (var dep in deps) {
                if (parents.Contains(dep)) {
                    throw new Exception($"包有循环依赖，请重新标记: {assetBundleName} {dep}");
                }

                CollectDependencies(dep, parents, info);
            }

            parents.RemoveAt(parents.Count - 1);
        }

        // 获取一个包的所有依赖包
        private static string[] GetDependencies(string assetBundleName) {
            if (Instance._dependenciesCache.TryGetValue(assetBundleName, out var dependencies)) {
                return dependencies;
            }

            if (Instance.UseEditor) {
                dependencies = AssetBundleHelper.GetAssetBundleDependencies(assetBundleName, true);
            }
            else {
                dependencies = Instance._assetBundleManifest.GetAllDependencies(assetBundleName);
            }

            Instance._dependenciesCache.Add(assetBundleName, dependencies);
            return dependencies;
        }

        private static void LoadOneBundle(string assetBundleName) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            if (Instance._bundleCache.TryGetValue(assetBundleName, out var bundleInfo)) {
                ++bundleInfo.refCount;
                return;
            }

            if (Instance.UseEditor) {
                var realPaths = AssetBundleHelper.GetAssetPathsFromAssetBundle(assetBundleName);
                // 缓存资源
                foreach (var realPath in realPaths) {
                    var assetName = Path.GetFileNameWithoutExtension(realPath);
                    var resource = AssetBundleHelper.LoadAssetAtPath(realPath);
                    CacheAsset(assetBundleName, assetName, resource);
                }

                // 缓存包
                if (realPaths.Length > 0) {
                    bundleInfo = new BundleInfo(assetBundleName, null);
                    Instance._bundleCache[assetBundleName] = bundleInfo;
                }
                else {
                    Log.Error($"assets bundle not found: {assetBundleName}");
                }

                return;
            }

            var path = Path.Combine(UnityHelper.Path.AppHotfixResPath, assetBundleName);
            UnityEngine.AssetBundle assetBundle;
            if (File.Exists(path)) {
                assetBundle = UnityEngine.AssetBundle.LoadFromFile(path);
            }
            else {
                path = Path.Combine(UnityHelper.Path.AppResPath, assetBundleName);
                assetBundle = UnityEngine.AssetBundle.LoadFromFile(path);
            }

            if (assetBundle == null) {
                // 获取资源的时候会抛异常，这个地方不直接抛异常，因为有些地方需要Load之后判断是否Load成功
                Log.Warning($"assets bundle not found: {assetBundleName}");
                return;
            }

            // 缓存资源
            if (!assetBundle.isStreamedSceneAssetBundle) {
                var assets = assetBundle.LoadAllAssets();
                foreach (var asset in assets) {
                    CacheAsset(assetBundleName, asset.name, asset);
                }
            }

            // 缓存包
            bundleInfo = new BundleInfo(assetBundleName, assetBundle);
            Instance._bundleCache[assetBundleName] = bundleInfo;
        }

        private static void CacheAsset(string bundleName, string assetName, UnityEngine.Object asset) {
            Instance._assetCache[bundleName, assetName] = asset;
        }

        public static async ETTask UnloadBundleAsync(string assetBundleName, bool unloadAllLoadedObjects = true) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            var deps = GetSortedDependencies(assetBundleName);
            foreach (var dep in deps) {
                // 一帧卸载一个包，避免卡死
                using (await TaskLock.Wait(TaskLockType.Resources, assetBundleName.GetHashCode())) {
                    UnloadOneBundle(dep, unloadAllLoadedObjects);
                    await Timer.WaitFrame();
                }
            }
        }

        public static void UnloadBundle(string assetBundleName, bool unloadAllLoadedObjects = true) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            var deps = GetSortedDependencies(assetBundleName);
            foreach (var dep in deps) {
                UnloadOneBundle(dep, unloadAllLoadedObjects);
            }
        }

        private static void UnloadOneBundle(string assetBundleName, bool unloadAllLoadedObjects = true) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            if (!Instance._bundleCache.TryGetValue(assetBundleName, out var bundleInfo)) {
                return;
            }

            --bundleInfo.refCount;

            if (bundleInfo.refCount > 0) return;

            Instance._bundleCache.Remove(assetBundleName);
            Instance._assetCache.Remove(assetBundleName);
            bundleInfo.Unload(unloadAllLoadedObjects);
        }

        public static async ETTask LoadBundleAsync(string assetBundleName) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            var deps = GetSortedDependencies(assetBundleName);
            using (var list = ListComponent<BundleInfo>.Create()) {
                async ETTask LoadDependency(string dependency, List<BundleInfo> bundleInfos) {
                    using var coroutineLock = await TaskLock.Wait(TaskLockType.Resources, dependency.GetHashCode());
                    var bundleInfo = await LoadOneBundleAsync(dependency);
                    if (bundleInfo == null || bundleInfo.refCount > 1) {
                        return;
                    }

                    bundleInfos.Add(bundleInfo);
                }

                async ETTask LoadAllAssetsInBundle(BundleInfo bundleInfo) {
                    using var locker = await TaskLock.Wait(TaskLockType.Resources, bundleInfo.name.GetHashCode());

                    if (bundleInfo.alreadyLoadAssets) {
                        return;
                    }

                    if (bundleInfo.assetBundle != null && !bundleInfo.assetBundle.isStreamedSceneAssetBundle) {
                        // 异步load资源到内存cache住
                        var request = bundleInfo.assetBundle.LoadAllAssetsAsync();
                        await request;
                        var assets = request.allAssets;

                        foreach (var asset in assets) {
                            CacheAsset(bundleInfo.name, asset.name, asset);
                        }
                    }

                    bundleInfo.alreadyLoadAssets = true;
                }

                // LoadFromFileAsync部分可以并发加载
                using (var tasks = ListComponent<ETTask>.Create()) {
                    foreach (var dependency in deps) {
                        tasks.Add(LoadDependency(dependency, list));
                    }

                    await ETTaskHelper.WaitAll(tasks);

                    // ab包从硬盘加载完成，可以再并发加载all assets
                    tasks.Clear();
                    foreach (var bundleInfo in list) {
                        tasks.Add(LoadAllAssetsInBundle(bundleInfo));
                    }

                    await ETTaskHelper.WaitAll(tasks);
                }
            }
        }

        private static async ETTask<BundleInfo> LoadOneBundleAsync(string assetBundleName) {
            // assetBundleName = BundleNameToLower(assetBundleName);
            if (Instance._bundleCache.TryGetValue(assetBundleName, out var bundleInfo)) {
                ++bundleInfo.refCount;
                return null;
            }

            if (Instance.UseEditor) {
                var realPaths = AssetBundleHelper.GetAssetPathsFromAssetBundle(assetBundleName);
                foreach (var realPath in realPaths) {
                    var assetName = Path.GetFileNameWithoutExtension(realPath);
                    var resource = AssetBundleHelper.LoadAssetAtPath(realPath);
                    CacheAsset(assetBundleName, assetName, resource);
                }

                if (realPaths.Length > 0) {
                    bundleInfo = new BundleInfo(assetBundleName, null);
                    Instance._bundleCache[assetBundleName] = bundleInfo;
                }
                else {
                    Log.Error("Bundle not exist! BundleName: " + assetBundleName);
                }

                await Timer.WaitFrame();

                return bundleInfo;
            }

            var path = Path.Combine(UnityHelper.Path.AppHotfixResPath, assetBundleName);
            if (!File.Exists(path)) {
                path = Path.Combine(UnityHelper.Path.AppResPath, assetBundleName);
            }

            Log.Debug("Async load bundle BundleName : " + path);

            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
            await assetBundleCreateRequest;
            var assetBundle = assetBundleCreateRequest.assetBundle;
            if (assetBundle == null) {
                // 获取资源的时候会抛异常，这个地方不直接抛异常，因为有些地方需要Load之后判断是否Load成功
                Log.Warning($"assets bundle not found: {assetBundleName}");
                return null;
            }

            // 缓存包
            bundleInfo = new BundleInfo(assetBundleName, assetBundle);
            Instance._bundleCache[assetBundleName] = bundleInfo;
            return bundleInfo;
        }

        private static string BundleNameToLower1(string name) {
            if (string.IsNullOrEmpty(name)) return name;
            if (Instance._bundleNameToLowerCache.TryGetValue(name, out var result)) return result;
            result = name.ToLower();
            Instance._bundleNameToLowerCache[name] = result;
            return result;
        }
        
        private class BundleInfo {
            public string name;
            public int refCount;
            public AssetBundle assetBundle;
            public bool alreadyLoadAssets;

            public BundleInfo(string name, AssetBundle assetBundle) {
                this.name = name;
                this.assetBundle = assetBundle;
                this.refCount = 1;
                this.alreadyLoadAssets = false;
            }

            public void Unload(bool unloadAllLoadedObjects = true) {
                this.name = null;
                this.refCount = 0;
                this.alreadyLoadAssets = false;
                if (this.assetBundle != null) this.assetBundle.Unload(unloadAllLoadedObjects);
                this.assetBundle = null;
            }
        }
    }

    public static class AssetBundleHelper {
        public static string ToBundleName(this string self) {
            if (ResourcesManager.Instance.stringToBundleCache.TryGetValue(self, out var result)) {
                return result;
            }

            result = (self + ".unity3d").ToLower();
            ResourcesManager.Instance.stringToBundleCache[self] = result;
            return result;
        }
        
        public static UnityEngine.Object LoadAssetAtPath(string s) {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
#else
			return null;
#endif
        }

        public static string[] GetAssetPathsFromAssetBundle(string assetBundleName) {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
#else
			return new string[0];
#endif
        }

        public static string[] GetAssetBundleDependencies(string assetBundleName, bool v) {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetBundleDependencies(assetBundleName, v);
#else
			return new string[0];
#endif
        }
    }
}