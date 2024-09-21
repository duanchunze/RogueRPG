using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public class ProcedureLineScriptsGen : HsenlScriptsGeneratorEditor {
        public List<IHsenlScriptsGenerateScheme> schemes = new();

        public override List<IHsenlScriptsGenerateScheme> GetSchemes() {
            return this.schemes;
        }

        public class ScriptsCreatorEditor : EditorWindow {
            private static EditorWindow _window;

            private string outputFormRootDir = "Assets/Scripts/GameModel/ProcedureLine/";
            private string outputHandlerRootDir = "Assets/Scripts/GameHotReload/ProcedureLineHandler/";
            private string scriptName;

            private List<string> templatePaths = new() {
                "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGeneratorEditor/ProcedureLineHandlerScriptsGen/ProcedureLineItemTemplate.txt",
                "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGeneratorEditor/ProcedureLineHandlerScriptsGen/ProcedureLineItemPriorityTemplate.txt",
                "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGeneratorEditor/ProcedureLineHandlerScriptsGen/ProcedureLineHandlerTemplate.txt",
            };

            private List<string> fileNames = new() {
                "Pli{0}Form.cs",
                "Pli{0}Priority.cs",
                "Plh{0}.cs",
            };

            [MenuItem("Hsenl/ScriptsGenerator/生成流水线基础代码")]
            private static void Generate() {
                _window = CreateWindow<ScriptsCreatorEditor>();
                _window.position = new Rect(100, 100, 600, 300); // 设置初始尺寸
                _window.Show();
            }

            private void OnGUI() {
                var createOk = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("根目录：", GUILayout.Width(60));
                this.outputFormRootDir = EditorGUILayout.TextField("", this.outputFormRootDir, GUILayout.Width(400));
                if (string.IsNullOrEmpty(this.outputFormRootDir)) {
                    GUILayout.Label("⚠ 目录不能为空 ", new GUIStyle { normal = { textColor = Color.red } });
                    createOk = false;
                }
                else {
                    if (!Directory.Exists(this.outputFormRootDir)) {
                        GUILayout.Label("⚠ 目录不存在 ", new GUIStyle { normal = { textColor = Color.red } });
                        createOk = false;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("根目录：", GUILayout.Width(60));
                this.outputHandlerRootDir = EditorGUILayout.TextField("", this.outputHandlerRootDir, GUILayout.Width(400));
                if (string.IsNullOrEmpty(this.outputHandlerRootDir)) {
                    GUILayout.Label("⚠ 目录不能为空 ", new GUIStyle { normal = { textColor = Color.red } });
                    createOk = false;
                }
                else {
                    if (!Directory.Exists(this.outputHandlerRootDir)) {
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
                    // 创建form
                    var dir = this.outputFormRootDir + this.scriptName;
                    if (Directory.Exists(dir)) {
                        throw new Exception($"该脚本合集已经存在了 {dir}");
                    }

                    Directory.CreateDirectory(dir);

                    var gen = new ProcedureLineScriptsGen {
                        schemes = new List<IHsenlScriptsGenerateScheme>()
                    };

                    for (int i = 0; i < 2; i++) {
                        var scheme = new GenerateScheme_Replace() {
                            templatePath = this.templatePaths[i],
                            outputPath = $"{dir}/{string.Format(this.fileNames[i], this.scriptName)}",
                        };
                        scheme.AddReplace("#NAME#", this.scriptName);
                        gen.schemes.Add(scheme);
                    }

                    // 创建handler
                    dir = this.outputHandlerRootDir + this.scriptName;
                    if (Directory.Exists(dir)) {
                        throw new Exception($"该脚本合集已经存在了 {dir}");
                    }

                    Directory.CreateDirectory(dir);

                    for (int i = 2; i < 3; i++) {
                        var scheme = new GenerateScheme_Replace() {
                            templatePath = this.templatePaths[i],
                            outputPath = $"{dir}/{string.Format(this.fileNames[i], this.scriptName)}",
                        };
                        scheme.AddReplace("#NAME#", this.scriptName);
                        gen.schemes.Add(scheme);
                    }

                    gen.ScriptsGenerate();

                    _window.Close();
                }
            }
        }
    }
}