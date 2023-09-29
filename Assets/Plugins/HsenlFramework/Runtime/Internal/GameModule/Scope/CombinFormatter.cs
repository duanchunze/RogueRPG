using System;
using System.Collections.Generic;

namespace Hsenl {
    [FrameworkMember]
    public abstract class CombinFormatter {
        internal static MultiList<Type, CrossCombinFormatter> CrossCombinFormatters { get; } = new();

        // 组合方式
        // 1、先判断该type是不是有formatter, 有则覆盖掉默认的和父级匹配的方案
        // 2、如果有

        [OnEventSystemInitialized]
        private static void Init() {
            CrossCombinFormatters.Clear();
            foreach (var type in AssemblyHelper.GetSubTypes(typeof(CombinFormatter), EventSystem.GetAssemblies())) {
                var combinFormatter = (CombinFormatter)Activator.CreateInstance(type);
                var formatters = combinFormatter.GetCrossCombinFormatters();
                foreach (var kv in formatters) {
                    var crossCombinFormatter = new CrossCombinFormatter();
                    CrossCombinFormatters.Add(kv.Key, crossCombinFormatter);
                    foreach (var t in kv.Value) {
                        crossCombinFormatter.types.Add(t);
                    }
                }
            }
        }

        /// <summary>
        /// 跨域组合默认只和自己的父域进行匹配, 如果想继续向上匹配, 则需要特别指定组合格式
        /// 如果指定了组合格式, 则会覆盖掉默认格式, 默认格式就是只和自己的父域做匹配
        /// 格式: key: 目标subs, value: 指定向上组合的路径. null代表在该层, 任何父域都满足
        /// </summary>
        /// <returns></returns>
        protected abstract MultiList<Type, Type> GetCrossCombinFormatters();

        public class CrossCombinFormatter {
            public bool succ; // 匹配是否暂时成功("暂时"是因为每一层都要进行一次判断)
            public readonly List<Type> types = new();
        }
    }
}