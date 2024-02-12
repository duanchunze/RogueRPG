using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public class HsenlScriptsGenerateScheme : IHsenlScriptsGenerateScheme {
        public string universalTemplatePath; // 通用模板路径
        public string universalTemplateReplaceOrigianl; // 通用被取代源
        public string universalOutputDirectory; // 通用输出目录
        
        public bool clearDirectory;

        private readonly List<HsenlScriptsGenerateInfo> _generateInfos = new();

        public void AddGenerateInfo(HsenlScriptsGenerateInfo generateInfo) {
            this._generateInfos.Add(generateInfo);
        }

        public void Generate() {
            if (!string.IsNullOrEmpty(this.universalOutputDirectory)) {
                if (this.clearDirectory) {
                    FileHelper.CleanDirectory(this.universalOutputDirectory);
                }
            }

            foreach (var generateInfo in this._generateInfos) {
                var templatePath = string.IsNullOrEmpty(generateInfo.templatePath) ? this.universalTemplatePath : generateInfo.templatePath;
                if (string.IsNullOrEmpty(templatePath))
                    throw new Exception("template path is null");
                var outputDir = string.IsNullOrEmpty(generateInfo.outputDirectory) ? this.universalOutputDirectory : generateInfo.outputDirectory;
                if (string.IsNullOrEmpty(outputDir))
                    throw new Exception("output dir is null");
                var replaceOriginal = string.IsNullOrEmpty(generateInfo.templateReplaceOrigianl)
                    ? this.universalTemplateReplaceOrigianl
                    : generateInfo.templateReplaceOrigianl;
                if (string.IsNullOrEmpty(replaceOriginal))
                    throw new Exception("replace original is null");
                var replaceContent = generateInfo.templateReplaceContent;
                if (string.IsNullOrEmpty(replaceContent))
                    throw new Exception("replace content is null");
                var fileName = generateInfo.outputFileName;
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("output file name is null");

                if (!File.Exists(templatePath)) {
                    Debug.LogError($"path '{templatePath}' is not exists");
                    return;
                }

                if (!Directory.Exists(outputDir)) {
                    Debug.LogError($"directory '{outputDir}' is not exists");
                    return;
                }

                using var fsr = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fsr);
                var template = sr.ReadToEnd();
                var content = template.Replace(replaceOriginal, replaceContent);
                if (!outputDir.EndsWith("\\") && !outputDir.EndsWith("/")) {
                    outputDir += "\\";
                }

                var outputPath = outputDir + fileName;
                using var fsw = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using var sw = new StreamWriter(fsw);
                sw.BaseStream.SetLength(0);
                fsw.Write(content.ToByteArray());
            }

            AssetDatabase.Refresh();
        }
    }
}