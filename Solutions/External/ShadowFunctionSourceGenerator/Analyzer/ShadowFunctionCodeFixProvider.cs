using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowFunction;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = "ShadowFunctionCodeFixProvider"), Shared]
public class ShadowFunctionCodeFixProvider : CodeFixProvider {
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("SF00001");

    public override FixAllProvider GetFixAllProvider() {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // 找到方法声明节点
        if (root.FindNode(diagnosticSpan) is not MethodDeclarationSyntax methodDeclaration)
            return;

        var doc = context.Document;
        var project = doc.Project;
        var compilation = await project.GetCompilationAsync();
        if (compilation == null)
            return;

        var methodSymbol = compilation.GetSemanticModel(root.SyntaxTree).GetDeclaredSymbol(methodDeclaration);
        if (methodSymbol == null)
            return;

        var attrOnClass = methodSymbol.ContainingType.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "ShadowFunctionAttribute");
        if (attrOnClass == null)
            return;

        var sourceTypeSymbol = Common.GetArgValueInAttributeData<ITypeSymbol>(attrOnClass, 0);
        if (sourceTypeSymbol == null)
            return;

        var currentAssemblyName = sourceTypeSymbol.ContainingAssembly.Name;
        var sourceTypeNameWithoutGenericParam = Common.GetTypeDisplayNameWithoutGenericParams((INamedTypeSymbol)sourceTypeSymbol);
        Dictionary<string, HashSet<string>> dict;
        if (ShadowFunctionGenerator.UseSourceManifestCarrier) {
            var metadataName = Common.GetSourceFunctionManifestCarrierMetadataName_Plaintext(currentAssemblyName);
            var manifestCarrierTypeSymbol = compilation.GetTypeByMetadataName(metadataName);
            if (manifestCarrierTypeSymbol == null) {
                return;
            }

            if (!Common.ParseSourceFunctionManifest(manifestCarrierTypeSymbol, ShadowFunctionGenerator.ManifestSplit2, ShadowFunctionGenerator.ManifestSplit1,
                    out dict)) {
                return;
            }
        }
        else {
            if (!ShadowFunctionGenerator.MehtodPlaintextCollection.TryGetValue(currentAssemblyName, out dict)) {
                return;
            }
        }

        if (!dict.TryGetValue(sourceTypeNameWithoutGenericParam, out var all_source_method_plaintext)) {
            return;
        }

        Common.DeconstructShadowFunction(methodSymbol, sourceTypeSymbol, out _, out var self, out _, out _);

        try {
            // 获取当前类里所有影子函数, 并转化成唯一明文的名字, 用于后面的判存
            var memberSymbols = methodSymbol.ContainingType.GetMembers();
            var alrealyExistMethodSymbols = memberSymbols
                .Where(x => x.GetAttributes()
                    .Any(xx => xx.AttributeClass?.Name == "ShadowFunctionAttribute"))
                .Select(x => (IMethodSymbol)x);
            HashSet<string> alrealyExistFuncNames = new();
            foreach (var existMethodSymbol in alrealyExistMethodSymbols) {
                Common.DeconstructShadowFunction(existMethodSymbol, sourceTypeSymbol, out _, out var self2, out var finalName, out var finalReturn);
                var alrealyExistFuncName = Common.ConstructMethodCombinationNameIncludeParaName(existMethodSymbol, self2 ? 1 : 0, finalName, finalReturn);
                alrealyExistFuncNames.Add(alrealyExistFuncName);
            }

            foreach (var plaintext in all_source_method_plaintext) {
                // 如果已经实现的影子函数, 就不再加入待选列表中
                if (alrealyExistFuncNames.Contains(plaintext))
                    continue;

                if (Common.DeconstructMethodCombinationName(plaintext, out var methodReturn, out var methodName, out var paramlist)) {
                    // 组合参数内容
                    SeparatedSyntaxList<ParameterSyntax> newParameters = new();
                    if (self) {
                        newParameters = newParameters.Add(methodDeclaration.ParameterList.Parameters[0]);
                    }

                    foreach (var p in paramlist) {
                        newParameters = newParameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(p)));
                    }

                    // 覆盖旧函数
                    var newMethodDeclaration = methodDeclaration
                        .WithIdentifier(SyntaxFactory.Identifier(methodName))
                        .WithReturnType(SyntaxFactory.ParseTypeName(methodReturn).WithTriviaFrom(methodDeclaration.ReturnType))
                        .WithParameterList(SyntaxFactory.ParameterList(newParameters)).WithTriviaFrom(methodDeclaration.ParameterList)
                        .WithModifiers(methodDeclaration.Modifiers)
                        .WithLeadingTrivia(methodDeclaration.GetLeadingTrivia())
                        .WithTrailingTrivia(methodDeclaration.GetTrailingTrivia());

                    // 根据情况自动添加或删除async关键字
                    var hasAsyncToken = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword));
                    var isAsync = methodReturn.Contains("HTask");
                    if (isAsync) {
                        if (!hasAsyncToken) {
                            newMethodDeclaration = newMethodDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));
                        }
                    }
                    else {
                        if (hasAsyncToken) {
                            newMethodDeclaration =
                                newMethodDeclaration.WithModifiers(
                                    SyntaxFactory.TokenList(newMethodDeclaration.Modifiers.Where(m => !m.IsKind(SyntaxKind.AsyncKeyword))));
                        }
                    }

                    // 组合用于提示给用户的名字, 这个名字更直观
                    string paramContent = "";
                    if (self) {
                        paramContent += methodDeclaration.ParameterList.Parameters[0];
                        if (paramlist.Count != 0) {
                            paramContent += ", ";
                        }
                    }

                    for (int i = 0, len = paramlist.Count; i < len; i++) {
                        paramContent += paramlist[i];
                        if (i != len - 1) {
                            paramContent += ", ";
                        }
                    }

                    string methodContent = $"{methodName}({paramContent})";
                    methodContent += ": " + methodReturn;

                    context.RegisterCodeFix(
                        CodeAction.Create("重命名为 " + methodContent,
                            ct => ReplaceMethodAsync(context.Document, methodDeclaration, newMethodDeclaration, ct),
                            equivalenceKey: "MethodRename"),
                        diagnostic);
                }
            }
        }
        catch (Exception e) {
            context.RegisterCodeFix(
                CodeAction.Create("分析器BUG " + e, token => default(Task<Document>)!, equivalenceKey: "MethodRename"),
                diagnostic);
            throw;
        }
    }

    private static async Task<Document> ReplaceMethodAsync(Document document, MethodDeclarationSyntax oldMethod, MethodDeclarationSyntax newMethod,
        CancellationToken cancellationToken) {
        int i = 10;
        FLAG:
        try {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root!.ReplaceNode(oldMethod, newMethod);
            var doc = document.WithSyntaxRoot(newRoot);
            return doc;
        }
        catch (OperationCanceledException) {
            return null!;
        }
        catch (Exception e) when (e is not OperationCanceledException) {
            await Task.Delay(100, cancellationToken);
            if (i-- > 0) {
                goto FLAG;
            }

            throw;
        }
    }
}