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
        public Bodied Owner => this.Bodied?.Owner;

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
                    CrossDecombinMatchForParent(this, prevParent);
                    this.ForeachChildrenScope((childScope, _) => {
                        CrossDecombinMatchForParent(childScope, prevParent); // 
                    });
                }

                // 确定父子关系后, head要处理的就是, 找到自己的bodied
                this.Bodied = this.FindScopeInParent<Bodied>();

                if (value != null) {
                    this.CalcMaximumCrossLayerInTheory();
                    CrossCombinMatchForParent(this, value, 1);

                    this.ForeachChildrenScope((childScope, layer) => {
                        childScope.CalcMaximumCrossLayerInTheory();
                        CrossCombinMatchForParent(childScope, value, layer + 1); //
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

                this.elements.Add(component.ComponentIndex, unbodied);
            });
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Scope) throw new Exception($"one entity only one scope. '{this.Name}' '{component.GetType().Name}'");
            if (component is not Unbodied unbodied)
                return;

            if (!this.elements.TryAdd(component.ComponentIndex, unbodied)) {
                throw new Exception($"this component is alrealy has in scope. '{this.GetType().Name}' '{component.GetType().Name}'");
            }

            unbodied.unbodiedHead = this;

            MultiCombinMatch(this, component);
            CrossCombinMatch(this, component);
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is not Unbodied unbodied)
                return;

            this.elements.Remove(component.ComponentIndex);

            unbodied.unbodiedHead = null;

            MultiDecombinMatch(this, component);
            CrossDecombinMatch(this, component);
        }
    }
}