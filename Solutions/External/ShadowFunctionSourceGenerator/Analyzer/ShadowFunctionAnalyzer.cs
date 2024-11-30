using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ShadowFunction;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ShadowFunctionAnalyzer : DiagnosticAnalyzer {
#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor Rule = new("SF00001",
#pragma warning restore RS2008
        "影子函数错误",
        "影子函数'{0}'错误: '{1}'",
        "ShadowFunctionMatchCheck",
        DiagnosticSeverity.Error,
        true,
        "");

    // 分析器是多线程的, 所以要声明变量的话记得加锁
    private readonly ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> _hashcodeLookupTable = new();

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context) {
        this._hashcodeLookupTable.Clear();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(this.AnalyzeSymbol, SymbolKind.Method);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context) {
        var errorDetail = "";
        var methodSymbol = (IMethodSymbol)context.Symbol;

        // 方法必须带有特性
        var attrOnMehtod = methodSymbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "ShadowFunctionAttribute");
        if (attrOnMehtod == null)
            return;

        try {
            // 方法的类必须有特性
            var attrOnClass = methodSymbol.ContainingType.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "ShadowFunctionAttribute");
            if (attrOnClass == null)
                return;

            // 只处理影子类, 不处理元类
            var sourceTypeSymbol = Common.GetArgValueInAttributeData<ITypeSymbol>(attrOnClass, 0);
            if (sourceTypeSymbol == null)
                return;

            // 
            Common.DeconstructShadowFunction(methodSymbol, sourceTypeSymbol, out _, out var self, out var finalName, out var finalReturn);
            var methodCombinationName = Common.ConstructMethodCombinationName(methodSymbol, self ? 1 : 0, finalName, finalReturn);
            var sourceTypeNameWithoutGenericParam = Common.GetTypeDisplayNameWithoutGenericParams((INamedTypeSymbol)sourceTypeSymbol);
            var hashcode = Common.ConstructMethodHashCode(
                sourceTypeSymbol.ContainingAssembly.Name,
                sourceTypeNameWithoutGenericParam,
                methodCombinationName).ToString();

            var currentAssemblyName = sourceTypeSymbol.ContainingAssembly.Name;
            Dictionary<string, HashSet<string>> dict;
            if (ShadowFunctionGenerator.UseSourceManifestCarrier) {
                if (!this._hashcodeLookupTable.TryGetValue(currentAssemblyName, out dict)) {
                    if (!ParseHashcodeLookupTable(out dict)) {
                        goto FLAG;
                    }

                    this._hashcodeLookupTable[currentAssemblyName] = dict;
                }

                bool ParseHashcodeLookupTable(out Dictionary<string, HashSet<string>> result) {
                    var metadataName = Common.GetSourceFunctionManifestCarrierMetadataName_HashCode(currentAssemblyName);
                    var manifestCarrierTypeSymbol = context.Compilation.GetTypeByMetadataName(metadataName);
                    if (manifestCarrierTypeSymbol == null) {
                        errorDetail = "源函数清单没找到!";
                        result = null!;
                        return false;
                    }

                    if (!Common.ParseSourceFunctionManifest(manifestCarrierTypeSymbol, ShadowFunctionGenerator.ManifestSplit2,
                            ShadowFunctionGenerator.ManifestSplit1, out result)) {
                        errorDetail = "解析源函数清单数据失败!";
                        return false;
                    }

                    return true;
                }
            }
            else {
                // 使用这种方案有时会出现找不到程序集的情况, 具体什么原因不是太清楚, 所以使用上面的方案
                if (!ShadowFunctionGenerator.MethodHashcodeCollection.TryGetValue(currentAssemblyName, out dict)) {
                    errorDetail = $"没找到源函数程序集!'{currentAssemblyName}'";
                    goto FLAG;
                }
            }

            if (!dict.TryGetValue(sourceTypeNameWithoutGenericParam, out var all_source_method_hashcodes)) {
                errorDetail = $"没找到源函数类!'{sourceTypeNameWithoutGenericParam}'";
                goto FLAG;
            }

            if (all_source_method_hashcodes.Contains(hashcode))
                return;

            errorDetail = $"没找到对应源函数!: {hashcode} : {methodCombinationName}";
            FLAG:
            var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.ToDisplayString(), errorDetail);
            context.ReportDiagnostic(diagnostic);
        }
        catch (Exception e) {
            var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], e);
            context.ReportDiagnostic(diagnostic);
        }
    }
}