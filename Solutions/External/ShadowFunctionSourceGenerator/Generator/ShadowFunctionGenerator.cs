using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace ShadowFunction {
    [Generator]
    public class ShadowFunctionGenerator : ISourceGenerator {
        public enum FuncKeyMode {
            String,
            HashCode,
            Index,
        }
        private const string ShadowFunctionSystemNamespace = "Hsenl"; // 影子函数系统所在的命名空间
        private const string ShadowFunctionAttributeFullName = "Hsenl.ShadowFunctionAttribute"; // 影子函数特性的全名
        private const string ShadowFunctionAttributeShortName = "ShadowFunction"; // 影子函数特性的简称
        private const string ShadowFunctionMandatoryAttributeFullName = "Hsenl.ShadowFunctionMandatoryAttribute"; // 强制影子函数特性的全名
        private const string ShadowFunctionMandatoryAttributeShortName = "ShadowFunctionMandatory"; // 强制影子函数特性的简称
        private const FuncKeyMode FunctionKeyMode = FuncKeyMode.Index;
        public const char ManifestSplit1 = '#';
        public const char ManifestSplit2 = ';';
        
        // 不使用载体的方式, 就用静态缓存的方式来存储数据, 虽然静态的方式更好, 但偶尔会出现莫名其妙的问题, 导致应该找到的数据找不到, 
        // 所以使用载体的方式, 保稳一些
        public const bool UseSourceManifestCarrier = true;

        public static bool IsGenerating { get; private set; }

        // 创建了一个字典, key1为程序集名, key2为类的typeDisplayStringWithoutGenericParam, HashSet为该类的所有源函数的hashcode
        private static readonly ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> _methodHashcodeCollection = new();
        // 同上, 只不过hashSet里存的是函数的明文
        private static readonly ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> _mehtodPlaintextCollection = new();
        // 按引用的顺序, 记录每个程序集的名字与自己最后一个源函数在整个进程中的索引
        private static readonly List<(string assemblyName, int index)> _sortedIndexes = new();
        // 记录每个程序集里, 所有的源函数的 hashcode 与 index的对应表
        private static readonly Dictionary<string, Dictionary<int, int>> _hashcodeToIndexLookupTable = new();
        
        public static ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> MethodHashcodeCollection => _methodHashcodeCollection;
        public static ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> MehtodPlaintextCollection => _mehtodPlaintextCollection;

        /*
         * - 源生成器在运行时, 会给每一个程序集都创建一个该实例, 并不是整个进程就创建一个实例.
         * - Execute的触发顺序是根据程序集之间相互的引用关系决定的, 引用的程序集总是会先执行.
         * - 不是每次修改, 所有的程序集都会执行Execute, 如果修改没有影响到他, 则不会触发Execute
         */
        
        public void Initialize(GeneratorInitializationContext context) {
            // 当代码变化、生成时, 触发. 每个程序集触发一次, 回调返回的是该程序集所有语法节点
            context.RegisterForSyntaxNotifications(() => new ShadowFunctionReceiver());
        }
    
        public void Execute(GeneratorExecutionContext context) {
            // 判断是否是由我们上面注册的修改通知引发的, 如果不是, 就不处理
            if (context.SyntaxReceiver is not ShadowFunctionReceiver receiver) {
                return;
            }
            
            IsGenerating = true;
            var debugContent = "";
            
    #if DEBUG
            System.Diagnostics.Debugger.Launch();
    #endif
            
            try {
                var compilation = context.Compilation;
                // ReSharper disable once RedundantAssignment
                var currentAssemblyName = context.Compilation.Assembly.Name;

                if (!UseSourceManifestCarrier) {
                    // 先删除当前程序集的一些数据, 如果有影子相关内容, 后面还会再添加, 如果没有, 则保证一些已经没用的程序集, 不会留在静态数据里占空
                    _methodHashcodeCollection.TryRemove(currentAssemblyName, out _);
                    _mehtodPlaintextCollection.TryRemove(currentAssemblyName, out _);
                    for (int i = _sortedIndexes.Count - 1; i >= 0; i--) {
                        var tuple = _sortedIndexes[i];
                        if (tuple.assemblyName == currentAssemblyName) {
                            _sortedIndexes.RemoveAt(i);
                        }
                    }

                    _hashcodeToIndexLookupTable.Remove(currentAssemblyName);
                }

                // 先找到需要的 attribute
                var shadowAttrSymbol = compilation.GetTypeByMetadataName(ShadowFunctionAttributeFullName);
                if (shadowAttrSymbol == null)
                    return;

                var mandatoryAttrSymbol = compilation.GetTypeByMetadataName(ShadowFunctionMandatoryAttributeFullName);

                // 根据SyntaxNode获取其Symbol
                List<INamedTypeSymbol> candidateNameTypeSymbols = new();
                foreach (var typeDeclarationSyntax in receiver.Candidates) {
                    var nameTypeSymbol = compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree).GetDeclaredSymbol(typeDeclarationSyntax);
                    if (nameTypeSymbol == null)
                        continue;

                    candidateNameTypeSymbols.Add(nameTypeSymbol);
                }

                // 遍历所有类, 收集信息
                List<TypeSymbolInfo> typeSymbolInfos = new();
                try {
                    foreach (var candidateNameTypeSymbol in candidateNameTypeSymbols) {
                        // 带有属性标记的类, 就是确定要处理的类, 无论对错都会生成, 错了就显示在代码里
                        var attributeData = candidateNameTypeSymbol.GetAttributes()
                            .FirstOrDefault(x => x.AttributeClass?.Equals(shadowAttrSymbol, SymbolEqualityComparer.Default) ?? false);
                        if (attributeData == null)
                            continue;

                        TypeSymbolInfo symbolInfo = new(candidateNameTypeSymbol, attributeData);
                        this.CollectTypeSymbolInfo(context, symbolInfo, shadowAttrSymbol, mandatoryAttrSymbol);
                        typeSymbolInfos.Add(symbolInfo);
                    }
                }
                catch (Exception e) {
                    context.AddSource($"Error_For_CollectTypeSymbolInfos.g.cs", e.ToString());
                }

                if (!UseSourceManifestCarrier) {
                    // HashCode 集合相关
                    try {
                        {
                            Dictionary<string, HashSet<string>> lookupTable = null;
                            foreach (var typeSymbolInfo in typeSymbolInfos) {
                                if (!typeSymbolInfo.IsSource)
                                    continue;

                                var hashcodes = new HashSet<string>();
                                foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                                    hashcodes.Add(methodSymbolInfo.hashcode.ToString());
                                }

                                lookupTable ??= new Dictionary<string, HashSet<string>>();
                                lookupTable[typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam] = hashcodes;
                            }
                    
                            if (lookupTable != null)
                                _methodHashcodeCollection[currentAssemblyName] = lookupTable;
                        }

                        {
                            Dictionary<string, HashSet<string>> lookupTable = null;
                            foreach (var typeSymbolInfo in typeSymbolInfos) {
                                if (!typeSymbolInfo.IsSource)
                                    continue;

                                var plaintext = new HashSet<string>();
                                foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                                    plaintext.Add(methodSymbolInfo.combinationNameIncludeParamName);
                                }

                                lookupTable ??= new Dictionary<string, HashSet<string>>();
                                lookupTable[typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam] = plaintext;
                            }
                    
                            if (lookupTable != null)
                                _mehtodPlaintextCollection[currentAssemblyName] = lookupTable;
                        }
                    }
                    catch (Exception e) {
                        debugContent += $"{e}";
                    }
                    
                    // 如果再犯错, 就用另一种方案.
                    // 1 每个程序集先遍历自己的依赖程序集, 尝试获得 hashcode 与 index的映射表载体.
                    // 2 根据所有映射表的中提供的dict的长度, 相加得到自己的起始index. 如果没有就从0开始
                    // 3 然后根据起始index制作自己的 hashcode 与 index映射表, 放到载体上.
                    
                    debugContent += $"// {DateTime.Now.Ticks}\n";
                    var record = false;
                    // index相关
                    try {
                        int currentAssemblyIndexInList = -1;
                        int index = 0;
                        Dictionary<int, int> currentHashCodeToIndexLookupTable = new();
                        Dictionary<int, int> allIndexesLookupTable = null;
                        foreach (var typeSymbolInfo in typeSymbolInfos) {
                            // --------源类
                            if (typeSymbolInfo.IsSource) {
                                if (currentAssemblyIndexInList == -1) {
                                    // 判断当前程序集是否存在, 并尝试获取自己在列表中的索引
                                    for (int i = 0; i < _sortedIndexes.Count; i++) {
                                        var tuple = _sortedIndexes[i];
                                        if (tuple.assemblyName == currentAssemblyName) {
                                            currentAssemblyIndexInList = i;
                                            break;
                                        }
                                    }
                
                                    if (currentAssemblyIndexInList != -1) {
                                        // 自己存在, 且处于第一个位置, 那就以0为起点
                                        if (currentAssemblyIndexInList == 0) {
                                            index = 0;
                                        }
                                        // 否则就以前一个程序集完成时的index为自己的起点
                                        else {
                                            index = _sortedIndexes[currentAssemblyIndexInList - 1].index;
                                        }
                                    }
                                    // 自己不存在在列表中, 则要把自己按照程序集的引用顺序添加到列表中去
                                    else {
                                        if (_sortedIndexes.Count == 0) {
                                            currentAssemblyIndexInList = 0;
                                            _sortedIndexes.Add(default); // 先占个位置, 后面会更新
                                        }
                                        else {
                                            var referenceAssemblyNames = context.Compilation.ReferencedAssemblyNames.Select(x => x.Name).ToList();
                                            if (referenceAssemblyNames.Count == 0) {
                                                currentAssemblyIndexInList = 0;
                                                _sortedIndexes.Insert(currentAssemblyIndexInList, default); // 先插入, 占个位置, 后面会更新
                                            }
                                            else {
                                                // 遍历所有程序集, 只要自己没有引用他, 就排到他前面
                                                var insertSucc = false;
                                                for (int i = 0; i < _sortedIndexes.Count; i++) {
                                                    var tuple = _sortedIndexes[i];
                                                    if (!referenceAssemblyNames.Contains(tuple.assemblyName)) {
                                                        insertSucc = true;
                                                        currentAssemblyIndexInList = i;
                                                        _sortedIndexes.Insert(currentAssemblyIndexInList, default); // 先插入, 占个位置, 后面会更新
                                                        break;
                                                    }
                                                }
                
                                                // 如果所有的程序集自己都引用了, 就放到最后
                                                if (!insertSucc) {
                                                    currentAssemblyIndexInList = _sortedIndexes.Count;
                                                    _sortedIndexes.Add(default); // 先占个位置, 后面会更新
                                                }
                                            }
                                        }
                
                                        // 自己存在, 且处于第一个位置, 那就以0为起点
                                        if (currentAssemblyIndexInList == 0) {
                                            index = 0;
                                        }
                                        // 否则就以前一个程序集完成时的index为自己的起点
                                        else {
                                            index = _sortedIndexes[currentAssemblyIndexInList - 1].index;
                                        }
                                    }
                                }
                
                                foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                                    methodSymbolInfo.index = index;
                                    currentHashCodeToIndexLookupTable[methodSymbolInfo.hashcode] = index;
                                    index++;
                                }
                            }
                            // --------影子类
                            else {
                                if (allIndexesLookupTable == null) {
                                    // 把所有程序集里的hashcodeToIndex的映射都集中到一个dict里去
                                    allIndexesLookupTable = new Dictionary<int, int>();
                                    foreach (var kv in _hashcodeToIndexLookupTable) {
                                        foreach (var kv2 in kv.Value) {
                                            allIndexesLookupTable.Add(kv2.Key, kv2.Value);
                                        }
                                    }
                                }
                
                                foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                                    if (allIndexesLookupTable.TryGetValue(methodSymbolInfo.hashcode, out var value)) {
                                        methodSymbolInfo.index = value;    
                                    }
                                    else {
                                        methodSymbolInfo.index = -1;
                                        if (!record) {
                                            record = true;
                                            foreach (var kv in allIndexesLookupTable) {
                                                debugContent += $"{kv.Key}: {kv.Value}\n";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                
                        // 更新源数据
                        if (currentAssemblyIndexInList != -1) {
                            _sortedIndexes[currentAssemblyIndexInList] = (currentAssemblyName, index); // 更新当前程序集的源函数的index信息
                            _hashcodeToIndexLookupTable[currentAssemblyName] = currentHashCodeToIndexLookupTable; // 更新当前程序集的哈希to index的对应表
                        }
                    }
                    catch (Exception e) {
                        context.AddSource($"Error_For_HashCodeToIndexMap.g.cs", e.ToString());
                    }
                }

                if (UseSourceManifestCarrier) {
                    var alreadyGeneratedManifestAttribute = false;
                    void GenerateManifestAttribute(Compilation c) {
                        if (alreadyGeneratedManifestAttribute)
                            return;
                        alreadyGeneratedManifestAttribute = true;
                        var manifestAttributeTemplate = $$"""
                                                          //------------------------------------------------------------------------------
                                                          // <auto-generated>
                                                          //     该特性是由SourceGeerator自动生成, 用于存储当前域的源函数信息, 以供影子函数使用,
                                                          //     我们不需要使用它, 名字前缀是用来防止污染代码提示的
                                                          // </auto-generated>
                                                          //------------------------------------------------------------------------------

                                                          namespace {{c.Assembly.Name}} {
                                                              [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false)]
                                                              public class SFS_DC_SourceFunctionManifestAttribute : global::System.Attribute {
                                                                  public string data;
                                                              
                                                                  public SFS_DC_SourceFunctionManifestAttribute(string data) {
                                                                      this.data = data;
                                                                  }
                                                              }
                                                          }
                                                          """;

                        var sourceText = SourceText.From(manifestAttributeTemplate, Encoding.UTF8);
                        context.AddSource("SFS_DC_SourceFunctionManifestAttribute.g.cs", sourceText);
                        // var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
                        // compilation = compilation.AddSyntaxTrees(syntaxTree); // AddSource后, 就已经添加到compilation中了, 不需要再一次
                    }

                    // 生成源函数清单, 用于代码检测
                    try {
                        StringBuilder stringBuilder = new();
                        var infos = typeSymbolInfos.Where(x => x.IsSource).ToArray();
                        for (int i = 0, len = infos.Length; i < len; i++) {
                            var typeSymbolInfo = infos[i];

                            var last = i == len - 1;

                            stringBuilder.Append("\"");
                            stringBuilder.Append($"{typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam}");
                            stringBuilder.Append(ManifestSplit1);
                            var counter = 0;
                            for (int j = 0, jlen = typeSymbolInfo.methodInfos.Count; j < jlen; j++) {
                                var jlast = j == jlen - 1;
                                var methodSymbolInfo = typeSymbolInfo.methodInfos[j];

                                if (counter >= 0) {
                                    counter = 0;
                                    stringBuilder.Append("\"");
                                    stringBuilder.Append(" +");
                                    stringBuilder.Append("\n");
                                    stringBuilder.Append("                                                         ");
                                    stringBuilder.Append("\"");
                                }

                                stringBuilder.Append(methodSymbolInfo.hashcode);

                                if (!jlast)
                                    stringBuilder.Append(ManifestSplit1);

                                counter++;
                            }

                            stringBuilder.Append(ManifestSplit2);
                            stringBuilder.Append("\"");
                            if (!last) {
                                stringBuilder.Append(" +");
                                stringBuilder.Append("\n");
                                stringBuilder.Append("                                                  ");
                            }
                        }

                        if (stringBuilder.Length != 0) {
                            GenerateManifestAttribute(compilation);
                            StringBuilder subsb = new();
                            subsb.Append($$"""
                                           //------------------------------------------------------------------------------
                                           // <auto-generated>
                                           //     该类是由SourceGeerator自动生成, 用于存储当前域的源函数信息, 以供影子函数使用,
                                           //     我们不需要使用它, 名字前缀是用来防止污染代码提示的, 无实际意义
                                           // </auto-generated>
                                           //------------------------------------------------------------------------------
                                           """);
                            subsb.Append("\n");
                            subsb.Append($"namespace {compilation.Assembly.Name} {{");
                            subsb.Append($"\n");
                            subsb.Append($"    [SFS_DC_SourceFunctionManifestAttribute(data: {stringBuilder})]");
                            subsb.Append($"\n");
                            subsb.Append($"    public static class SFS_DC_SourceFunctionManifestCarrier_HashCode {{ }}");
                            subsb.Append($"\n");
                            subsb.Append($"}}");

                            var sourceText = SourceText.From(subsb.ToString(), Encoding.UTF8);
                            context.AddSource("SFS_DC_SourceFunctionManifestCarrier_HashCode.g.cs", sourceText);
                        }
                    }
                    catch (Exception e) {
                        context.AddSource($"Error_For_Generate_ManifestCarrier_HashCode.g.cs", e.ToString());
                    }

                    // 生成源函数的明文清单, 代码修复可以查询该清单, 以实现代码补全
                    try {
                        StringBuilder stringBuilder = new();
                        var infos = typeSymbolInfos.Where(x => x.IsSource).ToArray();
                        for (int i = 0, len = infos.Length; i < len; i++) {
                            var typeSymbolInfo = infos[i];

                            var last = i == len - 1;

                            stringBuilder.Append("\"");
                            stringBuilder.Append($"{typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam}");
                            stringBuilder.Append(ManifestSplit1);
                            var counter = 0;
                            for (int j = 0, jlen = typeSymbolInfo.methodInfos.Count; j < jlen; j++) {
                                var jlast = j == jlen - 1;
                                var methodSymbolInfo = typeSymbolInfo.methodInfos[j];

                                if (counter >= 0) {
                                    counter = 0;
                                    stringBuilder.Append("\"");
                                    stringBuilder.Append(" +");
                                    stringBuilder.Append("\n");
                                    stringBuilder.Append("                                                         ");
                                    stringBuilder.Append("\"");
                                }

                                stringBuilder.Append(methodSymbolInfo.combinationNameIncludeParamName);

                                if (!jlast)
                                    stringBuilder.Append(ManifestSplit1);

                                counter++;
                            }

                            stringBuilder.Append(ManifestSplit2);
                            stringBuilder.Append("\"");
                            if (!last) {
                                stringBuilder.Append(" +");
                                stringBuilder.Append("\n");
                                stringBuilder.Append("                                                  ");
                            }
                        }

                        if (stringBuilder.Length != 0) {
                            GenerateManifestAttribute(compilation);
                            StringBuilder subsb = new();
                            subsb.Append($$"""
                                           //------------------------------------------------------------------------------
                                           // <auto-generated>
                                           //     该类是由SourceGeerator自动生成, 提供给代码修复器使用
                                           //     我们不需要使用它, 名字前缀是用来防止污染代码提示的, 无实际意义
                                           // </auto-generated>
                                           //------------------------------------------------------------------------------
                                           """);
                            subsb.Append("\n");
                            subsb.Append($"namespace {compilation.Assembly.Name} {{");
                            subsb.Append($"\n");
                            subsb.Append($"    [SFS_DC_SourceFunctionManifestAttribute(data: {stringBuilder})]");
                            subsb.Append($"\n");
                            subsb.Append($"    public static class SFS_DC_SourceFunctionManifestCarrier_Plaintext {{ }}");
                            subsb.Append($"\n");
                            subsb.Append($"}}");

                            var sourceText = SourceText.From(subsb.ToString(), Encoding.UTF8);
                            context.AddSource("SFS_DC_SourceFunctionManifestCarrier_Plaintext.g.cs", sourceText);
                        }
                    }
                    catch (Exception e) {
                        context.AddSource($"Error_For_Generate_ManifestCarrier_Plaintext.g.cs", e.ToString());
                    }
                    
                    // 生成源函数的hashcode to index的清单
                    try {
                        var sourceTypeSymbolInfos = typeSymbolInfos.Where(x => x.IsSource).ToArray();
                        
                        var index = 0;
                        foreach (var referencedAssemblyName in context.Compilation.ReferencedAssemblyNames) {
                            var metadataName = Common.GetSourceFunctionManifestCarrierMetadataName_HashCodeToIndex(referencedAssemblyName.Name);
                            var manifestCarrierTypeSymbol = context.Compilation.GetTypeByMetadataName(metadataName);
                            if (manifestCarrierTypeSymbol == null) 
                                continue;
                        
                            if (!Common.ParseSourceFunctionManifest_HashcodeToIndex(manifestCarrierTypeSymbol, out var dict)) {
                                throw new Exception("解析源函数HashcodeToIndex清单数据失败!");
                            }
                            
                            index += dict.Count;
                        }

                        foreach (var typeSymbolInfo in sourceTypeSymbolInfos) {
                            foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                                methodSymbolInfo.index = index;
                                index++;
                            }
                        }
                        
                        StringBuilder stringBuilder = new();
                        for (int i = 0, len = sourceTypeSymbolInfos.Length; i < len; i++) {
                            var typeSymbolInfo = sourceTypeSymbolInfos[i];
                            for (int j = 0, jlen = typeSymbolInfo.methodInfos.Count; j < jlen; j++) {
                                var methodSymbolInfo = typeSymbolInfo.methodInfos[j];
                                stringBuilder.Append("\"");
                                stringBuilder.Append(methodSymbolInfo.hashcode);
                                stringBuilder.Append(",");
                                stringBuilder.Append(methodSymbolInfo.index);
                                stringBuilder.Append(";");
                                stringBuilder.Append("\"");
                                
                                if (i == len - 1 && j == jlen - 1)
                                    continue;
                                
                                stringBuilder.Append(" +");
                                stringBuilder.Append("\n");
                                stringBuilder.Append("                                                  ");
                            }
                        }

                        if (stringBuilder.Length != 0) {
                            GenerateManifestAttribute(compilation);
                            StringBuilder subsb = new();
                            subsb.Append($$"""
                                           //------------------------------------------------------------------------------
                                           // <auto-generated>
                                           //     该类是由SourceGeerator自动生成, 用于存储当前域的源函数信息, 以供影子函数使用,
                                           //     我们不需要使用它, 名字前缀是用来防止污染代码提示的, 无实际意义
                                           // </auto-generated>
                                           //------------------------------------------------------------------------------
                                           """);
                            subsb.Append("\n");
                            subsb.Append($"namespace {compilation.Assembly.Name} {{");
                            subsb.Append($"\n");
                            subsb.Append($"    [SFS_DC_SourceFunctionManifestAttribute(data: {stringBuilder})]");
                            subsb.Append($"\n");
                            subsb.Append($"    public static class SFS_DC_SourceFunctionManifestCarrier_HashCodeToIndex {{ }}");
                            subsb.Append($"\n");
                            subsb.Append($"}}");

                            var sourceText = SourceText.From(subsb.ToString(), Encoding.UTF8);
                            context.AddSource("SFS_DC_SourceFunctionManifestCarrier_HashCodeToIndex.g.cs", sourceText);
                        }
                    }
                    catch (Exception e) {
                        context.AddSource($"Error_For_Generate_ManifestCarrier_HashCodeToIndex.g.cs", e.ToString());
                    }
                    
                    // 影子函授获取源函数的index
                    try {
                        var shadowTypeSymbolInfos = typeSymbolInfos.Where(x => !x.IsSource).ToArray();
                        Dictionary<string, Dictionary<string, string>> hashcodeToIndexMap = new();
                        foreach (var typeSymbolInfo in shadowTypeSymbolInfos) {
                            var sourceAssemblyName = typeSymbolInfo.sourceTypeSymbol!.ContainingAssembly.Name;
                            if (!hashcodeToIndexMap.TryGetValue(sourceAssemblyName, out var dict)) {
                                var metadataName = Common.GetSourceFunctionManifestCarrierMetadataName_HashCodeToIndex(sourceAssemblyName);
                                var manifestCarrierTypeSymbol = context.Compilation.GetTypeByMetadataName(metadataName);
                                if (manifestCarrierTypeSymbol == null) {
                                    throw new Exception("源函数HashcodeToIndex清单没找到!");
                                }
                            
                                if (!Common.ParseSourceFunctionManifest_HashcodeToIndex(manifestCarrierTypeSymbol, out dict)) {
                                    throw new Exception("解析源函数HashcodeToIndex清单数据失败!");
                                }

                                hashcodeToIndexMap[sourceAssemblyName] = dict;
                            }
                            
                            foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                                if (dict.TryGetValue(methodSymbolInfo.hashcode.ToString(), out var value)) {
                                    methodSymbolInfo.index = int.Parse(value);    
                                }
                                else {
                                    methodSymbolInfo.index = -1;
                                }
                            }
                        }
                    }
                    catch (Exception e) {
                        context.AddSource($"Error_For_HashCodeToIndexMap.g.cs", e.ToString());
                    }
                }

                // 供影子系统的Manager在运行时使用
                try {
                    List<(string typeName, string combinationName)> lookupTable = new();
                    foreach (var typeSymbolInfo in typeSymbolInfos) {
                        if (!typeSymbolInfo.IsSource)
                            continue;

                        foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                            lookupTable.Add((typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam, methodSymbolInfo.combinationName));
                        }
                    }

                    if (lookupTable.Count != 0) {
                        StringBuilder subsb = new();
                        subsb.Append($$"""
                                       //------------------------------------------------------------------------------
                                       // <auto-generated>
                                       //     该类是由SourceGeerator自动生成, 用于存储当前域的源函数明文信息, 以供查阅使用.
                                       //       key是函数所在类的名字, value是函数的组合名
                                       //       名字前缀是用来防止污染代码提示的
                                       // </auto-generated>
                                       //------------------------------------------------------------------------------
                                       """);
                        subsb.Append("\n");
                        subsb.Append("using System.Collections.Generic;");
                        subsb.Append("\n");
                        subsb.Append("\n");
                        subsb.Append($"namespace {compilation.Assembly.Name} {{");
                        subsb.Append($"\n");
                        subsb.Append($"    public class SFS_DC_SourceFunctionCollection {{");
                        subsb.Append($"\n");
                        subsb.Append($"        public readonly List<(string typeName, string combinationName)> lookupTable = new() {{");
                        subsb.Append($"\n");
                        var currentTypeName = lookupTable[0].typeName;
                        foreach (var tuple in lookupTable) {
                            if (currentTypeName != tuple.typeName) {
                                currentTypeName = tuple.typeName;
                                subsb.Append($"\n");
                            }

                            subsb.Append($"            (\"{tuple.typeName}\", \"{tuple.combinationName}\"),");
                            subsb.Append($"\n");
                        }

                        subsb.Append($"        }};");
                        subsb.Append($"\n");
                        subsb.Append($"    }}");
                        subsb.Append($"\n");
                        subsb.Append($"}}");

                        var sourceText = SourceText.From(subsb.ToString(), Encoding.UTF8);
                        context.AddSource("SFS_DC_SourceFunctionCollection.g.cs", sourceText);
                    }
                }
                catch (Exception e) {
                    context.AddSource($"Error_For_Generate_SourceFunctionCollection.g.cs", e.ToString());
                }
                
                // 同上
                try {
                    List<(string hashcode, string combinationName)> lookupTable = new();
                    foreach (var typeSymbolInfo in typeSymbolInfos) {
                        if (!typeSymbolInfo.IsSource)
                            continue;
                        foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                            switch (FunctionKeyMode) {
                                case FuncKeyMode.String:
                                case FuncKeyMode.HashCode:
                                    lookupTable.Add((methodSymbolInfo.hashcode.ToString(), methodSymbolInfo.combinationName));
                                    break;
                                case FuncKeyMode.Index:
                                    lookupTable.Add((methodSymbolInfo.index.ToString(), methodSymbolInfo.combinationName));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }

                    if (lookupTable.Count != 0) {
                        StringBuilder subsb = new();
                        subsb.Append($$"""
                                       //------------------------------------------------------------------------------
                                       // <auto-generated>
                                       //     该类是由SourceGeerator自动生成, 用于存储当前域所有源函数, 以供管理器函数使用.
                                       //       key是函数的唯一码, value是函数的组合名
                                       //       名字前缀是用来防止污染代码提示的
                                       // </auto-generated>
                                       //------------------------------------------------------------------------------
                                       """);
                        subsb.Append("\n");
                        subsb.Append("using System.Collections.Generic;");
                        subsb.Append("\n");
                        subsb.Append("\n");
                        subsb.Append($"namespace {compilation.Assembly.Name} {{");
                        subsb.Append($"\n");
                        subsb.Append($"    public class SFS_DC_SourceFunctionLookupTable_All {{");
                        subsb.Append($"\n");
                        subsb.Append($"        public readonly List<(int hashcode, string combinationName)> lookupTable = new() {{");
                        subsb.Append($"\n");
                        foreach (var tuple in lookupTable) {
                            subsb.Append($"            ({tuple.hashcode}, \"{tuple.combinationName}\"),");
                            subsb.Append($"\n");
                        }

                        subsb.Append($"        }};");
                        subsb.Append($"\n");
                        subsb.Append($"    }}");
                        subsb.Append($"\n");
                        subsb.Append($"}}");

                        var sourceText = SourceText.From(subsb.ToString(), Encoding.UTF8);
                        context.AddSource("SFS_DC_SourceFunctionLookupTable_All.g.cs", sourceText);
                    }
                }
                catch (Exception e) {
                    context.AddSource($"Error_For_Generate_SourceFunctionLookupTable_All.g.cs", e.ToString());
                }

                // 同上
                try {
                    List<(string hashcode, string combinationName)> lookupTable = new();
                    foreach (var typeSymbolInfo in typeSymbolInfos) {
                        if (!typeSymbolInfo.IsSource)
                            continue;
                        foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                            if (methodSymbolInfo.RealAllowMultiShadowFuns)
                                continue;

                            switch (FunctionKeyMode) {
                                case FuncKeyMode.String:
                                case FuncKeyMode.HashCode:
                                    lookupTable.Add((methodSymbolInfo.hashcode.ToString(), methodSymbolInfo.combinationName));
                                    break;
                                case FuncKeyMode.Index:
                                    lookupTable.Add((methodSymbolInfo.index.ToString(), methodSymbolInfo.combinationName));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }

                    if (lookupTable.Count != 0) {
                        StringBuilder subsb = new();
                        subsb.Append($$"""
                                       //------------------------------------------------------------------------------
                                       // <auto-generated>
                                       //     该类是由SourceGeerator自动生成, 用于存储当前域所有Single源函数(不允许实现多个影子函数的源函数), 以供管理器函数使用.
                                       //       key是函数的唯一码, value是函数的组合名
                                       //       名字前缀是用来防止污染代码提示的
                                       // </auto-generated>
                                       //------------------------------------------------------------------------------
                                       """);
                        subsb.Append("\n");
                        subsb.Append("using System.Collections.Generic;");
                        subsb.Append("\n");
                        subsb.Append("\n");
                        subsb.Append($"namespace {compilation.Assembly.Name} {{");
                        subsb.Append($"\n");
                        subsb.Append($"    public class SFS_DC_SourceFunctionLookupTable_Single {{");
                        subsb.Append($"\n");
                        subsb.Append($"        public readonly List<(int hashcode, string combinationName)> lookupTable = new() {{");
                        subsb.Append($"\n");
                        foreach (var tuple in lookupTable) {
                            subsb.Append($"            ({tuple.hashcode}, \"{tuple.combinationName}\"),");
                            subsb.Append($"\n");
                        }

                        subsb.Append($"        }};");
                        subsb.Append($"\n");
                        subsb.Append($"    }}");
                        subsb.Append($"\n");
                        subsb.Append($"}}");

                        var sourceText = SourceText.From(subsb.ToString(), Encoding.UTF8);
                        context.AddSource("SFS_DC_SourceFunctionLookupTable_Single.g.cs", sourceText);
                    }
                }
                catch (Exception e) {
                    context.AddSource($"Error_For_Generate_SourceFunctionLookupTable_Single.g.cs", e.ToString());
                }
                
                // 遍历所有类, 挨个生成代码
                foreach (var typeSymbolInfo in typeSymbolInfos) {
                    try {
                        if (typeSymbolInfo.IsSource) {
                            CreateSourceTypeCode(context, typeSymbolInfo);
                        }
                        else {
                            CreateShadowTypeCode(context, typeSymbolInfo);
                        }

                        if (typeSymbolInfo.error != "succ") {
                            context.AddSource($"Error_For_Generate_{typeSymbolInfo.metadataName}.g.cs", typeSymbolInfo.error);
                        }
                    }
                    catch (Exception e) {
                        context.AddSource($"Error_For_Generate_{typeSymbolInfo.metadataName}.g.cs", e.ToString());
                    }
                }
            }
            finally {
                context.AddSource($"SFS_DC_ShadowFunctionSystemDebug.g.cs", debugContent);
                IsGenerating = false;
            }
        }
    
        private void CollectTypeSymbolInfo(GeneratorExecutionContext context, TypeSymbolInfo typeSymbolInfo, INamedTypeSymbol attrTypeSymbol, 
            INamedTypeSymbol? mandatoryAttrTypeSymbol) {
            var selfTypeSymbol = typeSymbolInfo.typeSymbol;
            
            if (selfTypeSymbol.TypeKind != TypeKind.Class && selfTypeSymbol.TypeKind != TypeKind.Struct) {
                typeSymbolInfo.error = $"TypeKind: '{selfTypeSymbol.TypeKind}'不支持";
                return;
            }
            
            typeSymbolInfo.sourceTypeSymbol = Common.GetArgValueInAttributeData<INamedTypeSymbol>(typeSymbolInfo.attributeData, 0);
            typeSymbolInfo.sourceTypePriority = Common.GetArgValueInAttributeData<int>(typeSymbolInfo.attributeData, 1);
            typeSymbolInfo.allowMultiShadowFuns = Common.GetArgValueInAttributeData<bool>(typeSymbolInfo.attributeData, 2);
            
            typeSymbolInfo.displayString = selfTypeSymbol.ToDisplayString();
            typeSymbolInfo.metadataName = selfTypeSymbol.MetadataName;
            typeSymbolInfo.name = selfTypeSymbol.Name;
    
            if (typeSymbolInfo.IsSource) {
                typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam = Common.GetTypeDisplayNameWithoutGenericParams(typeSymbolInfo.typeSymbol);
    
                var memberSymbols = Common.GetMembersInBase(selfTypeSymbol);
                foreach (var memberSymbol in memberSymbols) {
                    if (memberSymbol.Kind != SymbolKind.Method)
                        continue;
                    
                    // 需要包含了特性
                    var attribueData = memberSymbol.GetAttributes()
                        .FirstOrDefault(x => x.AttributeClass?.Equals(attrTypeSymbol, SymbolEqualityComparer.Default) ?? false);
                    if (attribueData == null)
                        continue;
    
                    // 如果该成员是父类的成员, 那么还需要判断父类是否也包含了特性
                    if (memberSymbol.ContainingType.Equals(selfTypeSymbol, SymbolEqualityComparer.Default)) {
                        if (!memberSymbol.ContainingType.GetAttributes().Any(xx => xx.AttributeClass != null && xx.AttributeClass.Equals(attrTypeSymbol, SymbolEqualityComparer.Default)))
                            continue;
                    }
    
                    var methodSymbol = (IMethodSymbol)memberSymbol;
                    
                    Common.DeconstructSourceFunction(methodSymbol, out var hasRet, out var finalName, out var finalReturn);
                    
                    // 得到该函数的唯一编号
                    var methodCombinationName = Common.ConstructMethodCombinationName(methodSymbol, 0, finalName, finalReturn);
                    var hashcode = Common.ConstructMethodHashCode(
                        typeSymbolInfo.assemblySymbol.Name,
                        typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam,
                        methodCombinationName);
                    
                    MethodSymbolInfo methodSymbolInfo = new(
                        typeSymbolInfo, 
                        methodSymbol, 
                        attribueData, 
                        finalName, 
                        finalReturn, 
                        methodCombinationName, 
                        hashcode,
                        hasRet);
                    
                    methodSymbolInfo.allowMultiShadowFuns = Common.GetArgValueInAttributeData<bool>(attribueData, 2);
                    methodSymbolInfo.combinationNameIncludeParamName = Common.ConstructMethodCombinationNameIncludeParaName(methodSymbol, 0, finalName, finalReturn);
                    
                    typeSymbolInfo.methodInfos.Add(methodSymbolInfo);
                }
            }
            else {
                typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam = Common.GetTypeDisplayNameWithoutGenericParams(typeSymbolInfo.sourceTypeSymbol!);
                
                if (selfTypeSymbol.ContainingAssembly.ToDisplayString() == typeSymbolInfo.sourceTypeSymbol!.ContainingAssembly.ToDisplayString()) {
                    typeSymbolInfo.error = "影子类和源类不能在同一个程序集";
                    return;
                }
    
                var memberSymbols = selfTypeSymbol.GetMembers();
                List<(IMethodSymbol methodSymbol, AttributeData attributeData)> shadowMethods = new();
                List<(IMethodSymbol methodSymbol, AttributeData attributeData)> shadowMandatoryMethods = new();
                foreach (var memberSymbol in memberSymbols) {
                    if (memberSymbol.Kind != SymbolKind.Method)
                        continue;
                    
                    foreach (var attribute in memberSymbol.GetAttributes()) {
                        if (attribute.AttributeClass != null && attribute.AttributeClass.Equals(attrTypeSymbol, SymbolEqualityComparer.Default)) {
                            shadowMethods.Add(((IMethodSymbol)memberSymbol, attribute));
                            break;
                        }
    
                        if (attribute.AttributeClass != null && attribute.AttributeClass.Equals(mandatoryAttrTypeSymbol, SymbolEqualityComparer.Default)) {
                            shadowMandatoryMethods.Add(((IMethodSymbol)memberSymbol, attribute));
                            break;
                        }
                    }
                }
                
                var sourceTypeContainingAssemblyName = typeSymbolInfo.sourceTypeSymbol.ContainingAssembly.Name;
                // 从源类中获取所有源函数的信息汇总.   现在不需要了, 检测影子函数连接是否成功, 交给代码分析器去做了
                // HashSet<string> all_source_method_hashcodes = null;
                // if (shadowMethods.Count != 0) {
                //     if (!_methodNameToHashcodeLookupTable.TryGetValue(sourceTypeContainingAssemblyName, out var dict)) {
                //         var metadataName = Common.GetSourceFunctionManifestCarrierMetadataName(sourceTypeContainingAssemblyName);
                //         var manifestCarrierTypeSymbol = context.Compilation.GetTypeByMetadataName(metadataName);
                //         if (manifestCarrierTypeSymbol != null) {
                //             if (Common.ParseSourceFunctionManifest(manifestCarrierTypeSymbol, ManifestSplit2, ManifestSplit1, out dict)) {
                //                 _methodNameToHashcodeLookupTable[sourceTypeContainingAssemblyName] = dict;
                //             }
                //         }
                //     }
                //
                //     dict?.TryGetValue(typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam, out all_source_method_hashcodes);
                // }
                
                // 筛选所有符合条件的影子函数1
                foreach (var (methodSymbol, attributeData) in shadowMethods) {
                    var pri = Common.GetArgValueInAttributeData<int>(attributeData, 1);
                    
                    // 拆解影子函数
                    Common.DeconstructShadowFunction(methodSymbol, typeSymbolInfo.sourceTypeSymbol, out var hasRet, out var self, out var finalName, out var finalReturn);
    
                    // 得到该函数的唯一编号
                    var methodCombinationName = Common.ConstructMethodCombinationName(methodSymbol, self ? 1 : 0, finalName, finalReturn);
                    var hashcode = Common.ConstructMethodHashCode(
                        sourceTypeContainingAssemblyName,
                        typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam, 
                        methodCombinationName);
                    // var hashcodeStr = hashcode.ToString();
                    
                    ShadowMethodSymbolInfo shadowMethodSymbolInfo = new(
                        typeSymbolInfo, 
                        methodSymbol, 
                        attributeData, 
                        finalName, 
                        finalReturn, 
                        methodCombinationName, 
                        hashcode, 
                        hasRet, 
                        pri);
                    
                    shadowMethodSymbolInfo.allowMultiShadowFuns = Common.GetArgValueInAttributeData<bool>(attributeData, 2);

                    // if (all_source_method_hashcodes == null) {
                    //     shadowMethodSymbolInfo.methodSucc = $"没到源类的清单";
                    // }
                    // else {
                    //     if (!all_source_method_hashcodes.Contains(hashcodeStr)) {
                    //         shadowMethodSymbolInfo.methodSucc = $"没找到匹配的源函数";
                    //     }
                    // }
                    
                    typeSymbolInfo.methodInfos.Add(shadowMethodSymbolInfo);
                }
    
                foreach (var (methodSymbol, attributeData) in shadowMandatoryMethods) {
                    var pri = Common.GetArgValueInAttributeData<int>(attributeData, 0);
                    
                    // 拆解影子函数
                    Common.DeconstructShadowFunction(methodSymbol, typeSymbolInfo.sourceTypeSymbol, out var hasRet, out var self, out var finalName, out var finalReturn);
                    
                    // 得到该函数的唯一编号
                    var methodCombinationName = Common.ConstructMethodCombinationName(methodSymbol, self ? 1 : 0, finalName, finalReturn);
                    var hashcode = Common.ConstructMethodHashCode(
                        sourceTypeContainingAssemblyName,
                        typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam, 
                        methodCombinationName);
    
                    ShadowMethodSymbolInfo shadowMethodSymbolInfo = new(
                        typeSymbolInfo, 
                        methodSymbol, 
                        attributeData, 
                        finalName, 
                        finalReturn, 
                        methodCombinationName, 
                        hashcode, 
                        hasRet, 
                        pri);
                    
                    shadowMethodSymbolInfo.allowMultiShadowFuns = Common.GetArgValueInAttributeData<bool>(attributeData, 2);
                    
                    // 对于标记为Mandatory特性的影子函数, 尽管添加就行了
                    typeSymbolInfo.methodInfos.Add(shadowMethodSymbolInfo);
                }
            }
        }
    
        private static void CreateSourceTypeCode(GeneratorExecutionContext context, TypeSymbolInfo typeSymbolInfo) {
            TypeCodeBuilder typeCodeBuilder = new();
            typeCodeBuilder.AppendUsing("using System;");
            typeCodeBuilder.Namespace = typeSymbolInfo.typeSymbol.ContainingNamespace.ToDisplayString();
            typeCodeBuilder.DeclaredAccessibility = typeSymbolInfo.typeSymbol.DeclaredAccessibility;
            typeCodeBuilder.IsPartial = true;
            typeCodeBuilder.TypeKind = typeSymbolInfo.typeSymbol.TypeKind;
            typeCodeBuilder.Name = typeSymbolInfo.typeSymbol.ToDisplayString(new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
            ));
            
            foreach (var methodSymbolInfo in typeSymbolInfo.methodInfos) {
                if (methodSymbolInfo.methodSymbol.IsAbstract) continue;

                // 该源函数是否支持多个影子函数, 如果支持的话, 就会采用foreach的方式
                var multiShadowFuncs = methodSymbolInfo.RealAllowMultiShadowFuns;
                // 如果源函数有返回值, 但其却设置了允许实现多个影子函数, 这种情况, 就把返回值隐藏, 并采用回调返回值的形式来处理
                var useReturnCallback = methodSymbolInfo.hasRet && multiShadowFuncs;
                // 如果源函数有返回值, 且其没有设置允许实现多个影子函数, 这种情况, 就是强制的Invoke, 必须要实现, 否则调用函数会报错
                var mustImplemented = methodSymbolInfo.hasRet && !multiShadowFuncs;
                // 如果函数是异步函数同时不带有返回值, 且允许多影子函数, 这种情况采用的是最复杂的一种异步优先级排序执行方案
                var useAsyncPriority = methodSymbolInfo.methodSymbol.IsAsync && !methodSymbolInfo.hasRet && multiShadowFuncs;
                // 是否使用try catch包裹, 只要是多影子函数, 就包裹, 否则就不包裹
                var useTryCatch = multiShadowFuncs;
                
                MethodCodeBuilder methodCodeBuilder = new();
                methodCodeBuilder.DeclaredAccessibility = Accessibility.Private;
                methodCodeBuilder.IsStatic = methodSymbolInfo.methodSymbol.IsStatic;
                methodCodeBuilder.IsAsync = methodSymbolInfo.methodSymbol.IsAsync && !useReturnCallback;
                methodCodeBuilder.Return = useReturnCallback ? "void" : methodSymbolInfo.finalReturnName; // 除了useReturnCallback这种特殊情况, 其他的都用源函数的返回值
                methodCodeBuilder.Name = $"{methodSymbolInfo.finalMethodName}Shadow";
                
                string paramTypeNames = null; // fun(int arg1, string arg2); 记录成 int, string
                string paramNames = null; // 同样上面示例函数, 记录成 arg1, arg2
                // -------- 构建函数头
                for (int i = 0, len = methodSymbolInfo.methodSymbol.Parameters.Length; i < len; i++) {
                    var last = i == len - 1;
                    var parameter = methodSymbolInfo.methodSymbol.Parameters[i];
                        
                    methodCodeBuilder.AppendParam($"{parameter.Type} {parameter.Name}");
    
                    // 这两个参数是后面用的
                    paramTypeNames += parameter.Type.ToString();
                    if (!last) paramTypeNames += ", ";
    
                    paramNames += parameter.Name;
                    if (!last) paramNames += ", ";
                }

                
                if (useReturnCallback) {
                    methodCodeBuilder.AppendParam($"Action<{methodSymbolInfo.methodSymbol.ReturnType.ToDisplayString()}> action = null");
                }
                
                // ---------- 构建函数体
                StringBuilder methodBodyBuilder = new();

                if (!multiShadowFuncs) {
                    methodBodyBuilder.Append($$"""
                                                   if (!{{ShadowFunctionSystemNamespace}}.ShadowFunction.GetFunction(
                                               """);
                }
                else {
                    methodBodyBuilder.Append($$"""
                                                   if (!{{ShadowFunctionSystemNamespace}}.ShadowFunction.GetFunctions(
                                               """);
                }

                switch (FunctionKeyMode) {
                    case FuncKeyMode.String:
                        methodBodyBuilder.Append($"(int)global::System.HashCode.Combine(\"{typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam}\", \"{methodSymbolInfo.combinationName}\")");
                        break;
                    case FuncKeyMode.HashCode:
                        methodBodyBuilder.Append(methodSymbolInfo.hashcode);
                        break;
                    case FuncKeyMode.Index:
                        methodBodyBuilder.Append(methodSymbolInfo.index);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (!multiShadowFuncs)
                    methodBodyBuilder.Append(", out var del))");
                else
                    methodBodyBuilder.Append(", out var dels))");
                methodBodyBuilder.Append("\n");
                methodBodyBuilder.Append($$"""
                                                            
                                           """);
                // 没找到影子函数返回, 如果是必须要实现的, 则报错, 否则就只是return
                if (mustImplemented)
                    methodBodyBuilder.Append("throw new NotImplementedException();");
                else
                    methodBodyBuilder.Append("return;");
                methodBodyBuilder.Append("\n");

                if (useAsyncPriority) {
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   HTask? first0PriorityDel = null;
                                                   ListHTask<HTask> parallels = null;
                                               """);
                }
                
                if (multiShadowFuncs) {
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   foreach (var kv in dels) {
                                               """); // foreach 开始
                }
                
                if (useAsyncPriority) {
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                       if (kv.Key < 0) {
                                                           if (Convert2Task(kv.Value, out var task)) {
                                               """);
                    if (useTryCatch) { // useAsyncPriority下, 必定要用try, 这里相当于废话了
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                   try {
                                                                       await task;
                                                                   }
                                                                   catch(Exception e) {
                                                                       ShadowDebug.LogError(e);
                                                                   }
                                                   """);
                        methodBodyBuilder.Append("\n");
                    }
                    else {
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                   await task;
                                                   """);
                        methodBodyBuilder.Append("\n");
                    }
                    methodBodyBuilder.Append($$"""
                                                           }
                                                       }
                                               """);
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                       else if (kv.Key == 0) {
                                                           if (first0PriorityDel == null && parallels == null) {
                                                               if (!Convert2Task(kv.Value, out var task)) {
                                                                   continue;
                                                               }
                                                               first0PriorityDel = task;
                                                               continue;
                                                           }
                                                           
                                                           parallels ??= ListHTask<HTask>.Create();
                                                           if (first0PriorityDel != null) {
                                                               parallels.Add(first0PriorityDel.Value);
                                                               first0PriorityDel = null;
                                                           }
                                                           
                                                           try {
                                                               if (Convert2Task(kv.Value, out var task))
                                                                   parallels.Add(task);
                                                           }
                                                           catch {
                                                               parallels.Dispose();
                                                               throw;
                                                           }
                                                       }
                                                       else {
                                                           if (first0PriorityDel != null || parallels != null) {
                                                               try {
                                                                   await GetParallelTasks();
                                                               }
                                               """);
                    if (useTryCatch) {
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                   catch(Exception e) {
                                                                       ShadowDebug.LogError(e);
                                                                   }
                                                   """);
                        methodBodyBuilder.Append("\n");
                    }
                    else {
                        methodBodyBuilder.Append("\n");
                    }

                    methodBodyBuilder.Append($$"""
                                                               finally {
                                                                   parallels?.Dispose();
                                                                   first0PriorityDel = null;
                                                                   parallels = null;
                                                               }
                                                           }
                                                           if (Convert2Task(kv.Value, out var task)) {
                                               """);
                    if (useTryCatch) {
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                   try {
                                                                       await task;
                                                                   }
                                                                   catch(Exception e) {
                                                                       ShadowDebug.LogError(e);
                                                                   }
                                                   """);
                        methodBodyBuilder.Append("\n");
                    }
                    else {
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                   await task;
                                                   """);
                        methodBodyBuilder.Append("\n");
                    }
                    methodBodyBuilder.Append($$"""
                                                           }
                                                       }
                                               """);
                }
                // 可能不是异步函数, 或者可能是带有返回值的函数, 无论是哪种情况, 都不适合使用上面那种复杂的异步优先级执行方案, 所以使用较简单的顺序执行的方案
                else
                {
                    var ifcount = 0;
                    for (var i = 0; i < 2; i++) {
                        var hasThis = i == 1;
                        
                        if (methodSymbolInfo.methodSymbol.IsStatic && hasThis) {
                            continue;
                        }
                        
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                
                                                   """);
                        
                        if (ifcount > 0) methodBodyBuilder.Append("else ");
                        if (!multiShadowFuncs)
                            methodBodyBuilder.Append("if (del is global::System.");
                        else
                            methodBodyBuilder.Append("if (kv.Value is global::System.");
                        var isFunc = methodSymbolInfo.methodSymbol.IsAsync || methodSymbolInfo.hasRet;
                        methodBodyBuilder.Append(isFunc ? "Func" : "Action");
                        if (hasThis || !string.IsNullOrEmpty(paramTypeNames) || isFunc) {
                            methodBodyBuilder.Append("<");
                            var hasPreceding = false;
                            if (hasThis) {
                                methodBodyBuilder.Append(typeSymbolInfo.displayString);
                                hasPreceding = true;
                            }
                        
                            if (!string.IsNullOrEmpty(paramTypeNames)) {
                                if (hasPreceding) methodBodyBuilder.Append(", ");
                                methodBodyBuilder.Append(paramTypeNames);
                                hasPreceding = true;
                            }
                        
                            if (methodSymbolInfo.hasRet || methodSymbolInfo.methodSymbol.IsAsync) {
                                if (hasPreceding) methodBodyBuilder.Append(", ");
                                methodBodyBuilder.Append(methodSymbolInfo.finalReturnName);
                            }
                        
                            methodBodyBuilder.Append(">");
                        }
                        
                        var argnum = ifcount != 0 ? ifcount.ToString() : null;
                        methodBodyBuilder.Append($" func{argnum}) {{");
                        methodBodyBuilder.Append("\n");
                        {
                            for (var j = 0; j < 2; j++) {
                                if ((!methodSymbolInfo.hasRet || !multiShadowFuncs) && j == 1)
                                    break;
                                
                                var space = "            ";
                                methodBodyBuilder.Append(space);
                                if (useReturnCallback) {
                                    if (j == 0) {
                                        methodBodyBuilder.Append($$"""
                                                                   if (action == null) {
                                                                   """);
                                        space += "    ";
                                        methodBodyBuilder.Append("\n");
                                        methodBodyBuilder.Append(space);
                                    }
                                    else if (j == 1) {
                                        methodBodyBuilder.Append($$"""
                                                                   else {
                                                                   """);
                                        space += "    ";
                                        methodBodyBuilder.Append("\n");
                                        methodBodyBuilder.Append(space);
                                    }
                                }

                                if (useTryCatch) {
                                    methodBodyBuilder.Append($$"""
                                                               try {
                                                               """);
                                    space += "    ";
                                    methodBodyBuilder.Append("\n");
                                    methodBodyBuilder.Append(space);
                                }

                                var writed2 = useReturnCallback && j == 1;
                                if (writed2) {
                                    methodBodyBuilder.Append($$"""
                                                               action.Invoke(
                                                               """);
                                }

                                
                                if (mustImplemented) {
                                    if (methodCodeBuilder.IsAsync) {
                                        methodBodyBuilder.Append($$"""
                                                                   return await 
                                                                   """);
                                    }
                                    else {
                                        methodBodyBuilder.Append($$"""
                                                                   return 
                                                                   """);
                                    }
                                }
                                else {
                                    if (methodCodeBuilder.IsAsync) {
                                        methodBodyBuilder.Append($$"""
                                                                   await 
                                                                   """);
                                    }
                                }
                                
                                methodBodyBuilder.Append($"func{argnum}.Invoke(");
                                var hasPreceding = false;
                                if (hasThis) {
                                    methodBodyBuilder.Append("this");
                                    hasPreceding = true;
                                }
                                if (!string.IsNullOrEmpty(paramNames)) {
                                    if (hasPreceding) methodBodyBuilder.Append(", ");
                                    methodBodyBuilder.Append(paramNames);
                                }
                                
                                if (writed2) {
                                    methodBodyBuilder.Append(")");
                                }
                                
                                methodBodyBuilder.Append(");");
                                methodBodyBuilder.Append("\n");
                                if (useTryCatch) {
                                    space = space.Remove(space.Length - 4);
                                    methodBodyBuilder.Append($$"""
                                                               {{space}}}
                                                               """);
                                    methodBodyBuilder.Append("\n");
                                    methodBodyBuilder.Append($$"""
                                                               {{space}}catch (Exception e) {
                                                               {{space}}    ShadowDebug.LogError(e);
                                                               {{space}}}
                                                               """);
                                    methodBodyBuilder.Append("\n");
                                }

                                if (useReturnCallback) {
                                    space = space.Remove(space.Length - 4);
                                    methodBodyBuilder.Append($$"""
                                                                           }
                                                               """);
                                    methodBodyBuilder.Append("\n");
                                }
                            }
                            
                            methodBodyBuilder.Append($$"""
                                                                    }
                                                       """);
                        }
                        
                        ifcount++;
                    }
                }
                
                if (multiShadowFuncs) {
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   }
                                               """); // foreach 结束
                }
                
                if (!useAsyncPriority && mustImplemented) {
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   
                                               """);
                    methodBodyBuilder.Append("throw new NotImplementedException();");
                }
                
                if (useAsyncPriority) {
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   if (first0PriorityDel != null || parallels != null) {
                                                       try {
                                                           await GetParallelTasks();
                                                       }
                                               """);
                    if (useTryCatch) {
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                           catch(Exception e) {
                                                               ShadowDebug.LogError(e);
                                                           }
                                                   """);
                        methodBodyBuilder.Append("\n");
                    }
                    else {
                        methodBodyBuilder.Append("\n");
                    }
                    methodBodyBuilder.Append($$"""
                                                       finally {
                                                           parallels?.Dispose();
                                                       }
                                                   }
                                               """);
                    
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   return;
                                               """);
                    
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   Hsenl.HTask GetParallelTasks() {
                                                        if (parallels != null)
                                                            return Hsenl.HTask.WaitAll(parallels);
                                                        
                                                        if (first0PriorityDel == null)
                                                            throw new NullReferenceException(nameof(first0PriorityDel));
                                                        
                                                        return first0PriorityDel.Value;
                                                   }
                                               """);
                
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                   bool Convert2Task(Delegate del, out Hsenl.HTask task) {
                                               """);
                    var ifcount = 0;
                    for (var i = 0; i < 2; i++) {
                        var hasThis = i == 1;
                        
                        if (methodSymbolInfo.methodSymbol.IsStatic && hasThis) {
                            continue;
                        }
                        
                        methodBodyBuilder.Append("\n");
                        methodBodyBuilder.Append($$"""
                                                                
                                                        """);
                        
                        if (ifcount > 0) methodBodyBuilder.Append("else ");
                        methodBodyBuilder.Append("if (del is global::System.");
                        var isFunc = methodSymbolInfo.methodSymbol.IsAsync || methodSymbolInfo.hasRet;
                        methodBodyBuilder.Append(isFunc ? "Func" : "Action");
                        if (hasThis || !string.IsNullOrEmpty(paramTypeNames) || isFunc) {
                            methodBodyBuilder.Append("<");
                            var hasPreceding = false;
                            if (hasThis) {
                                methodBodyBuilder.Append(typeSymbolInfo.displayString);
                                hasPreceding = true;
                            }
                        
                            if (!string.IsNullOrEmpty(paramTypeNames)) {
                                if (hasPreceding) methodBodyBuilder.Append(", ");
                                methodBodyBuilder.Append(paramTypeNames);
                                hasPreceding = true;
                            }
                        
                            if (methodSymbolInfo.hasRet || methodSymbolInfo.methodSymbol.IsAsync) {
                                if (hasPreceding) methodBodyBuilder.Append(", ");
                                methodBodyBuilder.Append(methodSymbolInfo.finalReturnName);
                            }
                        
                            methodBodyBuilder.Append(">");
                        }
                        
                        var argnum = ifcount != 0 ? ifcount.ToString() : null;
                        methodBodyBuilder.Append($" func{argnum}) {{");
                        methodBodyBuilder.Append("\n");
                        {
                            var space = "            ";
                            methodBodyBuilder.Append(space);
                            if (useTryCatch) {
                                methodBodyBuilder.Append($$"""
                                                           try {
                                                           """);
                                space += "    ";
                                methodBodyBuilder.Append("\n");
                                methodBodyBuilder.Append(space);
                            }

                            methodBodyBuilder.Append("task = ");
                            methodBodyBuilder.Append($"func{argnum}.Invoke(");
                            var hasPreceding = false;
                            if (hasThis) {
                                methodBodyBuilder.Append("this");
                                hasPreceding = true;
                            }
                            if (!string.IsNullOrEmpty(paramNames)) {
                                if (hasPreceding) methodBodyBuilder.Append(", ");
                                methodBodyBuilder.Append(paramNames);
                            }

                            methodBodyBuilder.Append(");");
                            methodBodyBuilder.Append("\n");
                            if (useTryCatch) {
                                space = space.Remove(space.Length - 4);
                                methodBodyBuilder.Append($$"""
                                                           {{space}}}
                                                           """);
                                methodBodyBuilder.Append("\n");
                                methodBodyBuilder.Append($$"""
                                                            {{space}}catch (Exception e) {
                                                            {{space}}    ShadowDebug.LogError(e);
                                                            {{space}}    task = default;
                                                            {{space}}    return false;
                                                            {{space}}}
                                                            """);
                                methodBodyBuilder.Append("\n");
                            }
                            methodBodyBuilder.Append($$"""
                                                                        
                                                            """);
                            methodBodyBuilder.Append("return true;");
                            methodBodyBuilder.Append("\n");
                            methodBodyBuilder.Append($$"""
                                                                    
                                                            """);
                            methodBodyBuilder.Append("}");
                        }
                        
                        ifcount++;
                    }
                    
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append("\n");
                    methodBodyBuilder.Append($$"""
                                                       task = default;
                                                       return false;
                                                   }
                                               """);
                }
                
                methodCodeBuilder.Body = methodBodyBuilder.ToString();
                if (methodSymbolInfo.methodSucc != "succ") 
                    methodCodeBuilder.Body += methodSymbolInfo.methodSucc;
                typeCodeBuilder.AppendMethodBuilder(methodCodeBuilder);
            }
    
            var source = typeCodeBuilder.ToString();
            context.AddSource($"{typeSymbolInfo.typeSymbol.MetadataName}.g.cs", source!);
        }
        
        private static void CreateShadowTypeCode(GeneratorExecutionContext context, TypeSymbolInfo typeSymbolInfo) {
            if (typeSymbolInfo.error != "succ")
                return;
            
            TypeCodeBuilder typeCodeBuilder = new();
            typeCodeBuilder.Namespace = typeSymbolInfo.typeSymbol.ContainingNamespace.ToDisplayString();
            typeCodeBuilder.DeclaredAccessibility = typeSymbolInfo.typeSymbol.DeclaredAccessibility;
            typeCodeBuilder.IsPartial = true;
            typeCodeBuilder.TypeKind = typeSymbolInfo.typeSymbol.TypeKind;
            typeCodeBuilder.Name = typeSymbolInfo.name;
            
            MethodCodeBuilder registerMethodCodeBuilder = new();
            registerMethodCodeBuilder.DeclaredAccessibility = Accessibility.Private;
            registerMethodCodeBuilder.IsStatic = typeSymbolInfo.typeSymbol.IsStatic;
            registerMethodCodeBuilder.Return = "void";
            registerMethodCodeBuilder.Name = "Register";
    
            MethodCodeBuilder unregisterMethodCodeBuilder = new();
            unregisterMethodCodeBuilder.DeclaredAccessibility = Accessibility.Private;
            unregisterMethodCodeBuilder.IsStatic = typeSymbolInfo.typeSymbol.IsStatic;
            unregisterMethodCodeBuilder.Return = "void";
            unregisterMethodCodeBuilder.Name = "Unregister";
            
            // 正式开始处理这些影子函数
            StringBuilder registerBodyBuilder = new();
            StringBuilder unregisterBodyBuilder = new();
            foreach (var methodSymbol in typeSymbolInfo.methodInfos) {
                var shadowMethodSymbolInfo = (ShadowMethodSymbolInfo)methodSymbol;
                
                registerBodyBuilder.Append($"""
                                                {ShadowFunctionSystemNamespace}.ShadowFunction.Register<global::System.
                                            """);
                
                var isFunc = shadowMethodSymbolInfo.methodSymbol.IsAsync || shadowMethodSymbolInfo.hasRet;
                registerBodyBuilder.Append(isFunc ? "Func" : "Action");
                
                // 合并参数
                string paramTypeNames = null;
                for (int i = 0, len = shadowMethodSymbolInfo.methodSymbol.Parameters.Length; i < len; i++) {
                    var last = i == len - 1;
                    var parameter = shadowMethodSymbolInfo.methodSymbol.Parameters[i];
                    paramTypeNames += parameter.Type.ToString();
                    if (!last) paramTypeNames += ", ";
                }
                
                // <> 泛型内容
                if (!string.IsNullOrEmpty(paramTypeNames) || isFunc) {
                    registerBodyBuilder.Append("<");
                    var hasPreceding = false;
                    
                    if (!string.IsNullOrEmpty(paramTypeNames)) {
                        registerBodyBuilder.Append(paramTypeNames);
                        hasPreceding = true;
                    }
            
                    if (shadowMethodSymbolInfo.hasRet || shadowMethodSymbolInfo.methodSymbol.IsAsync) {
                        if (hasPreceding) registerBodyBuilder.Append(", ");
                        registerBodyBuilder.Append(shadowMethodSymbolInfo.finalReturnName);
                    }
            
                    registerBodyBuilder.Append(">");
                }
            
                // () 参数内容
                string arg1;
                switch (FunctionKeyMode) {
                    case FuncKeyMode.String:
                        arg1 = $"""
                                       (int)global::System.HashCode.Combine("{typeSymbolInfo.sourceTypeDisplayStringWithoutGenericParam}", "{shadowMethodSymbolInfo.combinationName}")
                                """;
                        break;
                    case FuncKeyMode.HashCode:
                        arg1 = shadowMethodSymbolInfo.hashcode.ToString();
                        break;
                    case FuncKeyMode.Index:
                        arg1 = shadowMethodSymbolInfo.index.ToString();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                
                registerBodyBuilder.Append(">(");
                {
                    registerBodyBuilder.Append(arg1);
                    registerBodyBuilder.Append(", ");
                    registerBodyBuilder.Append($"\"{typeSymbolInfo.typeSymbol.ContainingAssembly.Name}\"");
                    registerBodyBuilder.Append(", ");
                    registerBodyBuilder.Append($"\"{typeSymbolInfo.typeSymbol.ToDisplayString()}\"");
                    registerBodyBuilder.Append(", ");
                    registerBodyBuilder.Append(shadowMethodSymbolInfo.priority != 0 ? shadowMethodSymbolInfo.priority.ToString() : typeSymbolInfo.sourceTypePriority.ToString());
                    registerBodyBuilder.Append(", ");
                    registerBodyBuilder.Append(shadowMethodSymbolInfo.finalMethodName);
                    registerBodyBuilder.Append(");");
                }
                
                unregisterBodyBuilder.Append($"""
                                                  {ShadowFunctionSystemNamespace}.ShadowFunction.Unregister(
                                              """);
                unregisterBodyBuilder.Append(arg1);
                unregisterBodyBuilder.Append(", ");
                unregisterBodyBuilder.Append($"\"{typeSymbolInfo.typeSymbol.ContainingAssembly.Name}\"");
                unregisterBodyBuilder.Append(", ");
                unregisterBodyBuilder.Append($"\"{typeSymbolInfo.typeSymbol.ToDisplayString()}\"");
                unregisterBodyBuilder.Append(", ");
                unregisterBodyBuilder.Append(shadowMethodSymbolInfo.priority != 0 ? shadowMethodSymbolInfo.priority.ToString() : typeSymbolInfo.sourceTypePriority.ToString());
                unregisterBodyBuilder.Append(");");
            
                // 现在错误交给分析器, 这里就不再标记错误了
                // if (shadowMethodSymbolInfo.methodSucc != "succ") registerBodyBuilder.Append(shadowMethodSymbolInfo.methodSucc); // 如果没找对应源函数, 在后加个标记, 故意让编译出错.
                registerBodyBuilder.Append("\n");
                unregisterBodyBuilder.Append("\n");
            }
            
            if (registerBodyBuilder.Length != 0) registerBodyBuilder.Remove(registerBodyBuilder.Length - 1, 1);
            registerMethodCodeBuilder.Body = registerBodyBuilder.ToString();
            typeCodeBuilder.AppendMethodBuilder(registerMethodCodeBuilder);
            
            if (registerBodyBuilder.Length != 0) unregisterBodyBuilder.Remove(unregisterBodyBuilder.Length - 1, 1);
            unregisterMethodCodeBuilder.Body = unregisterBodyBuilder.ToString();
            typeCodeBuilder.AppendMethodBuilder(unregisterMethodCodeBuilder);
            
            var source = typeCodeBuilder.ToString();
            context.AddSource($"{typeSymbolInfo.metadataName}.g.cs", source!);
        }
        
        private class ShadowFunctionReceiver : ISyntaxReceiver {
            // 挑选出的影子类, 后续只处理这些类
            public List<TypeDeclarationSyntax> Candidates { get; } = new();
    
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax) {
                    // 判断该类是否包含目标特性
                    var hasAtt = classDeclarationSyntax.AttributeLists.Any(list =>
                        list.Attributes.Any(x => {
                            if (x.Name is IdentifierNameSyntax identifierNameSyntax) {
                                if (identifierNameSyntax.Identifier.Text == ShadowFunctionAttributeShortName) {
                                    return true;
                                }
                            }
    
                            return false;
                        }));
    
                    if (hasAtt) {
                        this.Candidates.Add((TypeDeclarationSyntax)syntaxNode);
                    }
                }
            }
        }
        
        public class TypeSymbolInfo {
            public readonly IAssemblySymbol assemblySymbol; // 自己所在程序集
            public readonly INamedTypeSymbol typeSymbol; // 自己的类型
            public readonly AttributeData attributeData; // 自己的特性
            
            public INamedTypeSymbol? sourceTypeSymbol; // 自己的元类型(影子类才有元类)
            public int sourceTypePriority; // 元类的优先级
            public bool allowMultiShadowFuns; // 元类是否允许存在多个影子函数
    
            public string? displayString; // 看到的是什么名字, 就是什么名字, 包含命名空间
            public string? metadataName; // 一般情况下但name一样, 但如果是例如泛型的时候, 就是class`1这种元数据写法
            public string? name; // 只是自己的名字
            public string sourceTypeDisplayStringWithoutGenericParam; // 自己元类的直观名字, 像这种 Hsenl.Class<>
            
            public readonly List<MethodSymbolInfo> methodInfos = new();
    
            public string error = "succ";
    
            public bool IsSource => this.sourceTypeSymbol == null;
    
            public TypeSymbolInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData) {
                this.assemblySymbol = typeSymbol.ContainingAssembly;
                this.typeSymbol = typeSymbol;
                this.attributeData = attributeData;
            }
        }
        
        public class MethodSymbolInfo {
            public readonly TypeSymbolInfo typeSymbolInfo;
            public readonly IMethodSymbol methodSymbol;
            public readonly AttributeData attributeData;
            
            public bool allowMultiShadowFuns;
    
            public readonly string finalMethodName; // 单纯的名字, 不包括其他东西, 例如IInterface.Func()的最终名字为 Func
            public readonly string finalReturnName; // 最终名字, 比如当方法为async方法时, 如果返回值是void, 则自动替换成 HTask
            public readonly string combinationName; // 函数的组合名字(名字 + 参数 + 返回值的组合)
            public readonly int hashcode; // 该方法在整个进程的唯一编号, 由程序集名 + 所在类名 + 本身的组合名, 哈希合并而来
            public int index;
    
            public readonly bool hasRet;

            public string combinationNameIncludeParamName; // 函数的明文
            public string methodSucc = "succ";

            public bool RealAllowMultiShadowFuns => this.allowMultiShadowFuns || this.typeSymbolInfo.allowMultiShadowFuns;
                
            public MethodSymbolInfo(
                TypeSymbolInfo typeSymbolInfo,
                IMethodSymbol methodSymbol, 
                AttributeData attributeData,
                string finalMethodName, 
                string finalReturnName, 
                string combinationName, 
                int hashcode, 
                bool hasRet) {

                this.typeSymbolInfo = typeSymbolInfo;
                this.methodSymbol = methodSymbol;
                this.attributeData = attributeData;
                this.finalMethodName = finalMethodName;
                this.finalReturnName = finalReturnName;
                this.combinationName = combinationName;
                this.hashcode = hashcode;
                this.hasRet = hasRet;
            }
        }
        
        public class ShadowMethodSymbolInfo : MethodSymbolInfo {
            public readonly int priority;
            public IMethodSymbol? sourceMethodSymbol;
    
            public ShadowMethodSymbolInfo(
                TypeSymbolInfo typeSymbolInfo,
                IMethodSymbol methodSymbol,
                AttributeData attributeData,
                string finalMethodName,
                string finalReturnName,
                string combinationName,
                int hashcode,
                bool hasRet,
                int priority)
                : base(typeSymbolInfo, methodSymbol, attributeData, finalMethodName, finalReturnName, combinationName, hashcode, hasRet) {
                this.priority = priority;
            }
        }
    }
}