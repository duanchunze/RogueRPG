using System;
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
        public Bodied AttachedBodied => this.Bodied?.AttachedBodied;

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

                    this.ForeachChildrenScope((childScope, _) => {
                        if (childScope.CombinMatchMode == CombinMatchMode.Auto) {
                            CrossDecombinMatchForParents(childScope, prevParent); //
                        }
                    });
                }

                // 重写添加了这句 -> 确定父子关系后, head要处理的就是, 找到自己的bodied
                this.Bodied = this.FindScopeInParent<Bodied>();

                // 确立好父子关系后再进行跨域匹配, 保证形成组合的时候, 父子关系是正确的.
                if (value != null) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        this.CalcMaximumCrossLayerInTheory();
                        CrossCombinsMatchForParents(this, value, 1, null);
                    }

                    this.ForeachChildrenScope((childScope, layer) => {
                        if (childScope.CombinMatchMode == CombinMatchMode.Auto) {
                            childScope.CalcMaximumCrossLayerInTheory();
                            CrossCombinsMatchForParents(childScope, value, layer + 1, null); // 
                        }
                    });
                }

                this.OnParentScopeChangedInternal(prevParent);
                prevParent?.OnChildScopeRemoveInternal(this);
                value?.OnChildScopeAddInternal(this);
            }
        }

        internal override void OnDeserializedInternal() {
            this.Entity.ForeachComponents(component => {
                if (component is not Unbodied unbodied)
                    return;

                unbodied.unbodiedHead = this;
            });
        }

        protected internal override void OnDisposed() {
            base.OnDisposed();
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
                if (combinInfo.childCombiners.Count != 0) {
                    if (this.CombinMatchMode == CombinMatchMode.Auto) {
                        if (this.ParentScope != null) {
                            if (this.HasComponentsAny(combinInfo.totalChildTypeCacher)) {
                                foreach (var combiner in combinInfo.childCombiners) {
                                    CrossCombinMatchForParent(this, this.ParentScope, 1, combiner);
                                }
                            }
                        }
                    }
                }

                if (combinInfo.parentCombiners.Count != 0) {
                    if (this.HasComponentsAny(combinInfo.totalParentTypeCacher)) {
                        this.ForeachChildrenScope((child, layer) => {
                            if (child.CombinMatchMode == CombinMatchMode.Auto) {
                                foreach (var combiner in combinInfo.parentCombiners) {
                                    CrossCombinMatchForParent(child, this, layer, combiner);
                                }
                            }
                        });
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