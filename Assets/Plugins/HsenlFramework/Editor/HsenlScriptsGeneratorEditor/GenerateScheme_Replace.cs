using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hsenl {
    public class GenerateScheme_Replace : IHsenlScriptsGenerateScheme {
        public string templatePath;
        public string outputPath;
        
        private readonly List<(string original, string replace)> _replaces = new();

        public void AddReplace(string original, string replace) {
            this._replaces.Add((original, replace));
        }

        public void Generate() {
            if (!File.Exists(this.templatePath)) {
                Debug.LogError($"template path '{this.templatePath}' is not exists");
                return;
            }

            var dir = Path.GetDirectoryName(this.outputPath);
            if (!Directory.Exists(dir)) {
                Debug.LogError($"output dir '{dir}' is not exists");
                return;
            }
            
            using var fsr = new FileStream(this.templatePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fsr);
            var template = sr.ReadToEnd();
            string content = template;
            foreach (var replace in this._replaces) {
                content = content.Replace(replace.original, replace.replace);
            }

            using var fsw = new FileStream(this.outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var sw = new StreamWriter(fsw);
            sw.BaseStream.SetLength(0);
            fsw.Write(content.ToByteArray());
            Debug.Log($"Generate Script '{this.outputPath}'");
        }
    }
}