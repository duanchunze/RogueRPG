using System;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    /*
     * 这个择敌器的逻辑类似传统RPG的逻辑, 例如wow
     */
    [Serializable]
    [MemoryPackable]
    public partial class SelectorDefault : Selector {
        private SelectionTargetDefault _primaryTarget; // 主要目标, 比如用户主动选择的目标
        private SelectionTargetDefault _forceTarget; // 强制目标, 比如受到嘲讽时

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public SelectionTargetDefault PrimaryTarget {
            get {
                if (this._primaryTarget is { IsDisposed: true }) {
                    this._primaryTarget = null;
                }

                return this._primaryTarget;
            }
            set {
                if (this._primaryTarget == value)
                    return;

                if (this._primaryTarget != null) {
                    this._primaryTarget.RemoveSelector(this);
                }

                this._primaryTarget = value;
                value?.AddSelector(this);
            }
        }

        public SelectionTargetDefault ForceTarget {
            get {
                if (this._forceTarget is { IsDisposed: true }) {
                    this._forceTarget = null;
                }

                return this._forceTarget;
            }
            set { this._forceTarget = value; }
        }

        public SelectionTargetDefault TrueTarget {
            get {
                var target = this.ForceTarget;
                if (target != null)
                    return target;
                
                target = this.PrimaryTarget;
                if (target != null)
                    return target;

                return null;
            }
        }

        protected override void OnDisable() {
            this.PrimaryTarget = null;
            this.ForceTarget = null;
        }
    }
}