using System.Collections.Generic;
using Hsenl;
using UnityEditor;
using Component = UnityEngine.Component;

public class MonoComponentScriptsGen : HsenlScriptsGeneratorEditor {
    private const string _templatePath = "Assets/Plugins/HsenlFramework/Editor/HsenlScriptsGeneratorEditor/MonoComponentScriptsGen/Hsenl2MonoComponentTemplate.txt";
    private const string _outputDir = "Assets/Scripts/GameView/Generate/Hsenl2MonoGenerate";

    [MenuItem("Hsenl/ScriptsGenerator/清空并生成Hsenl组件的Mono版")]
    private static void Generate() {
        IOHelper.CleanDirectory(_outputDir);
        new MonoComponentScriptsGen().ScriptsGenerate();
    }

    public override List<IHsenlScriptsGenerateScheme> GetSchemes() {
        List<IHsenlScriptsGenerateScheme> schemes = new();
        
        foreach (var hsenlComponentType in AssemblyHelper.GetSubTypes(typeof(Hsenl.Component))) {
            if (hsenlComponentType.IsAbstract)
                continue;
            
            if (hsenlComponentType.IsGenericType)
                continue;

            switch (hsenlComponentType.Name) {
                // case "Substantive":
                case "Unbodied":
                    continue;
            }

            GenerateScheme_Replace scheme = new();
            scheme.templatePath = _templatePath;
            scheme.outputPath = $"{_outputDir}/{hsenlComponentType.Name}Component.cs";
            scheme.AddReplace("#NAME#", hsenlComponentType.Name);
            scheme.AddReplace("#FULLNAME#", hsenlComponentType.FullName);
            schemes.Add(scheme);
        }

        return schemes;
    }
}