using System;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    // 无形的, 比如数值组件, 伤害组件, 控制器组件, 这些都是无形的
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Unbodied : Component, IUnbodied {
        [MemoryPackIgnore]
        [ShowInInspector]
        private Substantive _substantive;

        [MemoryPackIgnore]
        public Substantive Substantive {
            get {
                if (this._substantive == null) {
                    if (this.Parent != null) {
                        this._substantive = this.GetComponentInParent<Substantive>();
                    }
                }

                return this._substantive;
            }
            internal set => this._substantive = value;
        }

        internal override void OnParentChangedInternal(Entity previousParent) {
            // 当unbodied移动父级时, 如果自己没有subs, 就从父级去找
            // unbodied应该要有一个subs, 如果自己本身没有, 就把父实体的subs当做自己的subs, 如果还是没有, 才作罢.
            var subs = this.GetComponent<Substantive>();
            if (subs == null) {
                subs = this.Parent?.GetComponentInParent<Substantive>();
            }

            this.Substantive = subs;
        }

        internal override void OnComponentAddInternal(Component component) {
            if (component is Substantive substantive)
                this.Substantive = substantive;
        }

        internal override void OnComponentRemoveInternal(Component component) {
            if (component is Substantive substantive) {
                if (this.Substantive != substantive) {
                    // 因为移除的时候不是自己, 那添加的时候添加的那个是谁呢? 理论上只要如果自身上有sub, unb的sub一定是这个sub
                    // 出现这种情况可能是因为OnComponentRemove事件被重复触发了, 只可能是框架的问题, 因为这块用户接触不到
                    throw new Exception("errors that should not occur");
                }

                this.Substantive = null;
            }
        }

        internal void OnBeforeParentSubstantiveChangeInternal(Substantive futrueParent) {
            try {
                this.OnBeforeParentSubstantiveChange(futrueParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void OnAfterParentSubstantiveChangedInternal(Substantive previousParent) {
            try {
                this.OnAfterParentSubstantiveChanged(previousParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
        
        internal void OnChildSubstantiveAddInternal(Substantive childSubs) {
            try {
                this.OnChildSubstantiveAdd(childSubs);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void OnChildSubstantiveRemoveInternal(Substantive childSubs) {
            try {
                this.OnChildSubstantiveRemove(childSubs);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnBeforeParentSubstantiveChange(Substantive futrueParentSubs) { }
        
        protected virtual void OnAfterParentSubstantiveChanged(Substantive previousParentSubs) { }
        
        protected virtual void OnChildSubstantiveAdd(Substantive childSubs) { }
        
        protected virtual void OnChildSubstantiveRemove(Substantive childSubs) { }
    }
}