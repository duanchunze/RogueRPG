using System;
using UnityEngine;

namespace Hsenl {
    public enum UIOpenType {
        Single,
        Multi
    }

    public interface IUI {
        string Name { get; }
        bool IsOpen { get; }

        internal void InternalOpen(UIOpenType openType, UnityEngine.Transform parent);
        internal void InternalClose();
    }

    [DisallowMultipleComponent]
    public abstract class UI<T> : MonoBehaviour, IUI where T : UI<T> {
        protected UIOpenType OpenType { get; private set; }

        public string Name => typeof(T).Name;
        public bool IsOpen => this.gameObject.activeSelf;

        private void Awake() {
            this.OnCreate();
        }

        protected void Close() {
            switch (this.OpenType) {
                case UIOpenType.Single:
                    UIManager.SingleClose(this.Name);
                    break;
                case UIOpenType.Multi:
                    UIManager.MultiClose(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IUI.InternalOpen(UIOpenType openType, UnityEngine.Transform parent) {
            this.OpenType = openType;
            var rectTra = (RectTransform)this.transform;
            rectTra.SetParent(parent, false);
            rectTra.pivot = new UnityEngine.Vector2(0.5f, 0.5f);
            rectTra.anchorMin = UnityEngine.Vector2.zero;
            rectTra.anchorMax = UnityEngine.Vector2.one;
            rectTra.sizeDelta = UnityEngine.Vector2.zero;
            rectTra.localPosition = UnityEngine.Vector3.zero;
            this.gameObject.SetActive(true);
            this.OnOpen();
        }

        void IUI.InternalClose() {
            if (this)
                this.gameObject.SetActive(false);

            this.OnClose();
        }

        protected virtual void OnCreate() { }

        protected virtual void OnOpen() { }

        protected virtual void OnClose() { }
    }
}