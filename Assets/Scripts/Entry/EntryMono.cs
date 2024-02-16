using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    // 加载主体所有脚本, 加载Framework场景
    public class EntryMono : MonoBehaviour {
        private const string _currentPackageName = "Entry";
        private ResourcePackage _currentPackage;

        private IEnumerator Start() {
            yield return null;
            this._currentPackage = YooAssets.TryGetPackage(_currentPackageName);

            var playMode = EPlayMode.EditorSimulateMode;
            var packageName = "MainPackage";
            var buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline.ToString();

            var package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);

            // 初始化包
            InitializationOperation initializationOperation;
            switch (playMode) {
                case EPlayMode.EditorSimulateMode:
                    var createParams = new EditorSimulateModeParameters();
                    createParams.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
                    initializationOperation = package.InitializeAsync(createParams);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return initializationOperation;
            if (initializationOperation.Status != EOperationStatus.Succeed) {
                Debug.LogError($"{initializationOperation.Error}");
                yield break;
            }

            Debug.Log($"Init package succ! Current version is '{initializationOperation.PackageVersion}'");

            Debug.Log($"Update version start...");

            // 更新版本
            var updatePackageVersionOperation = package.UpdatePackageVersionAsync();
            yield return updatePackageVersionOperation;
            if (updatePackageVersionOperation.Status != EOperationStatus.Succeed) {
                Debug.LogError($"{updatePackageVersionOperation.Error}");
                yield break;
            }

            Debug.Log($"Update version succ! Lastest version is '{updatePackageVersionOperation.PackageVersion}'");

            Debug.Log("Update package manifest start...");

            // 更新资源清单
            var updatePackageManifestOpeartion = package.UpdatePackageManifestAsync(updatePackageVersionOperation.PackageVersion, true);
            yield return updatePackageManifestOpeartion;
            if (updatePackageManifestOpeartion.Status != EOperationStatus.Succeed) {
                Debug.LogError($"{updatePackageManifestOpeartion.Error}");
                yield break;
            }

            Debug.Log("Update package manifest succ!");

            // 创建补丁下载器
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (downloader.TotalDownloadCount != 0) {
                // 获取需要下载的信息
                {
                    var sizeMB = downloader.TotalDownloadBytes / 1048576f;
                    sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
                    var totalSizeMB = sizeMB.ToString("f1");
                    Debug.Log($"Found update patch files, Total count {downloader.TotalDownloadCount} Total szie {totalSizeMB}MB\"");
                }

                yield return new WaitForSeconds(0.5f);

                // 注册下载事件的回调
                {
                    downloader.OnDownloadErrorCallback += (fileName, error) => { Debug.LogError($"File '{fileName}' download fail: {error}"); };

                    downloader.OnDownloadProgressCallback += (totalCount, downloadCount, totalBytes, downloadBytes) => {
                        var ratio = (float)downloadCount / totalCount;
                        string currentSizeMB = (downloadBytes / 1048576f).ToString("f1");
                        string totalSizeMB = (totalBytes / 1048576f).ToString("f1");
                        UnityEngine.Debug.Log($"{downloadCount}/{totalCount} {currentSizeMB}MB/{totalSizeMB}MB");
                    };
                }

                // 开始下载
                Debug.Log("Download start...");
                downloader.BeginDownload();
                yield return downloader;

                if (downloader.Status != EOperationStatus.Succeed) {
                    Debug.LogError("Download Fail!");
                    yield break;
                }

                // 清理未使用的缓存文件
                Debug.Log("Download Succ!");

                Debug.Log("Clear unused cache files start...");

                var clearUnusedCacheFilesOperation = package.ClearUnusedCacheFilesAsync();
                yield return clearUnusedCacheFilesOperation;
                if (downloader.Status != EOperationStatus.Succeed) {
                    Debug.LogError("Clear unused cache files Fail!");
                    yield break;
                }

                Debug.Log("Clear unused cache files succ!");
            }
            else {
                Debug.Log("Not found any download files !");
            }

            Debug.Log("Main Package Update Done!");

#if UNITY_EDITOR
#else
            var codeAssetInfos = package.GetAssetInfos("code");
            var assemblyNames = codeAssetInfos.Where(x => x.AssetPath.EndsWith(".dll.bytes")).Select(x => {
                var fileName = Path.GetFileNameWithoutExtension(x.AssetPath);
                return fileName.Replace(".dll", "");
            });

            foreach (var assemblyName in assemblyNames) {
                var entryDllHandle = YooAssets.LoadAssetSync<TextAsset>($"{assemblyName}.dll");
                var entryPdbHandle = YooAssets.LoadAssetSync<TextAsset>($"{assemblyName}.pdb");
                if (entryDllHandle.AssetObject == null) throw new Exception($"Asset '{assemblyName}.dll' can not find from entry package");
                byte[] entryDllBytes = ((TextAsset)entryDllHandle.AssetObject).bytes;
                byte[] entryPdbBytes = ((TextAsset)entryPdbHandle.AssetObject).bytes;
                Assembly.Load(entryDllBytes, entryPdbBytes);
            }

            Debug.Log("Load All Main Assemblies Succ!");
#endif

            YooAssets.DestroyPackage(_currentPackageName);
            Debug.Log("Entry Package Destroy!");
            YooAssets.LoadSceneAsync("Framework");
        }

        private void OnDestroy() {
            YooAssets.DestroyPackage(_currentPackageName);
        }
    }
}