using System;
using System.Collections.Generic;

namespace Hsenl {
    // 当前游戏的一些算法
    public static class GameAlgorithm {
        // 计算数值
        public static Num CalculateNumeric(Numerator numerator, NumericNode node, NumericType numericType) {
            if (numerator == null) {
                return Num.Empty();
            }
            
            if (node == null) {
                return numerator.GetValue(numericType);
            }
        
            var value = numerator.CalculateValue(node, numericType, true);
            return value;
        }

        // 合并计算数值.
        // 因为现在游戏的规则是技能可以装备自己的辅助技能, 该辅助技能只影响当前的技能, 所以技能也需要有一个自己的Numerator来作为数值Hub, 所以需要采用这种合并计算的方式
        public static Num MergeCalculateNumeric(Numerator num1, Numerator num2, NumericType numericType) {
            if (num1 == null) {
                return num2.GetValue(numericType);
            }

            if (num2 == null) {
                return num1.GetValue(numericType);
            }

            var value = num1.MergeCalculateValue(num2, numericType, true);
            return value;
        }

        public static Num MergeCalculateNumeric(IList<Numerator> numerators, NumericType numericType) {
            var value = Numerator.MergeCalculateValue(numerators, numericType, true);
            return value;
        }
    }
}