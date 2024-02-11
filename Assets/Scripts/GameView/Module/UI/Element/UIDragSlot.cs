using UnityEngine;
using UnityEngine.EventSystems;

namespace Hsenl.View {
    public abstract class UIDragSlot<TFiller> : UISlot<TFiller> {
        public RectTransform dragTransform;
        public bool dragLock;

        protected UIDrag drag;

        protected override void OnFillerIn() {
            if (!this.dragLock) {
                if (this.drag == null) {
                    this.drag = this.gameObject.AddComponent<UIDrag>();
                    this.drag.uiCamera = UIManager.Instance.uiCamera;
                    this.drag.useOffset = false;
                }

                this.drag.moveTransform = this.dragTransform;
                this.drag.onEndDrag = this.OnEndDrag;
            }
        }

        protected override void OnFillerTakeout() {
            if (this.drag) {
                Destroy(this.drag);
                this.drag = null;
            }
        }

        protected virtual void OnEndDrag(PointerEventData data) { }

        public Vector2 GetDragerPosition() {
            return RectTransformUtility.WorldToScreenPoint(UIManager.Instance.uiCamera, this.dragTransform.position);
        }
    }
}