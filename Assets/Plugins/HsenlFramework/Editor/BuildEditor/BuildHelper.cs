using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public static class BuildHelper {
        private const string ExeRelativeDirPrefix = "./Release/Build";

        public const string AssetBundleBuildFolder = "./Release/{0}/StreamingAssets/";

        public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions, bool isBuildExe,
            bool isContainsAB, bool clearFolder) {
            var buildTarget = BuildTarget.StandaloneWindows;
            var programName = Application.productName;
            var exeName = programName;
            switch (type) {
                case PlatformType.PC:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    exeName += ".exe";
                    break;
                case PlatformType.Android:
                    buildTarget = BuildTarget.Android;
                    exeName += ".apk";
                    break;
                case PlatformType.IOS:
                    buildTarget = BuildTarget.iOS;
                    break;
                case PlatformType.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    break;
            }

            var bundleFold = string.Format(AssetBundleBuildFolder, type);

            if (clearFolder && Directory.Exists(bundleFold)) {
                Directory.Delete(bundleFold, true);
            }

            Directory.CreateDirectory(bundleFold);

            UnityEngine.Debug.Log("start build asset bundle");
            BuildPipeline.BuildAssetBundles(bundleFold, buildAssetBundleOptions, buildTarget);

            UnityEngine.Debug.Log("finish build asset bundle");

            if (isContainsAB) {
                FileHelper.CleanDirectory("Assets/StreamingAssets/");
                FileHelper.CopyDirectory(bundleFold, "Assets/StreamingAssets/");
            }

            if (isBuildExe) {
                string[] levels = {
                    "Assets/Scenes/Launch.unity",
                };
                // var levels = EditorBuildSettings.scenes.Select(settingsScene => settingsScene.path).ToArray();
                UnityEngine.Debug.Log("start build exe");
                BuildPipeline.BuildPlayer(levels, $"{ExeRelativeDirPrefix}/{exeName}", buildTarget, buildOptions);
                UnityEngine.Debug.Log("finish build exe");
            }
            else {
                if (isContainsAB && type == PlatformType.PC) {
                    var targetPath = Path.Combine(ExeRelativeDirPrefix, $"{programName}_Data/StreamingAssets/");
                    FileHelper.CleanDirectory(targetPath);
                    Debug.Log($"src dir: {bundleFold}    target: {targetPath}");
                    FileHelper.CopyDirectory(bundleFold, targetPath);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}