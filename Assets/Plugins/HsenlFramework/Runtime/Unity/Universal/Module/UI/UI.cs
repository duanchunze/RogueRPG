using System;
using UnityEngine;

namespace Hsenl {
    public enum UIOpenType {
        Single,
        Multi
    }

    public interface IUI {
        string Name { get; }

        internal void InternalOpen(UIOpenType openType, UnityEngine.Transform parent);
        internal void InternalClose();
    }

    [DisallowMultipleComponent]
    public abstract class UI<T> : MonoBehaviour, IUI where T : UI<T> {
        protected UIOpenType OpenType { get; private set; }

        public string Name => typeof(T).Name;

        private void Start() {
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
            rectTra.pivot = new Vector2(0.5f, 0.5f);
            rectTra.anchorMin = Vector2.zero;
            rectTra.anchorMax = Vector2.one;
            rectTra.sizeDelta = Vector2.zero;
            rectTra.localPosition = Vector3.zero;
            this.gameObject.SetActive(true);
            this.OnOpen();
        }

        void IUI.InternalClose() {
            if (this)
                this.gameObject.SetActive(false);

            this.OnClose();
        }

        protected virtual void OnCreate() {
            // UIEvent<T>.InternalOnCreate(this);
        }

        protected virtual void OnOpen() {
            // UIEvent<T>.InternalOnOpen(this);
        }

        protected virtual void OnClose() {
            // UIEvent<T>.InternalOnClose(this);
        }
    }
}