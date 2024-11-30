using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class Selector : Unbodied {
        // 选择系统分为以下几个部分
        // 搜寻器: 从场景中搜寻符合条件的对象. 参数: 位置、范围 等自定义参数
        // 过滤器: 把上面的按照条件过滤出来. 参数: 标签、死亡与否等自定义参数
        // 选择器: 从上面过滤后的对象中, 选择出一个最终目标. 参数: 选择方案(最近的、血最少的等)
        // 该系统提供了一些快捷操作, 也可以自行选择自定义操作, 基本可以满足任何情况下的选择需求

        private static Dictionary<Type, ASelectionsSearcher> _searchers = new();
        private static Dictionary<Type, ASelectionsFilter> _filters = new();
        private static Dictionary<Type, ASelectionsSelect> _selects = new();

        public T GetSearcher<T>() where T : ASelectionsSearcher, new() {
            if (!_searchers.TryGetValue(typeof(T), out var result)) {
                result = new T();
                _searchers[typeof(T)] = result;
            }

            result.Selector = this;
            return (T)result;
        }

        public T GetFilter<T>() where T : ASelectionsFilter, new() {
            if (!_filters.TryGetValue(typeof(T), out var result)) {
                result = new T();
                _filters[typeof(T)] = result;
            }

            result.Selector = this;
            return (T)result;
        }

        public T GetSelect<T>() where T : ASelectionsSelect, new() {
            if (!_selects.TryGetValue(typeof(T), out var result)) {
                result = new T();
                _selects[typeof(T)] = result;
            }

            result.Selector = this;
            return (T)result;
        }
    }
}