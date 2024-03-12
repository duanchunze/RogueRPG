using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YooAsset;

namespace Hsenl {
    // 加载主体所有脚本, 加载Framework场景
    public class EntryMono : MonoBehaviour {
        private const string _currentPackageName = "Entry";

        public EPlayMode playMode;

        public UIPatch uiPatch;

        [Header("依赖顺序不能错")]
        public List<string> dllAssetNames = new() {
            "HsenlFramework.ThirdParty.dll",
            "HsenlFramework.Runtime.dll",
            "Hsenl.Network.Common.dll",
            "Hsenl.Network.Client.dll",
            "Hsenl.Network.Server.dll",
            "GameModel.dll",
            "GameHotReload.dll",
            "GameView.dll",
        };

        private IEnumerator Start() {
            yield return null;
            YooAssets.TryGetPackage(_currentPackageName);

            var packageName = "MainPackage";
            var buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline.ToString();

            var package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);

            // 初始化包
            InitializationOperation initializationOperation;
            switch (this.playMode) {
                case EPlayMode.EditorSimulateMode: {
                    var createParams = new EditorSimulateModeParameters {
                        SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName)
                    };
                    initializationOperation = package.InitializeAsync(createParams);
                    break;
                }
                case EPlayMode.OfflinePlayMode: {
                    var createParameters = new OfflinePlayModeParameters {
                        DecryptionServices = new FileStreamDecryption()
                    };
                    initializationOperation = package.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.HostPlayMode: {
                    string defaultHostServer = YooAssetsHelper.GetHostServerURL(packageName);
                    string fallbackHostServer = YooAssetsHelper.GetHostServerURL(packageName);
                    var createParameters = new HostPlayModeParameters {
                        DecryptionServices = new FileStreamDecryption(),
                        BuildinQueryServices = new GameQueryServices(),
                        RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer)
                    };
                    initializationOperation = package.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.WebPlayMode: {
                    string defaultHostServer = YooAssetsHelper.GetHostServerURL(packageName);
                    string fallbackHostServer = YooAssetsHelper.GetHostServerURL(packageName);
                    var createParameters = new WebPlayModeParameters {
                        DecryptionServices = new FileStreamDecryption(),
                        BuildinQueryServices = new GameQueryServices(),
                        RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer)
                    };
                    initializationOperation = package.InitializeAsync(createParameters);
                    break;
                }
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
            var updatePackageManifestOpeartion = package.UpdatePackageManifestAsync(updatePackageVersionOperation.PackageVersion);
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
                    var message = $"有补丁需要更新, 共{downloader.TotalDownloadCount}个文件, 大小{totalSizeMB}MB\"";
                    var confirm = new ConfirmWrap();
                    yield return this.StartCoroutine(this.WaitConfire(message, confirm));
                    if (!confirm.succ)
                        yield break;
                }

                yield return new WaitForSeconds(0.5f);

                // 注册下载事件的回调
                {
                    downloader.OnDownloadErrorCallback = (fileName, error) => { Debug.LogError($"File '{fileName}' download fail: {error}"); };

                    downloader.OnDownloadProgressCallback = (totalCount, downloadCount, totalBytes, downloadBytes) => {
                        var ratio = (float)downloadCount / totalCount;
                        string currentSizeMB = (downloadBytes / 1048576f).ToString("f1");
                        string totalSizeMB = (totalBytes / 1048576f).ToString("f1");
                        this.uiPatch.progressSlider.value = ratio;
                        this.uiPatch.sliderTipsText.text = $"{downloadCount}/{totalCount} {currentSizeMB}MB/{totalSizeMB}MB";
                    };
                }

                // 开始下载
                DOWNLOAD:
                this.uiPatch.sliderTipsText.gameObject.SetActive(true);
                Debug.Log("Download start...");
                downloader.BeginDownload();
                yield return downloader;
                this.uiPatch.sliderTipsText.gameObject.SetActive(true);

                if (downloader.Status != EOperationStatus.Succeed) {
                    Debug.LogError("Download Fail!");
                    var confirm = new ConfirmWrap();
                    yield return this.StartCoroutine(this.WaitConfire("下载失败", confirm, "再试一次"));
                    if (!confirm.succ)
                        yield break;
                    else
                        goto DOWNLOAD;
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
            // 补充元数据
            var metadataAssetsInfos = package.GetAssetInfos("metadata");
            foreach (var metadataAssetsInfo in metadataAssetsInfos) {
                var handle = YooAssets.LoadAssetSync<TextAsset>(metadataAssetsInfo.Address);
                byte[] dllBytes = ((TextAsset)handle.AssetObject).bytes;
                RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly: {metadataAssetsInfo.Address}. ret:");
            }

            // 加载dlls
            List<AssetInfo> codeAssetInfos = new();
            foreach (var dll in this.dllAssetNames) {
                var assetInfo = package.GetAssetInfo(dll);
                if (assetInfo == null) continue;
                codeAssetInfos.Add(assetInfo);
            }

            var assemblyNames = codeAssetInfos.Where(x => x.AssetPath.EndsWith(".dll.bytes")).Select(x => {
                var fileName = Path.GetFileNameWithoutExtension(x.AssetPath);
                return fileName.Replace(".dll", "");
            });

            foreach (var assemblyName in assemblyNames) {
                var dllHandle = YooAssets.LoadAssetSync<TextAsset>($"{assemblyName}.dll");
                var pdbHandle = YooAssets.LoadAssetSync<TextAsset>($"{assemblyName}.pdb");
                if (dllHandle.AssetObject == null) throw new Exception($"Asset '{assemblyName}.dll' can not find from entry package");
                byte[] entryDllBytes = ((TextAsset)dllHandle.AssetObject).bytes;
                byte[] entryPdbBytes = ((TextAsset)pdbHandle.AssetObject).bytes;
                try {
                    Assembly.Load(entryDllBytes, entryPdbBytes);
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }

            Debug.Log("Load All Main Assemblies Succ!");
#endif

            YooAssets.DestroyPackage(_currentPackageName);
            Debug.Log("Entry Package Destroy!");
            YooAssets.LoadSceneAsync("Framework");
        }

        private IEnumerator WaitConfire(string message, ConfirmWrap confirmWrap, string confirmContent = "确认", string cancelContent = "取消") {
            this.uiPatch.messageBox.gameObject.SetActive(true);
            this.uiPatch.messageText.text = message;
            this.uiPatch.confirmButton.GetComponentInChildren<Text>().text = confirmContent;
            this.uiPatch.cancelButton.GetComponentInChildren<Text>().text = cancelContent;
            bool ok = false;
            UnityAction confirmDel = () => {
                ok = true;
                confirmWrap.succ = true;
            };

            UnityAction cancelDel = () => {
                ok = true;
                confirmWrap.succ = false;
            };
            this.uiPatch.confirmButton.onClick.AddListener(confirmDel);
            this.uiPatch.cancelButton.onClick.AddListener(cancelDel);
            while (!ok) {
                yield return null;
            }

            this.uiPatch.confirmButton.onClick.RemoveListener(confirmDel);
            this.uiPatch.cancelButton.onClick.RemoveListener(cancelDel);
            this.uiPatch.messageBox.gameObject.SetActive(false);
        }

        private void OnDestroy() {
            YooAssets.DestroyPackage(_currentPackageName);
        }

        private class ConfirmWrap {
            public bool succ;
        }
    }
}