using System;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class CombinerAttribute : BaseAttribute {
        public readonly CombinerType combinerType;
        public readonly int splitPosition = -1; // 该值定义了一个拆分位置, 该位置前为主类型, 该位置以及其后面, 为次类型

        public CombinerAttribute(CombinerType combinerType) {
            this.combinerType = combinerType;
        }

        /// <summary>
        /// arg1, arg2, arg3 假如想分成 (arg1, arg2), arg3, 填2
        /// </summary>
        /// <param name="splitPosition"></param>
        /// <exception cref="ArgumentException"></exception>
        public CombinerAttribute(int splitPosition) {
            if (splitPosition <= 0) throw new ArgumentException($"split position must more than 0 '{splitPosition}'");
            this.combinerType = CombinerType.CrossCombiner;
            this.splitPosition = splitPosition;
        }
    }
}