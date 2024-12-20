﻿using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    // UnbodiedHead 的存在就是为了找bodied, 而且是作为一种补充, 正常情况, 用户应该先给entity添加一个bodied, 然后再添加unbodied,
    // 而如果我们没添加的话, 那系统就会默认添加一个UnbodiedHead
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class UnbodiedHead : Scope, IBodied, IUnbodiedHead {
        [MemoryPackIgnore]
        private Bodied _bodied;

        [MemoryPackIgnore]
        public Bodied Bodied {
            get => this._bodied;
            private set {
                if (this._bodied == value)
                    return;

                this._bodied = value;

                if (this.childrenScopes == null)
                    return;

                for (int i = 0, len = this.childrenScopes.Count; i < len; i++) {
                    var childScope = this.childrenScopes[i];
                    if (childScope is UnbodiedHead unbodiedHead) {
                        unbodiedHead.Bodied = value;
                    }
                }
            }
        }

        [MemoryPackIgnore]
        public Bodied MainBodied => this.Bodied?.MainBodied;

        public override Scope ParentScope {
            get => this.parentScope;
            internal set {
                if (this.parentScope == value)
                    return;

                this.OnBeforeParentScopeChangeInternal(value);

                // 父子关系确定
                var prevParent = this.parentScope;
                this.parentScope = value;

                if (prevParent != null) {
                    if (!prevParent.childrenScopes.Remove(this)) throw new Exception("???");
                    if (prevParent.childrenScopes.Count == 0)
                        prevParent.childrenScopes = null;
                }

                if (value != null) {
                    value.childrenScopes ??= new();
                    value.childrenScopes.Add(this);
                }

                if (prevParent != null) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        CrossDecombinMatchForParents(this, prevParent);
                    }

                    this.ForeachChildrenScope((childScope, _, p) => {
                        if (childScope.CombinMatchMode == CombinMatchMode.Auto) {
                            CrossDecombinMatchForParents(childScope, p); //
                        }
                    }, data: prevParent);
                }

                // 重写添加了这句 -> 确定父子关系后, head要处理的就是, 找到自己的bodied
                this.Bodied = this.FindScopeInParent<Bodied>();

                // 确立好父子关系后再进行跨域匹配, 保证形成组合的时候, 父子关系是正确的.
                if (value != null) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        this.CalcMaximumCrossLayerInTheory();
                        CrossCombinsMatchForParents(this, value, 1, null);
                    }

                    this.ForeachChildrenScope((childScope, layer, p) => {
                        if (childScope.CombinMatchMode == CombinMatchMode.Auto) {
                            childScope.CalcMaximumCrossLayerInTheory();
                            CrossCombinsMatchForParents(childScope, p, layer + 1, null); // 
                        }
                    }, data: value);
                }

                this.OnParentScopeChangedInternal(prevParent);
                prevParent?.OnChildScopeRemoveInternal(this);
                value?.OnChildScopeAddInternal(this);
            }
        }

        internal override void OnDeserializedInternal() {
            this.Entity.ForeachComponents((component, unbodiedHead) => {
                if (component is not Unbodied unbodied)
                    return;

                unbodied.unbodiedHead = unbodiedHead;
            }, this);
        }

        internal override void OnDisposedInternal() {
            base.OnDisposedInternal();
            this._bodied = null;
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Scope)
                throw new Exception($"one entity only one scope. '{this.Name}' '{component.GetType().Name}'");

            if (component is not Unbodied unbodied)
                return;

            unbodied.unbodiedHead = this;

            if (this.CombinMatchMode != CombinMatchMode.Manual) {
                MultiCombinMatch(this, unbodied);
            }

            if (Combiner.CombinerCache.CrossCombinLookupTable.TryGetValue(unbodied.ComponentIndex, out var combinInfo)) {
                if (combinInfo.combiners1.Count != 0) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        if (this.ParentScope != null) {
                            if (this.HasComponentsAny(combinInfo.totalTypeCacher1)) {
                                foreach (var combiner in combinInfo.combiners1) {
                                    CrossCombinMatchForParent(this, this.ParentScope, 1, combiner);
                                }
                            }
                        }
                    }
                }
                
                if (combinInfo.combiners2.Count != 0) {
                    if (this.HasComponentsAny(combinInfo.totalTypeCacher2)) {
                        this.ForeachChildrenScope<(CrossCombinInfo ci, Scope p)>((child, layer, data) => {
                            if (child.CombinMatchMode == CombinMatchMode.Auto) {
                                foreach (var combiner in data.ci.combiners2) {
                                    CrossCombinMatchForParent(child, data.p, layer, combiner);
                                }
                            }
                        }, data: (combinInfo, this));
                    }
                }
            }
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is not Unbodied unbodied)
                return;

            unbodied.unbodiedHead = null;

            if (this.CombinMatchMode != CombinMatchMode.Manual) {
                MultiDecombinMatch(this, unbodied);
            }

            if (this.CombinMatchMode == CombinMatchMode.Auto) {
                CrossDecombinMatchByComponent(this, unbodied);
            }
        }
    }
}