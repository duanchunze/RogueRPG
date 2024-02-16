// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// namespace Hsenl {
//     public class BundleLabelSetting : ScriptableObject {
//         [Serializable]
//         public class SettingInfo {
//             public string path;
//             public bool includeSub; // 是否包含子文件夹, 不包含的话, 就只处理当前这个路径下的文件
//             public List<string> exclusionPaths; // 排除这些路径, 可以是部分路径, 使用的是Contains判断.
//             public List<string> includeExtensions; // 包括的后缀名. 如果不设置, 则默认包括所有后缀名
//         }
//
//         public string bundleRootDirectory = "Assets/Bundles";
//         public string assetBundleManifestPath = "Assets/Bundles/Config/AssetsManifest.bytes";
//
//         [Header("使用该资源的路径 + 文件名，作为包名")]
//         public List<SettingInfo> rootPathsMode1 = new() {
//             new SettingInfo() {
//                 path = "Assets/Bundles/*Equip",
//                 includeSub = true,
//                 exclusionPaths = null,
//             }
//         };
//
//         [Header("使用该资源的路径，作为包名")]
//         public List<SettingInfo> rootPathsMode2 = new() {
//             new SettingInfo() {
//                 path = "Assets/Bundles/*Input",
//                 includeSub = true,
//                 exclusionPaths = null,
//             },
//             new SettingInfo() {
//                 path = "Assets/Bundles/*UI",
//                 includeSub = true,
//                 exclusionPaths = null,
//             }
//         };
//
//         [Header("使用该资源的文件名，作为包名")]
//         public List<SettingInfo> rootPathsMode3 = new() {
//             new SettingInfo() {
//                 path = "Assets/*Scenes",
//                 includeSub = false,
//                 exclusionPaths = null,
//             }
//         };
//     }
//
//     [CustomEditor(typeof(BundleLabelSetting))]
//     public class BundleLabelSettingEditor : Editor {
//         private BundleLabelSetting _t;
//         private readonly Dictionary<string, string> _bundleNameCache = new();
//
//         private bool _foldout = true;
//
//         private void OnEnable() {
//             this._t = (BundleLabelSetting)this.target;
//         }
//
//         public override void OnInspectorGUI() {
//             base.OnInspectorGUI();
//
//             EditorGUILayout.Space();
//
//             this._foldout = EditorGUILayout.Foldout(this._foldout, "当前所有被设置了包名的文件夹");
//             if (this._foldout) {
//                 EditorGUI.indentLevel++;
//                 var dirs = FileHelper.GetAllDirectory(this._t.bundleRootDirectory);
//                 foreach (var dir in dirs) {
//                     var assetImporter = AssetImporter.GetAtPath(dir);
//                     if (!string.IsNullOrEmpty(assetImporter.assetBundleName)) {
//                         EditorGUILayout.BeginHorizontal();
//
//                         EditorGUILayout.LabelField(dir);
//                         if (!this._bundleNameCache.TryGetValue(dir, out var bundleName)) {
//                             bundleName = assetImporter.assetBundleName;
//                         }
//
//                         this._bundleNameCache[dir] = EditorGUILayout.TextField(bundleName);
//                         if (GUILayout.Button("设置", GUILayout.MaxWidth(30f))) {
//                             assetImporter.assetBundleName = bundleName;
//                             AssetDatabase.SaveAssets();
//                             AssetDatabase.Refresh();
//                         }
//
//                         if (GUIUtility.keyboardControl == 0) {
//                             this._bundleNameCache[dir] = assetImporter.assetBundleName;
//                         }
//
//                         EditorGUILayout.EndHorizontal();
//                     }
//                 }
//
//                 EditorGUI.indentLevel--;
//             }
//         }
//     }
// }