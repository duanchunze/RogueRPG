using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class Selector : Unbodied {
        // 选择系统分为以下几个部分
        // 搜寻器: 从场景中搜寻符合条件的对象. 参数: 位置、范围 等自定义参数
        // 过滤器: 把上面的按照条件过滤出来. 参数: 标签、死亡与否等自定义参数
        // 选择器: 从上面过滤后的对象中, 选择出一个最终目标. 参数: 选择方案(最近的、血最少的等)
        // 上面三个均带过期判断
        // 该系统提供了一些快捷操作, 也可以自行选择自定义操作, 基本可以满足任何情况下的选择需求

        private Dictionary<Type, ASelectionsSearcher> _searcherCaches = new();
        private Dictionary<Type, ASelectionsFilter> _filterCaches = new();
        private Dictionary<Type, ASelectionsSelector> _selectorCaches = new();

        private SelectionTarget _primarySelection;

        public SelectionTarget PrimarySelection {
            get {
                if (this._primarySelection is { IsDisposed: true }) {
                    this._primarySelection = null;
                }

                return this._primarySelection;
            }
            set => this._primarySelection = value;
        }

        protected override void OnDisable() {
            this.PrimarySelection = null;
        }

        public T GetSearcher<T>() where T : ASelectionsSearcher, new() {
            if (!this._searcherCaches.TryGetValue(typeof(T), out var result)) {
                result = new T();
                this._searcherCaches[typeof(T)] = result;
            }

            result.selector = this;
            return (T)result;
        }

        public T GetFilter<T>() where T : ASelectionsFilter, new() {
            if (!this._filterCaches.TryGetValue(typeof(T), out var result)) {
                result = new T();
                this._filterCaches[typeof(T)] = result;
            }

            result.selector = this;
            return (T)result;
        }

        public T GetSelector<T>() where T : ASelectionsSelector, new() {
            if (!this._selectorCaches.TryGetValue(typeof(T), out var result)) {
                result = new T();
                this._selectorCaches[typeof(T)] = result;
            }

            result.selector = this;
            return (T)result;
        }
    }
}