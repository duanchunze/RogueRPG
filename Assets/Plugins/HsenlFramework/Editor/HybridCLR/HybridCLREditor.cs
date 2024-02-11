using System.IO;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public static class HybridCLREditor {
        [MenuItem("ET/ActiveBuildTarget_CopyAotDlls")]
        public static void ActiveBuildTargetAndCopyAotDll() {
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            CopyAotDll();
        }

        // [MenuItem("ET/CopyAotDlls")]
        public static void CopyAotDll() {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string fromDir = Path.Combine(HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir, target.ToString());
            string entryDir = "Assets/Bundles/AotDllEntry";
            string aotDllsDir = "Assets/Bundles/AotDlls";

            if (!Directory.Exists(entryDir)) {
                Directory.CreateDirectory(entryDir);
            }

            FileHelper.CleanDirectory(entryDir);

            if (!Directory.Exists(aotDllsDir)) {
                Directory.CreateDirectory(aotDllsDir);
            }

            FileHelper.CleanDirectory(aotDllsDir);

            foreach (var definition in HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions) {
                var sourceFileName = Path.Combine(fromDir, $"{definition.name}.dll");
                // 入口程序集单独放一个包
                var destFileName = Path.Combine(definition.name == "HsenlFramework.Entry" ? entryDir : aotDllsDir, $"{definition.name}.dll.bytes");
                File.Copy(sourceFileName, destFileName, true);
                Debug.Log($"{sourceFileName} -> {destFileName}");
            }

            Debug.Log($"CopyAotDll Finish!");

            AssetDatabase.Refresh();
        }
    }
}