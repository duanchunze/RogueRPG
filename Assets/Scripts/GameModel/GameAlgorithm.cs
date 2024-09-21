using System;
using System.Collections.Generic;

namespace Hsenl {
    // 当前游戏的一些算法
    public static class GameAlgorithm {
        // 计算数值
        public static Num CalculateNumeric(Numerator numerator, NumericNode node, NumericType numericType) {
            if (numerator == null) throw new ArgumentException(nameof(numerator));
            if (node == null) throw new ArgumentException(nameof(node));

            var value = numerator.CalculateValue(node, numericType, true);
            return value;
        }

        // 合并计算数值.
        // 释放技能时的属性计算是, 人物的属性与技能的属性合并, 得出最终的属性, 比如人物的当前的攻击距离为1, 技能的攻击距离为2, 那么该人物用该技能的攻击距离就是3.
        // 之所以要合并而不是把技能的属性附加到人物身上, 因为这个属性是临时的, 如果直接附加到人物身上, 那其他技能也会受到影响, 装备的属性可以附加到人物身上, 但技能不行.
        public static Num MergeCalculateNumeric(Numerator num1, Numerator num2, NumericType numericType) {
            if (num1 == null) throw new ArgumentException(nameof(num1));
            if (num2 == null) throw new ArgumentException(nameof(num2));

            var value = num1.MergeCalculateValue(num2, numericType, true);
            return value;
        }

        public static Num MergeCalculateNumeric(IList<Numerator> numerators, NumericType numericType) {
            var value = Numerator.MergeCalculateValue(numerators, numericType, true);
            return value;
        }
    }
}