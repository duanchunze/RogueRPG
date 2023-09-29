using System.Collections.Generic;
using Hsenl;
using UnityEditor;
using Component = UnityEngine.Component;

public class MonoComponentScriptsGen : HsenlScriptsGeneratorEditor {
    private static string _templatePath = "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGenerator/MonoComponentScriptsGen/Hsenl2MonoComponentTemplate.txt";
    private static string _outputDir = "Assets\\Scripts\\Generate\\Hsenl2MonoScripts\\";

    [MenuItem("ET/ScriptsGenerator/清空并生成Hsenl组件的Mono版")]
    private static void Generate() {
        new MonoComponentScriptsGen().ScriptsGenerate();
    }

    public override List<IHsenlScriptsGenerateScheme> GetSchemes() {
        List<IHsenlScriptsGenerateScheme> schemes = new();
        HsenlScriptsGenerateScheme scheme = new() {
            universalTemplatePath = _templatePath,
            universalOutputDirectory = _outputDir,
            universalTemplateReplaceOrigianl = "#NAME#",
            clearDirectory = true,
        };
        schemes.Add(scheme);

        var hsenlComponentTypes = AssemblyHelper.GetSubTypes(typeof(Hsenl.Component));
        foreach (var hsenlComponentType in hsenlComponentTypes) {
            if (hsenlComponentType.IsAbstract)
                continue;
            
            if (hsenlComponentType.IsGenericType)
                continue;

            switch (hsenlComponentType.Name) {
                case "Substantive":
                case "Unbodied":
                    continue;
            }

            scheme.AddGenerateInfo(new HsenlScriptsGenerateInfo(hsenlComponentType.Name, hsenlComponentType.Name + "Component.cs"));
        }

        return schemes;
    }
}