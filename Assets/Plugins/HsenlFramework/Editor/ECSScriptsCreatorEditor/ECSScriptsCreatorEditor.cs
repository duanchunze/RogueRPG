// ————————————————
// 版权声明：本文为CSDN博主「Prosper Lee」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
// 原文链接：https://blog.csdn.net/weixin_43526371/article/details/122603671

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

public class ECSScriptsCreatorEditor : EditorWindow {
    private class ScriptsCreatorInfo {
        public string nameWrap;
        public string templatePath; // 模板文件的路径
        public string outputDir; // 输出的目录
        public bool create = true;
    }

    private const string ComponentTemplatePath = "Assets/Editor/ECSScriptGenerator/ScriptTemplates/EcsComponentTemplate.cs.txt";
    private const string SystemTemplatePath = "Assets/Editor/ECSScriptGenerator/ScriptTemplates/EcsSystemTemplate.cs.txt";
    private const string AuthoringTemplatePath = "Assets/Editor/ECSScriptGenerator/ScriptTemplates/EcsAuthoringTemplate.cs.txt";

    private static EditorWindow _window;
    public static string createName;
    private static List<ScriptsCreatorInfo> _creatorInfos;

    // 创建Ecs脚本套件
    [MenuItem("Assets/Create/CreateEcsScriptsKit", false, 0)]
    private static void CreateEcsScriptsKit() {
        _creatorInfos = GetSelectedDirPath();
        _window = GetWindow<ECSScriptsCreatorEditor>();
        _window.Show();
    }

    // 获取文件夹路径
    private static List<ScriptsCreatorInfo> GetSelectedDirPath() {
        // Object[] GetFiltered(Type type, SelectionMode mode)
        // type ---> 只会检索此类型的对象
        // mode ---> SelectionMode.Assets 仅返回 Asset 目录中的资产对象
        var selections = Selection.GetFiltered(typeof(Object), SelectionMode.Assets); // Object ---> UnityEngine.Object

        if (selections.Length != 1) {
            Debug.LogError("只能选中一个路径进行创建");
        }

        // 路径、文件名称 集合
        List<ScriptsCreatorInfo> list = new();
        foreach (var selection in selections) {
            var path = AssetDatabase.GetAssetPath(selection);
            if (!Directory.Exists(path)) {
                var fullPath = Application.dataPath + "/" + path.Replace("Assets/", "");
                FileInfo fileInfo = new(fullPath);
                path = fileInfo.Directory?.FullName.Replace("\\", "/");
                path = path?[path.IndexOf("Assets/", StringComparison.Ordinal)..];
            }

            list.Add(new ScriptsCreatorInfo {
                nameWrap = "{0}",
                templatePath = ComponentTemplatePath,
                outputDir = $"{path}/Component"
            });
            list.Add(new ScriptsCreatorInfo {
                nameWrap = "{0}System",
                templatePath = SystemTemplatePath,
                outputDir = $"{path}/System"
            });
            list.Add(new ScriptsCreatorInfo {
                nameWrap = "{0}Authoring",
                templatePath = AuthoringTemplatePath,
                outputDir = $"{path}/Authoring"
            });
        }

        return list;
    }

    // 创建文件
    private static void CreateScriptFile() {
        foreach (var creatorInfo in _creatorInfos) {
            if (!creatorInfo.create) {
                continue;
            }

            const int instanceId = 0;
            var endAction = CreateInstance<NameByEnterOrUnfocus>();
            var pathName = $"{creatorInfo.outputDir}/{string.Format(creatorInfo.nameWrap, createName)}.cs";
            if (File.Exists(pathName)) {
                continue;
            }
#if false
                * 参数1：instanceId       已编辑资源的实例 ID。
                * 参数2：endAction        监听编辑名称的类的实例化
                * 参数3：pathName         创建的文件路径（包括文件名）
                * 参数4：icon             图标  
                * 参数5：resourceFile     模板路径

                endAction 直接使用 new NameByEnterOrUnfocus() 出现以下警告：
                    NameByEnterOrUnfocus must be instantiated using the ScriptableObject.CreateInstance method instead of new NameByEnterOrUnfocus.
                    必须使用ScriptableObject实例化NameByEnterOrUnfocus。CreateInstance方法，而不是新的NameByEnterOrUnfocus。
#endif
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(instanceId, endAction, pathName, null,
                creatorInfo.templatePath);
        }
    }

