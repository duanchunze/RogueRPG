using System.Runtime.CompilerServices;

namespace Hsenl {
    // 公共逻辑定义, 定义一些标准, 例如技能的持有者, 就是他的ParentSubstantive
    public static class CommonLogicDefine {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Substantive GetHolder(this Ability self) {
            return self.ParentSubstantive?.ParentSubstantive;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Substantive GetHolder(this Status self) {
            return self.ParentSubstantive?.ParentSubstantive;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Substantive GetHolder(this CardBar self) {
            return self.ParentSubstantive;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Substantive GetHolder(this CardBackpack self) {
            return self.ParentSubstantive;
        }
    }
}