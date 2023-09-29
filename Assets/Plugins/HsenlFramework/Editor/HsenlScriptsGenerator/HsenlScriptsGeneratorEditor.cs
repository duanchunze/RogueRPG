using System.Collections.Generic;
using System.IO;
using Hsenl;
using UnityEditor;
using UnityEngine;
using Component = Hsenl.Component;
/* 脚本构建有不同的方案可选, 不同方案的构建方式不同, 但有一些通用的选项
 * 通用选项:
 * - 生成前是否清空文件夹
 * 
 */
public abstract class HsenlScriptsGeneratorEditor {
    private List<IHsenlScriptsGenerateScheme> _schemes = new();

    public abstract List<IHsenlScriptsGenerateScheme> GetSchemes();

    protected void ScriptsGenerate() {
        this._schemes = this.GetSchemes();
        foreach (var scheme in this._schemes) {
            scheme.Generate();
        }
    }
}