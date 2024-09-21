using UnityEngine;
using UnityEngine.EventSystems;

namespace Hsenl.View {
    /// <summary>
    /// 该类赋予了slot拖拽的能力, 当填入填充物时, 该slot将变得可以拖拽(需提供拖拽transform), 并在拖拽结束时, 通过OnEndDrag回调通知.
    /// 还可以通过GetDragerPosition方法, 实时获得当前拖拽的屏幕位置.
    /// </summary>
    /// <typeparam name="TFiller"></typeparam>
    public abstract class UIDragSlot<TFiller> : UISlot<TFiller>, IUIDragSlot {
        public RectTransform dragTransform;
        public bool dragLock;

        protected override void OnFillerIn() {
            if (!this.dragLock) {
                var darg = this.GetComponent<UIDrag>();
                if (darg == null) {
                    darg = this.gameObject.AddComponent<UIDrag>();
                    darg.uiCamera = UIManager.Instance.uiCamera;
                    darg.useOffset = false;
                }

                darg.moveTransform = this.dragTransform;
                darg.onEndDrag = this.OnEndDrag;
            }
        }

        protected override void OnFillerTakeout() {
            var darg = this.GetComponent<UIDrag>();
            if (darg) {
                Destroy(darg);
            }
        }

        protected virtual void OnEndDrag(PointerEventData data) { }

        public UnityEngine.Vector2 GetDragerPosition() {
            return RectTransformUtility.WorldToScreenPoint(UIManager.Instance.uiCamera, this.dragTransform.position);
        }
    }
}