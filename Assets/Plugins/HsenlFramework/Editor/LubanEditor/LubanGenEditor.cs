using System.Diagnostics;
using UnityEditor;

namespace Hsenl {
    public static class LubanGenEditor {
        [MenuItem("Hsenl/GenLuban")]
        private static void Gen() {
            ProcessHelper.Run("gen.bat", "", "Luban/ExcelConfig/");
        }
    }
}