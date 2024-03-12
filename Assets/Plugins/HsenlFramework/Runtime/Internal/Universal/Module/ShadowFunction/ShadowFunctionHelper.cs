using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Hsenl {
    public static class ShadowFunctionHelper {
        private static string GetDisplayName(string name) {
            return name switch {
                "System.Byte" => "byte",
                "System.Char" => "char",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.Single" => "float",
                "System.Double" => "double",
                "System.String" => "string",
                "System.Void" => "void",
                _ => name
            };
        }
    }
}