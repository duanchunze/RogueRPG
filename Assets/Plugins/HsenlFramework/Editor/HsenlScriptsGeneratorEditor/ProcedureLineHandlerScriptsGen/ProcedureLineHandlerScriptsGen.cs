using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public class ProcedureLineHandlerScriptsGen : HsenlScriptsGeneratorEditor {
        public List<IHsenlScriptsGenerateScheme> schemes = new();

        public override List<IHsenlScriptsGenerateScheme> GetSchemes() {
            return this.schemes;
        }

        public class ScriptsCreatorEditor : EditorWindow {
            private static EditorWindow _window;

            private string outputRootDir = "Assets/Scripts/GameLogic/ProcedureLine/";
            private string scriptName;

            private List<string> templatePaths = new() {
                "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGenerator/ProcedureLineHandlerScriptsGen/ProcedureLineItemTemplate.txt",
                "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGenerator/ProcedureLineHandlerScriptsGen/ProcedureLineItemPriorityTemplate.txt",
            };

            private List<string> fileNames = new() {
                "Pli{0}Form.cs",
                "Pli{0}Priority.cs",
            };

            [MenuItem("ET/ScriptsGenerator/生成流水线处理代码")]
            private static void Generate() {
                _window = CreateWindow<ScriptsCreatorEditor>();
                _window.position = new Rect(100, 100, 600, 300); // 设置初始尺寸
                _window.Show();
            }

            private void OnGUI() {
                var createOk = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("根目录：", GUILayout.Width(60));
                this.outputRootDir = EditorGUILayout.TextField("", this.outputRootDir, GUILayout.Width(400));
                if (string.IsNullOrEmpty(this.outputRootDir)) {
                    GUILayout.Label("⚠ 目录不能为空 ", new GUIStyle { normal = { textColor = Color.red } });
                    createOk = false;
                }
                else {
                    if (!Directory.Exists(this.outputRootDir)) {
                        GUILayout.Label("⚠ 目录不存在 ", new GUIStyle { normal = { textColor = Color.red } });
                        createOk = false;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("脚本名称：", GUILayout.Width(60));
                this.scriptName = EditorGUILayout.TextField("", this.scriptName, GUILayout.Width(400));
                if (string.IsNullOrEmpty(this.scriptName)) {
                    GUILayout.Label("⚠ 脚本名不能为空 ", new GUIStyle { normal = { textColor = Color.red } });
                    createOk = false;
                }
                else {
                    if (!Regex.Match(this.scriptName, "[A-Z,a-z]").Success) {
                        GUILayout.Label("⚠ 脚本开头必须是字母 ", new GUIStyle { normal = { textColor = Color.red } });
                        createOk = false;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (createOk && GUILayout.Button("创建")) {
                    var dir = this.outputRootDir + this.scriptName;
                    if (Directory.Exists(dir)) {
                        throw new Exception($"该脚本合集已经存在了 {dir}");
                    }

                    Directory.CreateDirectory(dir);

                    var gen = new ProcedureLineHandlerScriptsGen {
                        schemes = new List<IHsenlScriptsGenerateScheme>()
                    };
                    var scheme = new HsenlScriptsGenerateScheme() {
                        universalOutputDirectory = dir,
                        universalTemplateReplaceOrigianl = "#NAME#",
                    };
                    for (int i = 0; i < this.templatePaths.Count; i++) {
                        scheme.AddGenerateInfo(new HsenlScriptsGenerateInfo(this.scriptName, string.Format(this.fileNames[i], this.scriptName)) {
                            templatePath = this.templatePaths[i],
                        });
                    }

                    gen.schemes.Add(scheme);
                    gen.ScriptsGenerate();
                    _window.Close();
                }
            }
        }
    }
}