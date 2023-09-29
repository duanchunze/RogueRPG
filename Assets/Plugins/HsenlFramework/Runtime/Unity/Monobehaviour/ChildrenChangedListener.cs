using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public class ChildrenChangedListener : MonoBehaviour {
        private List<UnityEngine.Transform> _children;

        public Action<UnityEngine.Transform> onChildAdd;
        public Action<UnityEngine.Transform> onChildRemove;

        private void OnTransformChildrenChanged() {
            for (int i = 0, len = this.transform.childCount; i < len; i++) {
                var child = this.transform.GetChild(i);
                if (this._children.Contains(child)) continue;
                this._children.Add(child);
                this.OnChildAdd(child);
            }

            for (int i = 0, len = this._children.Count; i < len; i++) {
                var child = this._children[i];
                if (child.IsChildOf(this.transform)) continue;
                this._children.Remove(child);
                this.OnChildRemove(child);
            }
        }

        private void OnChildAdd(UnityEngine.Transform child) {
            this.onChildAdd?.Invoke(child);
        }

        private void OnChildRemove(UnityEngine.Transform child) {
            this.onChildRemove?.Invoke(child);
        }
    }
}