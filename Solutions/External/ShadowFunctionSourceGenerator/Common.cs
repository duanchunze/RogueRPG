using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowFunction;

public static class Common {
    // private static readonly SHA256 _sha256 = SHA256.Create(); // 因为分析器是多线程, 所以都不提前缓存了, 比如stringbuilder, 还有这个, 多点gc无所谓

    public static int HashCodeCombine(string str1, string str2) {
        // 简单的合并哈希
        unchecked {
            var h1 = str1?.GetHashcode() ?? 0;
            var h2 = str2?.GetHashcode() ?? 0;
            var hashcode = (h1 * 397) ^ h2;
            return hashcode;
        }
    }
    
    public static int HashCodeCombine(string str1, string str2, string str3) {
        // 简单的合并哈希
        unchecked {
            var h1 = str1?.GetHashcode() ?? 0;
            var h2 = str2?.GetHashcode() ?? 0;
            var h3 = str3?.GetHashcode() ?? 0;
            var hashcode = (h1 * 397) ^ h2;
            hashcode = (hashcode * 397) ^ h3;
            return hashcode;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetHashcode(this string self) {
        var _sha256 = SHA256.Create();
        byte[] hashBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(self));
        // 从哈希字节数组中提取一个确定性的整数（这里仅使用部分字节以适应int大小）
        int hashInt = BitConverter.ToInt32(hashBytes, 0);
        return hashInt;
    }

    public static void AssertNullReference<T>(T t, string? message = null) where T : class? {
        if (t == null) throw new NullReferenceException(null);
    }

    /// <summary>
    /// 获取自己和父类里包含的所有的成员, 自动避免重复成员
    /// 如果父类是在另一个程序集的话, 那么只能获取到非private的成员.
    /// 如果类型是一个class<>的话, 也找不到任何成员
    /// </summary>
    /// <returns></returns>
    public static ISymbol[] GetMembersInBase(ITypeSymbol typeSymbol) {
        var hashset = new HashSet<ISymbol>(new ISymbolComparer());
        foreach (var member in typeSymbol.GetMembers()) {
            hashset.Add(member);
        }

        var basicTypeSymbol = typeSymbol.BaseType;
        while (basicTypeSymbol != null) {
            if (basicTypeSymbol.SpecialType == SpecialType.System_Object)
                break;

            foreach (var member in basicTypeSymbol.GetMembers()) {
                hashset.Add(member);
            }

            basicTypeSymbol = basicTypeSymbol.BaseType;
        }

        return hashset.ToArray();
    }

    public static string GetSourceFunctionManifestCarrierMetadataName_HashCode(string assemblyName) {
        return $"{assemblyName}.SFS_DC_SourceFunctionManifestCarrier_HashCode";
    }
    
    public static string GetSourceFunctionManifestCarrierMetadataName_Plaintext(string assemblyName) {
        return $"{assemblyName}.SFS_DC_SourceFunctionManifestCarrier_Plaintext";
    }
    
    public static string GetSourceFunctionManifestCarrierMetadataName_HashCodeToIndex(string assemblyName) {
        return $"{assemblyName}.SFS_DC_SourceFunctionManifestCarrier_HashCodeToIndex";
    }

    // 获取一个类型的名字, 不带泛型参数的版本
    public static string GetTypeDisplayNameWithoutGenericParams(INamedTypeSymbol typeSymbol) {
        var name = typeSymbol.ToDisplayString();
        if (typeSymbol.IsGenericType) {
            // 如果是泛型的话, 就使用Class<>的形式
            StringBuilder _stringBuilder = new();
            for (int i = 1; i < typeSymbol.TypeArguments.Length; i++) {
                _stringBuilder.Append(',');
            }

            var match = Regex.Replace(name, @"(?<=<).*(?=>)", _ => _stringBuilder.ToString());
            name = match;
        }

        return name;
    }

    // 拆解一个源函数(得到该函数符合我们规则的信息, 最终名字、最终返回值等)
    public static void DeconstructSourceFunction(IMethodSymbol methodSymbol, out bool hasReturn, out string finalName, out string finalReturn) {
        // 最终的名字, 不包含任何其他内容
        finalName = methodSymbol.Name;
        if (finalName.Contains('.')) {
            finalName = finalName.Substring(finalName.LastIndexOf(".", StringComparison.Ordinal) + 1);
        }

        // 最终的返回值
        var returnTypeSymbol = (INamedTypeSymbol)methodSymbol.ReturnType;
        finalReturn = returnTypeSymbol.ToDisplayString();
        if (methodSymbol.IsAsync) {
            hasReturn = returnTypeSymbol.IsGenericType;
            if (finalReturn == "void")
                finalReturn = "Hsenl.HTask";
        }
        else {
            hasReturn = finalReturn != "void";
        }
    }

    // 拆解一个影子函数(同上)
    public static void DeconstructShadowFunction(IMethodSymbol methodSymbol, ITypeSymbol sourceTypeSymbol, out bool hasReturn, out bool self,
        out string finalName, out string finalReturn) {
        // 根据影子的第一个参数判断其有没有使用self参数
        var firstParamType = methodSymbol.Parameters.FirstOrDefault()?.Type;
        // 第一个参数为源类的话, 就是使用了self参数
        self = false;
        if (firstParamType != null) {
            if (((INamedTypeSymbol)firstParamType).IsGenericType) {
                string fullTypeNameWithoutGenerics1 = firstParamType.ToDisplayString(new SymbolDisplayFormat(
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                    genericsOptions: SymbolDisplayGenericsOptions.None // 不包含泛型参数
                ));
                string fullTypeNameWithoutGenerics2 = sourceTypeSymbol.ToDisplayString(new SymbolDisplayFormat(
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                    genericsOptions: SymbolDisplayGenericsOptions.None // 不包含泛型参数
                ));

                self = fullTypeNameWithoutGenerics1 == fullTypeNameWithoutGenerics2;
            }
            else {
                self = firstParamType.ToDisplayString() == sourceTypeSymbol.ToDisplayString();
            }
        }

        // 最终的名字, 不包含任何其他内容
        finalName = methodSymbol.Name;

        // 最终的返回值
        var returnTypeSymbol = (INamedTypeSymbol)methodSymbol.ReturnType;
        finalReturn = returnTypeSymbol.ToDisplayString();
        if (methodSymbol.IsAsync) {
            hasReturn = returnTypeSymbol.IsGenericType;
        }
        else {
            hasReturn = finalReturn != "void";
        }
    }
    
    // 构建一个函数的唯一码
    public static int ConstructMethodHashCode(string assemblyName, string sourceTypeDisplayStringWithoutGenericParam, string methodCombinationName) {
        return HashCodeCombine(assemblyName, sourceTypeDisplayStringWithoutGenericParam, methodCombinationName);
    }
    
    // 构建一个函数的组合名(把上面拆解后的信息组合成一个整体)
    public static string ConstructMethodCombinationName(IMethodSymbol methodSymbol, int argStartIndex, string methodName, string returnName) {
        StringBuilder _stringBuilder = new();
        _stringBuilder.Append(methodName); // 使用用户提供的方法名字
        for (int i = argStartIndex, len = methodSymbol.Parameters.Length; i < len; i++) {
            var parameterSymbol = methodSymbol.Parameters[i];
            _stringBuilder.Append("-");
            _stringBuilder.Append(parameterSymbol.Type.ToDisplayString());
        }

        _stringBuilder.Append($"-{returnName}");

        return _stringBuilder.ToString();
    }

    // 构建一个函数的组合名(包含参数名的版本)
    public static string ConstructMethodCombinationNameIncludeParaName(IMethodSymbol methodSymbol, int argStartIndex, string methodName, string returnName) {
        StringBuilder _stringBuilder = new();
        _stringBuilder.Append(methodName); // 使用用户提供的方法名字
        for (int i = argStartIndex, len = methodSymbol.Parameters.Length; i < len; i++) {
            var parameterSymbol = methodSymbol.Parameters[i];
            _stringBuilder.Append("-");
            _stringBuilder.Append($"{parameterSymbol.Type.ToDisplayString()} {parameterSymbol.Name}");
        }

        _stringBuilder.Append($"-{returnName}");

        return _stringBuilder.ToString();
    }

    // 通过组合名拆解一个函数(把上面组合成的一个整体给拆开, 上面两个方法组合的都能拆)
    public static bool DeconstructMethodCombinationName(string uniqueName, out string methodReturn, out string methodName, out List<string> paramlist) {
        string[] splits = uniqueName.Split('-');
        if (splits.Length == 1) {
            methodReturn = null!;
            methodName = null!;
            paramlist = null!;
            return false;
        }

        // 开头是名字, 最后是返回值, 中间的都是参数
        methodName = splits[0];
        methodReturn = splits[splits.Length - 1];
        paramlist = new List<string>();
        for (int i = 1, len = splits.Length - 1; i < len; i++) {
            paramlist.Add(splits[i]);
        }

        return true;
    }

    // 获得一个特性的参数, 根据索引号
    public static T? GetArgValueInAttributeData<T>(AttributeData attributeData, int index) {
        T? result = default;
        for (int i = 0, len = attributeData.ConstructorArguments.Length; i < len; i++) {
            if (i != index) continue;
            var arg = attributeData.ConstructorArguments[i];
            if (arg.Value is T t) {
                result = t;
            }
        }

        // if (result == null) {
        //     for (int i = 0, len = attributeData.NamedArguments.Length; i < len; i++) {
        //         if (i != index) continue;
        //         var arg = attributeData.NamedArguments[i].Value;
        //         if (arg.Value is T t) {
        //             result = t;
        //         }
        //     }
        // }

        return result;
    }

    // 解析源函数信息清单
    public static bool ParseSourceFunctionManifest(INamedTypeSymbol manifestCarrierTypeSymbol, char split1, char split2,
        out Dictionary<string, HashSet<string>> dict) {
        var manifestAttr = manifestCarrierTypeSymbol.GetAttributes().FirstOrDefault();
        if (manifestAttr == null) {
            dict = null!;
            return false;
        }

        dict = new Dictionary<string, HashSet<string>>();
        var manifestData = GetArgValueInAttributeData<string>(manifestAttr, 0);
        if (string.IsNullOrEmpty(manifestData))
            return true;

        foreach (var split in manifestData!.Split(split1)) {
            var subSplits = split.Split(split2); // 拆分
            if (subSplits.Length != 0) {
                var hashset = new HashSet<string>();
                for (int i = 1; i < subSplits.Length; i++) {
                    hashset.Add(subSplits[i]);
                }

                dict[subSplits[0]] = hashset;
            }
        }

        return true;
    }

    public static bool ParseSourceFunctionManifest_HashcodeToIndex(INamedTypeSymbol manifestCarrierTypeSymbol, out Dictionary<string, string> dict) {
        var manifestAttr = manifestCarrierTypeSymbol.GetAttributes().FirstOrDefault();
        if (manifestAttr == null) {
            dict = null!;
            return false;
        }
        
        dict = new Dictionary<string, string>();
        var manifestData = GetArgValueInAttributeData<string>(manifestAttr, 0);
        if (string.IsNullOrEmpty(manifestData))
            return true;
        
        foreach (var split in manifestData!.Split(';')) {
            if (string.IsNullOrEmpty(split))
                continue;
            
            var subSplits = split.Split(','); // 拆分
            if (subSplits.Length != 0) {
                dict.Add(subSplits[0], subSplits[1]);
            }
        }

        return true;
    }

    public static TypeDeclarationSyntax? GetMethodSyntaxContainingType(MethodDeclarationSyntax methodDeclarationSyntax) {
        // 首先检查当前方法是否直接位于类或结构体等类型的成员列表中
        var parent = methodDeclarationSyntax.Parent;

        if (parent is TypeDeclarationSyntax typeDeclaration) {
            // 如果是，可以直接获取类型名称
            return typeDeclaration;
        }

        // 如果不是直接位于类型声明下，可能是在嵌套类型或者是一个成员内部，需要向上遍历查找
        while (parent != null) {
            if (parent is TypeDeclarationSyntax containingType) {
                // 找到了包含当前方法的类型声明
                return containingType;
            }

            if (parent is MemberDeclarationSyntax member && member.Parent is TypeDeclarationSyntax outerType) {
                // 继续向上寻找外层类型
                parent = outerType;
            }
            else {
                // 继续向上遍历语法树
                parent = parent.Parent;
            }
        }

        return null;
    }
}

public class ISymbolComparer : IEqualityComparer<ISymbol> {
    public bool Equals(ISymbol x, ISymbol y) {
        return this.GetHashCode(x) == this.GetHashCode(y);
    }

    public int GetHashCode(ISymbol obj) {
        unchecked {
            // 我们只判断Kind和MetadataName, 只要这两个相同, 我们就认为他是同一个函数
            var hashCode = (int)obj.Kind;
            hashCode = (hashCode * 397) ^ obj.MetadataName.GetHashCode();
            return hashCode;
        }
    }
}