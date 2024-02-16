using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    public class Launch : MonoBehaviour {
        private IEnumerator Start() {
            yield return null;

            if (YooAssets.Initialized)
                YooAssets.Destroy();
            YooAssets.Initialize();

            var playMode = EPlayMode.EditorSimulateMode;
            var packageName = "EntryPackage";
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
                        Debug.Log($"{downloadCount}/{totalCount} {currentSizeMB}MB/{totalSizeMB}MB");
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

            Debug.Log("Entry Package Update Done!");

            // 开始使用包
            string assemblyName = "Entry";
            string typeName = "Entry";
            string methodName = "Start";
            Assembly assembly;
#if UNITY_EDITOR
            assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName);
#else
            var entryDllHandle = YooAssets.LoadAssetSync<TextAsset>($"{assemblyName}.dll");
            var entryPdbHandle = YooAssets.LoadAssetSync<TextAsset>($"{assemblyName}.pdb");
            if (entryDllHandle.AssetObject == null) throw new Exception($"Asset '{assemblyName}.dll' can not find from entry package");
            byte[] entryDllBytes = ((TextAsset)entryDllHandle.AssetObject).bytes;
            byte[] entryPdbBytes = ((TextAsset)entryPdbHandle.AssetObject).bytes;
            assembly = Assembly.Load(entryDllBytes, entryPdbBytes);
#endif

            if (assembly == null) throw new Exception($"Assembly '{assemblyName}' cannot be found!");
            // 找到入口类
            var entryType = assembly.GetTypes().FirstOrDefault(type => type.Name == typeName);
            if (entryType == null) throw new Exception($"Type '{typeName}' cannot be found from assembly '{assemblyName}'!");
            // 找到入口函数
            var startMethod = entryType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (startMethod == null) throw new Exception($"Method '{methodName}' cannot be found from type '{typeName}'!");

            // 执行入口函数
            Debug.Log($"Launch Success!");
            startMethod.Invoke(null, null);
        }
    }
}