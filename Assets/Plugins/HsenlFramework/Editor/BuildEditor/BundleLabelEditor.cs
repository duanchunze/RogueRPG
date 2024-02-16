// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using UnityEditor;
// using UnityEngine;
//
// namespace Hsenl {
//     public static class BundleLabelEditor {
//         private const string BundleLabelSettingFilePath = "Assets/Resources/BundleLabelSetting.asset";
//
//         [MenuItem("ET/AssetBundle/创建或选中“包标签设置文件”")]
//         private static void CreateBundleLabelSetting() {
//             var create = false;
//             var asset = AssetDatabase.LoadAssetAtPath<BundleLabelSetting>(BundleLabelSettingFilePath);
//
//             if (asset == null) {
//                 create = true;
//                 asset = ScriptableObject.CreateInstance<BundleLabelSetting>();
//                 AssetDatabase.CreateAsset(asset, BundleLabelSettingFilePath);
//             }
//
//             AssetDatabase.SaveAssets();
//             EditorUtility.FocusProjectWindow();
//             Selection.activeObject = asset;
//
//             if (create) Debug.Log($"《包标签设置文件》创建成功！ -- {BundleLabelSettingFilePath}");
//         }
//
//         [MenuItem("ET/AssetBundle/设置所有资源包的标签并创建资源清单")]
//         private static void SetLabels() {
//             AssetDatabase.RemoveUnusedAssetBundleNames();
//             var setting = Resources.Load<BundleLabelSetting>("BundleLabelSetting");
//             if (setting == null) {
//                 Debug.LogError("需要先创建“包标签设置文件”");
//                 return;
//             }
//
//             AssetsManifest assetsManifest = new();
//
//             var num = 0;
//             foreach (var info in setting.rootPathsMode1) {
//                 var path = info.path;
//                 path = path.Replace("\\", "/");
//                 var withoutPath = path.Split('*')[0];
//                 if (!withoutPath.EndsWith("/")) withoutPath += "/";
//                 path = path.Replace("*", "");
//
//                 // 获得该目录下，所有的文件, 包括子文件夹
//                 var fileInfos = Directory.CreateDirectory(path).GetAllFileInfos(info.includeSub);
//                 foreach (var fileInfo in fileInfos) {
//                     if (fileInfo.Name.EndsWith(".meta")) {
//                         continue;
//                     }
//
//                     if (info.includeExtensions != null && info.includeExtensions.Count != 0 && !info.includeExtensions.Contains(fileInfo.Extension)) {
//                         continue;
//                     }
//
//                     if (ContainsPath(fileInfo.FullName, info.exclusionPaths)) {
//                         continue;
//                     }
//
//                     var assetName = Path.GetFileNameWithoutExtension(fileInfo.Name); // 获得不带后缀的资源名
//                     var relativePath = "Assets/" + Path.GetRelativePath(Application.dataPath, fileInfo.FullName).Replace("\\", "/"); // 获得完整的unity相对路径
//                     var uniquePath = relativePath.Replace(fileInfo.Extension, ""); // 获得不带后缀的相对unity的路径
//                     
//                     var bundleName = uniquePath;
//                     bundleName = bundleName.Replace(withoutPath, ""); // 剔除不包含的内容
//                     bundleName = bundleName.Replace("/", AssetsManifest.AssetBundleSpaceMark); // 替换间隔符
//                     bundleName += ".unity3d"; // 添加后缀名
//
//                     var assetImporter = AssetImporter.GetAtPath(relativePath);
//                     assetImporter.assetBundleName = bundleName;
//                     assetsManifest.Register(uniquePath, bundleName, assetName);
//
//                     Debug.Log($"设置标签成功！Mode1 唯一路径：{uniquePath} 包名：{bundleName} 资源名：{assetName}");
//                     num++;
//                 }
//             }
//
//             foreach (var info in setting.rootPathsMode2) {
//                 var path = info.path;
//                 path = path.Replace("\\", "/");
//                 var withoutPath = path.Split('*')[0];
//                 if (!withoutPath.EndsWith("/")) withoutPath += "/";
//                 path = path.Replace("*", "");
//
//                 // var bundleName = path.Replace(withoutPath, "");
//                 // bundleName = bundleName.Replace("/", AssetsManifest.AssetBundleSpaceMark);
//                 // bundleName += ".unity3d";
//
//                 var fileInfos = Directory.CreateDirectory(path).GetAllFileInfos(info.includeSub);
//                 foreach (var fileInfo in fileInfos) {
//                     if (fileInfo.Name.EndsWith(".meta")) {
//                         continue;
//                     }
//
//                     if (info.includeExtensions != null && info.includeExtensions.Count != 0 && !info.includeExtensions.Contains(fileInfo.Extension)) {
//                         continue;
//                     }
//
//                     if (ContainsPath(fileInfo.FullName, info.exclusionPaths)) {
//                         continue;
//                     }
//
//                     var assetName = Path.GetFileNameWithoutExtension(fileInfo.Name); // 获得不带后缀的资源名
//                     var relativePath = "Assets/" + Path.GetRelativePath(Application.dataPath, fileInfo.FullName).Replace("\\", "/"); // 获得完整的unity相对路径
//                     var uniquePath = relativePath.Replace(fileInfo.Extension, ""); // 获得不带资源名与后缀名的unity相对路径
//                     
//                     var bundleName = relativePath.Replace(fileInfo.Name, "");
//                     if (bundleName.EndsWith("/")) bundleName = bundleName.Remove(bundleName.Length - 1);
//                     bundleName = bundleName.Replace(withoutPath, ""); // 剔除不包含的内容
//                     bundleName = bundleName.Replace("/", AssetsManifest.AssetBundleSpaceMark); // 替换间隔符
//                     bundleName += ".unity3d"; // 添加后缀名
//
//                     var assetImporter = AssetImporter.GetAtPath(relativePath);
//                     assetImporter.assetBundleName = bundleName;
//                     assetsManifest.Register(uniquePath, bundleName, assetName);
//
//                     Debug.Log($"设置标签成功！Mode2 唯一路径：{uniquePath} 包名：{bundleName} 资源名：{assetName}");
//                     num++;
//                 }
//             }
//
//             foreach (var info in setting.rootPathsMode3) {
//                 var path = info.path;
//                 path = path.Replace("\\", "/");
//                 path = path.Replace("*", "");
//
//                 if (File.Exists(path)) {
//                     // 如果该路径是一个资源，那么直接把名字设置好就行了
//                     var assetName = Path.GetFileNameWithoutExtension(path);
//                     var uniquePath = assetName;
//                     var bundleName = assetName + ".unity3d";
//
//                     var assetImporter = AssetImporter.GetAtPath(path);
//                     assetImporter.assetBundleName = bundleName;
//                     assetsManifest.Register(uniquePath, bundleName, assetName);
//
//                     Debug.Log($"设置标签成功！Mode3指定文件 唯一路径：{uniquePath} 包名：{bundleName} 资源名：{assetName}");
//                     num++;
//
//                     continue;
//                 }
//
//                 if (!Directory.Exists(path)) {
//                     Debug.LogError($"Mode3 目录无效，既不是文件目录，也不是文件夹目录：'{path}'");
//                     continue;
//                 }
//
//                 var fileInfos = Directory.CreateDirectory(path).GetAllFileInfos(info.includeSub);
//                 foreach (var fileInfo in fileInfos) {
//                     if (fileInfo.Name.EndsWith(".meta")) {
//                         continue;
//                     }
//
//                     if (info.includeExtensions != null && info.includeExtensions.Count != 0 && !info.includeExtensions.Contains(fileInfo.Extension)) {
//                         continue;
//                     }
//
//                     if (ContainsPath(fileInfo.FullName, info.exclusionPaths)) {
//                         continue;
//                     }
//
//                     var assetName = Path.GetFileNameWithoutExtension(fileInfo.Name);
//                     var relativePath = "Assets/" + Path.GetRelativePath(Application.dataPath, fileInfo.FullName).Replace("\\", "/");
//                     var uniquePath = relativePath.Replace(fileInfo.Extension, "");
//                     var bundleName = assetName + ".unity3d";
//
//                     var assetImporter = AssetImporter.GetAtPath(relativePath);
//                     assetImporter.assetBundleName = bundleName;
//                     assetsManifest.Register(uniquePath, bundleName, assetName);
//
//                     Debug.Log($"设置标签成功！Mode3 唯一路径：{uniquePath} 包名：{bundleName} 资源名：{assetName}");
//                     num++;
//                 }
//             }
//
//             if (assetsManifest.Length > 0) {
//                 var path = Application.dataPath + "/" + setting.assetBundleManifestPath.Replace("Assets/", "");
//                 using (var stream = File.Create(path)) {
//                     SerializeHelper.Serialize(assetsManifest, stream);
//                 }
//
//                 AssetDatabase.Refresh();
//                 Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(setting.assetBundleManifestPath);
//             }
//
//             Debug.Log($"设置完成！共'{num}'个！");
//         }
//
//         private static bool ContainsPath(string path, List<string> contains) {
//             path = path.Replace("\\", "/");
//             foreach (var contain in contains) {
//                 if (!path.Contains(contain)) continue;
//                 return true;
//             }
//
//             return false;
//         }
//
//         private static string CombineBundleName(FileInfo fileInfo, string interval) {
//             Stack<string> pathStack = new();
//             var dir = fileInfo.Directory;
//             while (dir != null && dir.Name != "Assets") {
//                 pathStack.Push(dir.Name);
//                 dir = dir.Parent;
//             }
//
//             StringBuilder sb = new();
//             while (pathStack.Count > 0) {
//                 var str = pathStack.Pop();
//                 if (str.Contains(interval)) {
//                     throw new Exception($"<路径中不能包含间隔符号'{interval}'> '{sb.Append(str)}'");
//                 }
//
//                 sb.Append(str);
//                 if (pathStack.Count != 0) {
//                     sb.Append(interval);
//                 }
//             }
//
//             return sb.ToString();
//         }
//     }
// }