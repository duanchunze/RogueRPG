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

    private readonly ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> _hashcodeLookupTable = new();

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context) {
        this._hashcodeLookupTable.Clear();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(this.AnalyzeSymbol, SymbolKind.Method);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context) {
        string errorDetail;
        var methodSymbol = (IMethodSymbol)context.Symbol;
        var attrOnMehtod = methodSymbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "ShadowFunctionAttribute");
        if (attrOnMehtod == null)
            return;

        try {
            var attrOnClass = methodSymbol.ContainingType.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "ShadowFunctionAttribute");
            if (attrOnClass == null)
                return;

            var sourceTypeSymbol = Common.GetArgValueInAttributeData<ITypeSymbol>(attrOnClass, 0);
            if (sourceTypeSymbol == null)
                return;

            Common.DeconstructShadowFunction(methodSymbol, sourceTypeSymbol, out _, out var self, out var finalName, out var finalReturn);
            string methodUniqueName = Common.ConstructMethodUniqueName(methodSymbol, self ? 1 : 0, finalName, finalReturn);
            var sourceTypeNameWithoutGenericParam = Common.GetTypeNameWithoutGenericParams((INamedTypeSymbol)sourceTypeSymbol);
            uint hashcode = Common.HashCodeCombine(sourceTypeNameWithoutGenericParam, methodUniqueName);
            var hashcodeStr = hashcode.ToString();

            var metadataName = Common.GetSourceFunctionManifestCarrierMetadataName(sourceTypeSymbol.ContainingAssembly.Name);
            if (!this._hashcodeLookupTable.TryGetValue(metadataName, out var dict)) {
                var manifestCarrierTypeSymbol = context.Compilation.GetTypeByMetadataName(metadataName);
                if (manifestCarrierTypeSymbol == null) {
                    errorDetail = "源函数清单没找到!";
                    goto FLAG;
                }

                if (!Common.ParseSourceFunctionManifest(manifestCarrierTypeSymbol, ShadowFunctionGenerator.ManifestSplit2,
                        ShadowFunctionGenerator.ManifestSplit1, out dict)) {
                    errorDetail = "解析源函数清单数据失败!";
                    goto FLAG;
                }

                this._hashcodeLookupTable[metadataName] = dict;
            }

            if (!dict.TryGetValue(sourceTypeNameWithoutGenericParam, out var all_source_method_hashcodes)) {
                errorDetail = "没找到对应源类!";
                goto FLAG;
            }

            if (all_source_method_hashcodes.Contains(hashcodeStr))
                return;

            errorDetail = $"没找到对应源函数!: {hashcodeStr} : {methodUniqueName}";
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