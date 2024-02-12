using System;
using System.IO;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public static class HybridCLRCompileEditor {
        [MenuItem("ET/Compile And Copy Dlls")]
        private static void ActiveBuildTargetAndCopyAotDll() {
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            CopyAotDll();
            CopyMetadataAotDll();
        }

        private static void CopyAotDll() {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string fromDir = Path.Combine(HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir, target.ToString());
            string codeEntryDir = "Assets/Bundles/DllEntry";
            string codeDir = "Assets/Bundles/Dlls";

            if (!Directory.Exists(codeEntryDir)) {
                Directory.CreateDirectory(codeEntryDir);
            }

            FileHelper.CleanDirectory(codeEntryDir);

            if (!Directory.Exists(codeDir)) {
                Directory.CreateDirectory(codeDir);
            }

            FileHelper.CleanDirectory(codeDir);

            foreach (var definition in HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions) {
                var sourceDllPath = Path.Combine(fromDir, $"{definition.name}.dll");
                var sourcePdbPath = Path.Combine(fromDir, $"{definition.name}.pdb");
                // 入口程序集单独放一个包
                var destDllPath = Path.Combine(definition.name == "HsenlFramework.Entry" ? codeEntryDir : codeDir, $"{definition.name}.dll.bytes");
                var destPdbPath = Path.Combine(definition.name == "HsenlFramework.Entry" ? codeEntryDir : codeDir, $"{definition.name}.pdb.bytes");
                try {
                    File.Copy(sourceDllPath, destDllPath, true);
                }
                catch (Exception e) {
                    Debug.LogError($"CopyAotDll Error -> {e}");
                    return;
                }

                try {
                    File.Copy(sourcePdbPath, destPdbPath, true);
                }
                catch (Exception e) {
                    Debug.LogError($"CopyAotPdb Error -> {e}");
                    return;
                }
            }

            Debug.Log($"CopyAotDll Finish!");

            AssetDatabase.Refresh();
        }

        private static void CopyMetadataAotDll() {
            // 之所以明文写在这, 因为写这里怎么都丢不了, 且这东西也不经常改
            string[] _metadataDlls = {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll",
                "System.Runtime.CompilerServices.Unsafe.dll",
            };

            string _destDir = "Assets/Bundles/DllMetadatas";

            if (Directory.Exists(_destDir)) {
                Directory.Delete(_destDir, true);
            }

            Directory.CreateDirectory(_destDir);

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var sourceDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, buildTarget.ToString());
            foreach (var dll in _metadataDlls) {
                var sourcePath = Path.Combine(sourceDir, dll);
                var destPath = Path.Combine(_destDir, $"{dll}.bytes");
                try {
                    File.Copy(sourcePath, destPath, true);
                    Debug.Log($"CopyMetadataDll2Bundle Success: '{destPath}'");
                }
                catch (Exception) {
                    Debug.LogError($"Could not find file: {sourcePath}");
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}