    private void OnGUI() {
        var createOk = true;
        var templateFileOk = true;
        var outputDirOk = true;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("脚本名称：", GUILayout.Width(60));
        createName = EditorGUILayout.TextField("", createName, GUILayout.Width(400));
        if (string.IsNullOrEmpty(createName)) {
            GUILayout.Label("⚠ 脚本名不能为空 ", new GUIStyle { normal = { textColor = Color.red } });
            createOk = false;
        }
        else {
            if (!Regex.Match(createName, "[A-Z,a-z]").Success) {
                GUILayout.Label("⚠ 脚本开头必须是字母 ", new GUIStyle { normal = { textColor = Color.red } });
                createOk = false;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        foreach (var creatorInfo in _creatorInfos) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("模板路径：", GUILayout.Width(60));
            EditorGUILayout.LabelField(creatorInfo.templatePath, GUILayout.Width(400));
            if (creatorInfo.create && !File.Exists(creatorInfo.templatePath)) {
                GUILayout.Label("⚠ 模板文件不存在 ", new GUIStyle { normal = { textColor = Color.red } });
                createOk = false;
                templateFileOk = false;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var path = $"{creatorInfo.outputDir}/{string.Format(creatorInfo.nameWrap, createName)}.cs";
            GUILayout.Label("输出路径：", GUILayout.Width(60));
            EditorGUILayout.LabelField(path, GUILayout.Width(400));
            creatorInfo.create = EditorGUILayout.Toggle(creatorInfo.create);
            if (creatorInfo.create && !Directory.Exists(creatorInfo.outputDir)) {
                GUILayout.Label("⚠ 输出路径不存在 ", new GUIStyle { normal = { textColor = Color.red } });
                createOk = false;
                outputDirOk = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        if (!templateFileOk && GUILayout.Button("创建模板文件")) {
            foreach (var creatorInfo in _creatorInfos) {
                if (!creatorInfo.create) continue;
                if (!File.Exists(creatorInfo.templatePath)) {
                    var dir = creatorInfo.templatePath.Substring(0, creatorInfo.templatePath.LastIndexOf("/", StringComparison.Ordinal));
                    if (!Directory.Exists(dir)) {
                        Directory.CreateDirectory(dir);
                    }

                    FileStream fs = new(creatorInfo.templatePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Close();
                }
            }

            AssetDatabase.Refresh();
        }

        if (!outputDirOk && GUILayout.Button("创建输出路径")) {
            foreach (var creatorInfo in _creatorInfos) {
                if (!creatorInfo.create) continue;
                if (!Directory.Exists(creatorInfo.outputDir)) {
                    Directory.CreateDirectory(creatorInfo.outputDir);
                }
            }

            AssetDatabase.Refresh();
        }

        if (createOk && GUILayout.Button("创建")) {
            CreateScriptFile();
            _window.Close();
        }
    }
}

internal class NameByEnterOrUnfocus : EndNameEditAction {
    /// <summary>
    /// 当用户通过按下 Enter 键或失去键盘输入焦点接受编辑后的名称时，Unity 调用此函数
    /// </summary>
    /// <param name="instanceId">已编辑资源的实例 ID。</param>
    /// <param name="pathName">资源的路径。</param>
    /// <param name="resourceFile">传递给ProjectWindowUtil.StartNameEditingIfProjectWindowExists的资源文件字符串参数</param>
    public override void Action(int instanceId, string pathName, string resourceFile) {
        var obj = CreateScript(pathName, resourceFile);
        // 创建并展示
        ProjectWindowUtil.ShowCreatedAsset(obj);
    }

    private static Object CreateScript(string pathName, string resourceFile) {
        // 读取模板文件内容
        StreamReader streamReader = new(resourceFile);
        var templateText = streamReader.ReadToEnd();
        streamReader.Close();

        // 正则替换文本内自定义的变量
        var scriptText = Regex.Replace(templateText, "#NAME#", ECSScriptsCreatorEditor.createName);

        // 写入脚本
        StreamWriter streamWriter = new(pathName);
        streamWriter.Write(scriptText);
        streamWriter.Close();

        // 在路径导入资源
        AssetDatabase.ImportAsset(pathName);
        // 返回给定路径assetPath类型类型的第一个资源对象
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
    }
}