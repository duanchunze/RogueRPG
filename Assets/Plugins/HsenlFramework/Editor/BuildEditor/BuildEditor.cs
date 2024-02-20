using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Hsenl {
    public enum PlatformType {
        None,
        Android,
        IOS,
        PC,
        MacOS,
    }

    public enum BuildType {
        Development,
        Release,
    }

    public class BuildEditor : EditorWindow {
        private PlatformType _activePlatform;
        private PlatformType _platformType;
        private bool _clearFolder;
        private bool _isBuildExe;
        private bool _isContainAB;
        private CodeOptimization _codeOptimization = CodeOptimization.Debug;
        private BuildOptions _buildOptions;
        private BuildAssetBundleOptions _buildAssetBundleOptions = BuildAssetBundleOptions.None;

        private string _copySourceDirTemp = $"{Application.dataPath}/StreamingAssets";
        private string _copyDestDirTemp = $"{Application.dataPath}/../IL2CPPBuild/RogueRPG_Data/StreamingAssets";

        [MenuItem("Hsenl/Build Tool")]
        public static void ShowWindow() {
            GetWindow<BuildEditor>();
        }

        private void OnEnable() {
#if UNITY_ANDROID
			activePlatform = PlatformType.Android;
#elif UNITY_IOS
			activePlatform = PlatformType.IOS;
#elif UNITY_STANDALONE_WIN
            this._activePlatform = PlatformType.PC;
#elif UNITY_STANDALONE_OSX
			activePlatform = PlatformType.MacOS;
#else
			activePlatform = PlatformType.None;
#endif
            this._platformType = this._activePlatform;

            this._clearFolder = EditorPrefs.GetBool("build_clear_folder", this._clearFolder);
            this._isBuildExe = EditorPrefs.GetBool("build_buildexe", this._isBuildExe);
            this._isContainAB = EditorPrefs.GetBool("build_containbuildstreamming", this._isContainAB);
            this._copySourceDirTemp = EditorPrefs.GetString("_copySourceDirTemp", this._copySourceDirTemp);
            this._copyDestDirTemp = EditorPrefs.GetString("_copyDestDirTemp", this._copyDestDirTemp);
        }

        private void OnGUI() {
            this._platformType = (PlatformType)EditorGUILayout.EnumPopup(this._platformType);
            this._clearFolder = EditorGUILayout.Toggle("Clean Folder? ", this._clearFolder);
            this._isBuildExe = EditorGUILayout.Toggle("Build Exe?", this._isBuildExe);
            this._isContainAB = EditorGUILayout.Toggle("Contain Assets Bundle?", this._isContainAB);
            this._codeOptimization = (CodeOptimization)EditorGUILayout.EnumPopup("CodeOptimization ", this._codeOptimization);
            EditorGUILayout.LabelField("BuildAssetBundleOptions");
            this._buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField(this._buildAssetBundleOptions);

            switch (this._codeOptimization) {
                case CodeOptimization.None:
                case CodeOptimization.Debug:
                    this._buildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
                    break;
                case CodeOptimization.Release:
                    this._buildOptions = BuildOptions.None;
                    break;
            }

            GUILayout.Space(5);

            if (GUILayout.Button("BuildPackage")) {
                if (this._platformType == PlatformType.None) {
                    this.ShowNotification(new GUIContent("please select platform!"));
                    return;
                }

                if (this._platformType != this._activePlatform) {
                    switch (EditorUtility.DisplayDialogComplex("Warning!",
                                $"current platform is {this._activePlatform}, if change to {this._platformType}, may be take a long time",
                                "change",
                                "cancel", "no change")) {
                        case 0:
                            this._activePlatform = this._platformType;
                            break;
                        case 1:
                            return;
                        case 2:
                            this._platformType = this._activePlatform;
                            break;
                    }
                }

                BuildHelper.Build(this._platformType, this._buildAssetBundleOptions, this._buildOptions, this._isBuildExe, this._isContainAB,
                    this._clearFolder);
            }

            GUILayout.Space(5);

            this._copySourceDirTemp = EditorGUILayout.TextField("顺道拷贝: 源目录", this._copySourceDirTemp);
            this._copyDestDirTemp = EditorGUILayout.TextField("顺道拷贝: 目标目录", this._copyDestDirTemp);

            if (!string.IsNullOrEmpty(this._copySourceDirTemp) && !string.IsNullOrEmpty(this._copyDestDirTemp)) {
                if (GUILayout.Button("Copy")) {
                    if (Directory.Exists(this._copySourceDirTemp) && Directory.Exists(this._copyDestDirTemp)) {
                        CopyAll(this._copySourceDirTemp, this._copyDestDirTemp);
                    }
                }
            }

            if (GUI.changed) {
                EditorPrefs.SetBool("build_clear_folder", this._clearFolder);
                EditorPrefs.SetBool("build_buildexe", this._isBuildExe);
                EditorPrefs.SetBool("build_containbuildstreamming", this._isContainAB);
                EditorPrefs.SetString("_copySourceDirTemp", this._copySourceDirTemp);
                EditorPrefs.SetString("_copyDestDirTemp", this._copyDestDirTemp);
            }
        }

        // 递归方法来复制目录及其所有子目录和文件
        public static void CopyAll(string sourcePath, string targetPath) {
            // 创建目标目录（如果不存在）
            Directory.CreateDirectory(targetPath);

            if (sourcePath.EndsWith(".meta")) return;

            // 获取源目录下所有的文件名和子目录名
            foreach (string fileName in Directory.GetFiles(sourcePath)) {
                string destFile = Path.Combine(targetPath, Path.GetFileName(fileName));
                File.Copy(fileName, destFile, true); // 如果目标文件已存在则覆盖
                Debug.Log($"copy succ {destFile}");
            }

            foreach (string directoryName in Directory.GetDirectories(sourcePath)) {
                string destDir = Path.Combine(targetPath, new DirectoryInfo(directoryName).Name);
                CopyAll(directoryName, destDir); // 递归调用自身以处理子目录
            }
        }
    }
}