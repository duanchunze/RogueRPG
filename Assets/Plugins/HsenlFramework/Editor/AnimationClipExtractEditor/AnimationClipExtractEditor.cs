using UnityEngine;
using UnityEditor;
using System.IO;

namespace Hsenl {
    public static class AnimationClipExtractEditor {
        [MenuItem("ET/从FBX中提取动画片段")]
        private static void CopyClip() {
            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered)) {
                // 这里需要用这个 mode，不然拷贝的动画片段无法使用
                if (obj is not GameObject go) {
                    continue;
                }

                // if (!go.name.Contains("@")) {
                //     Debug.LogError($"'{go.name}' missing '@' sign");
                //     continue;
                // }

                var clipPath = AnimationsPath(go);
                var clipNewName = go.name.Replace("@", "_");

                if (!Directory.Exists(clipPath))
                    Directory.CreateDirectory(clipPath);

                var srcClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GetAssetPath(go));
                if (srcClip == null) {
                    continue;
                }

                var newClip = new AnimationClip();
                EditorUtility.CopySerialized(srcClip, newClip);
                AssetDatabase.CreateAsset(newClip, clipPath + "/" + clipNewName + ".anim");
                Debug.Log($"copy animation clip '{clipNewName}' success, path is :{clipPath + "/" + clipNewName + ".anim"}");
            }

            AssetDatabase.Refresh();
            Debug.Log("提取完毕！");
        }

        // Returns the path to the directory that holds the specified FBX.
        private static string CharacterRoot(UnityEngine.Object character) {
            var root = AssetDatabase.GetAssetPath(character);
            return root.Substring(0, root.LastIndexOf('/') + 1);
        }

        // Returns the path to the directory that holds materials generated
        // for the specified FBX.
        private static string AnimationsPath(UnityEngine.Object character) {
            return CharacterRoot(character) + "AnimationClip";
        }
    }
}