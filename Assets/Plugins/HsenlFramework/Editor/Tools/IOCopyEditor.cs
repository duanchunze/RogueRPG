using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public class IOCopyEditor : EditorWindow {
        private string srcPath;
        private string dstPath;

        private readonly List<DirectoryCopyInfo> _directoryCopyInfos = new();

        [MenuItem("Hsenl/Tools/拷贝助手")]
        private static void Create() {
            var window = CreateWindow<IOCopyEditor>();
            window.position = new Rect(100, 100, 900, 300);
            window.Show();
        }

        private void OnEnable() {
            var len = EditorPrefs.GetInt("dir_copy_infos_length");
            for (int i = 0; i < len; i++) {
                var info = new DirectoryCopyInfo();
                info._srcDir = EditorPrefs.GetString("src_dir_copy" + i);
                info._dstDir = EditorPrefs.GetString("dst_dir_copy" + i);
                info._clearDstDir = EditorPrefs.GetBool("clear_dst_dir" + i);
                this._directoryCopyInfos.Add(info);
            }
        }

        private void OnGUI() {
            for (int i = 0, len = this._directoryCopyInfos.Count; i < len; i++) {
                var dirCopyInfo = this._directoryCopyInfos[i];
                EditorGUILayout.BeginHorizontal();
                dirCopyInfo._srcDir = EditorGUILayout.TextField("源目录", dirCopyInfo._srcDir);
                if (GUILayout.Button("选择目录", GUILayout.Width(70))) {
                    dirCopyInfo._srcDir = EditorUtility.OpenFolderPanel("选择源目录", dirCopyInfo._srcDir, "");
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                dirCopyInfo._dstDir = EditorGUILayout.TextField("目标目录", dirCopyInfo._dstDir);
                if (GUILayout.Button("选择目录", GUILayout.Width(70))) {
                    dirCopyInfo._dstDir = EditorUtility.OpenFolderPanel("选择目标目录", dirCopyInfo._dstDir, "");
                }

                EditorGUILayout.EndHorizontal();
                dirCopyInfo._clearDstDir = EditorGUILayout.Toggle("清除目标目录", dirCopyInfo._clearDstDir);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(30))) {
                this._directoryCopyInfos.RemoveAt(this._directoryCopyInfos.Count - 1);
            }

            if (GUILayout.Button("+", GUILayout.Width(30))) {
                this._directoryCopyInfos.Add(new DirectoryCopyInfo());
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("拷贝目录")) {
                foreach (var copyInfo in this._directoryCopyInfos) {
                    if (!Directory.Exists(copyInfo._srcDir)) {
                        throw new Exception($"源目录{copyInfo._srcDir}不存在!");
                    }

                    var srcDirInfo = new DirectoryInfo(copyInfo._srcDir);
                    var dstDirInfo = new DirectoryInfo(copyInfo._dstDir);

                    if (!dstDirInfo.Exists) {
                        dstDirInfo.Create();
                    }

                    // 删除同名文件夹
                    foreach (var sub in dstDirInfo.GetDirectories()) {
                        if (sub.Name == srcDirInfo.Name) {
                            sub.Delete(true);
                        }
                    }

                    // 创建一个新的同名文件夹爱
                    var dstDir = copyInfo._dstDir + "/" + srcDirInfo.Name;
                    Directory.CreateDirectory(dstDir);

                    // 开始拷贝
                    FileHelper.CopyDirectory(copyInfo._srcDir, dstDir);

                    Debug.Log($"Copy Dir Succ: {dstDir}");
                }
            }
        }

        private void OnDisable() {
            this.UpdateEditorPrefs();
        }

        private void UpdateEditorPrefs() {
            EditorPrefs.SetInt("dir_copy_infos_length", this._directoryCopyInfos.Count);
            for (int i = 0, len = this._directoryCopyInfos.Count; i < len; i++) {
                EditorPrefs.SetString("src_dir_copy" + i, this._directoryCopyInfos[i]._srcDir);
                EditorPrefs.SetString("dst_dir_copy" + i, this._directoryCopyInfos[i]._dstDir);
                EditorPrefs.SetBool("clear_dst_dir" + i, this._directoryCopyInfos[i]._clearDstDir);
            }
        }

        private class DirectoryCopyInfo {
            public string _srcDir;
            public string _dstDir;
            public bool _clearDstDir;
        }
    }
}