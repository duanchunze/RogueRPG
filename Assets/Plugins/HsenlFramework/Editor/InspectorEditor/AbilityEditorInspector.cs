// using System;
// using System.ComponentModel;
// using System.IO;
// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace Hsenl {
//     [CustomEditor(typeof(UnityAbility))]
//     public class AbilityEditorInspector : OdinEditor {
//         private UnityAbility _t;
//
//         protected override void OnEnable() {
//             this._t = (UnityAbility)this.target;
//         }
//
//         public override void OnInspectorGUI() {
//             base.OnInspectorGUI();
//             var go = this._t.gameObject;
//             EditorGUILayout.BeginHorizontal();
//
//             if (PrefabUtility.IsPartOfPrefabAsset(go)) {
//                 if (GUILayout.Button("保存数据为二进制文件")) {
//                     var path = AssetDatabase.GetAssetPath(go);
//                     path = path[..path.LastIndexOf('/')];
//                     path += $"/binary{go.name}.bytes";
//                     var data = SerializeHelper.SerializeOdin(this._t.Component);
//                     using (var fs = new FileStream(path, FileMode.OpenOrCreate)) {
//                         fs.Seek(0, SeekOrigin.Begin);
//                         fs.Write(data);
//                         AssetDatabase.Refresh();
//                     }
//                 }
//             }
//
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.BeginHorizontal();
//
//             if (PrefabUtility.IsPartOfPrefabAsset(go)) {
//                 if (GUILayout.Button("从二进制文件加载数据")) {
//                     var path = AssetDatabase.GetAssetPath(go);
//                     path = path[..path.LastIndexOf('/')];
//                     path += $"/binary{go.name}.bytes";
//                     var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
//                     if (textAsset == null) {
//                         Debug.LogError($"读二进制文件失败 {path}");
//                         return;
//                     }
//
//                     var obj = SerializeHelper.DeserializeOdin<object>(textAsset.bytes);
//                     if (obj is ISupportInitialize supportInitialize) {
//                         try {
//                             supportInitialize.EndInit();
//                         }
//                         catch (Exception e) {
//                             Log.Error(e);
//                         }
//                     }
//                     var field = this._t.GetType().GetFieldInBase("component", AssemblyHelper.BindingFlagsInstanceIgnorePublic);
//                     field?.SetValue(this._t, obj);
//                 }
//             }
//
//             EditorGUILayout.EndHorizontal();
//         }
//     }
// }