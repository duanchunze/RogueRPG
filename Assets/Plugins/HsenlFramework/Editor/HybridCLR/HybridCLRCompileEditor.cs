using System;
using System.IO;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public static class HybridCLRCompileEditor {
        private const string _codeEntryDllDir = "Assets/Bundles/CodeEntry";
        private const string _codeDllDir = "Assets/Bundles/CodeMain";
        private const string _entryAssemblyName = "Entry";

        private static readonly string[] _metadataDlls = {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
        };

        private const string _metadataDllDir = "Assets/Bundles/MetadataDlls";

        [MenuItem("Hsenl/Compile And Copy Dlls %e")]
        private static void ActiveBuildTargetAndCopyAotDll() {
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            CopyCodeDlls();
            CopyMetadataDlls();
        }

        private static void CopyCodeDlls() {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string fromDir = Path.Combine(HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir, target.ToString());

            if (!Directory.Exists(_codeEntryDllDir)) {
                Directory.CreateDirectory(_codeEntryDllDir);
            }

            FileHelper.CleanDirectory(_codeEntryDllDir);

            if (!Directory.Exists(_codeDllDir)) {
                Directory.CreateDirectory(_codeDllDir);
            }

            FileHelper.CleanDirectory(_codeDllDir);

            foreach (var definition in HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions) {
                var sourceDllPath = Path.Combine(fromDir, $"{definition.name}.dll");
                var sourcePdbPath = Path.Combine(fromDir, $"{definition.name}.pdb");
                // 入口程序集单独放一个包
                var destDllPath = Path.Combine(definition.name == _entryAssemblyName ? _codeEntryDllDir : _codeDllDir, $"{definition.name}.dll.bytes");
                var destPdbPath = Path.Combine(definition.name == _entryAssemblyName ? _codeEntryDllDir : _codeDllDir, $"{definition.name}.pdb.bytes");
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
                
                Debug.Log($"Copy Dll To Bundle Succ: {destDllPath}");
            }

            Debug.Log($"Copy All Dlls Finish!");

            AssetDatabase.Refresh();
        }

        private static void CopyMetadataDlls() {
            if (Directory.Exists(_metadataDllDir)) {
                Directory.Delete(_metadataDllDir, true);
            }

            Directory.CreateDirectory(_metadataDllDir);

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var sourceDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, buildTarget.ToString());
            foreach (var dll in _metadataDlls) {
                var sourcePath = Path.Combine(sourceDir, dll);
                var destPath = Path.Combine(_metadataDllDir, $"{dll}.bytes");
                try {
                    File.Copy(sourcePath, destPath, true);
                    Debug.Log($"Copy MetadataDll To Bundle Succ: '{destPath}'");
                }
                catch (Exception) {
                    Debug.LogError($"Could not find file: {sourcePath}");
                }
            }
            
            Debug.Log($"Copy All MetadataDlls Finish!");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}