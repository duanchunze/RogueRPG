using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        public RectTransform moveTransform;
        public Action<PointerEventData> onEndDrag;
        public Camera uiCamera;
        public bool useOffset;

        [Header("场景中Canvas")]
        private Canvas _canvas;

        [Header("表示限制的区域")]
        private RectTransform _limitContainer;

        private Vector3 _originLocalPosition;
        private Vector3 _offset;

        // 最小、最大X、Y坐标
        private float minX, maxX, minY, maxY;

        private void Start() {
            this._originLocalPosition = this.moveTransform.localPosition;
            this._canvas = this.GetComponentInParent<Canvas>();
            this._limitContainer = (RectTransform)this._canvas.transform;
        }

        private void OnDisable() {
            this.ResetPosition();
            var canvas = this.GetComponent<Canvas>();
            if (canvas != null) {
                UnityEngine.Object.Destroy(canvas);
            }
        }

        /// <summary>
        /// 设置最大、最小坐标
        /// </summary>
        private void SetDragRange() {
            var position = this._limitContainer.position;
            var scaleFactor = this._canvas.scaleFactor;
            // 最小x坐标 = 容器当前x坐标 - 容器轴心距离左边界的距离 + UI轴心距离左边界的距离
            this.minX = position.x
                        - this._limitContainer.pivot.x * this._limitContainer.rect.width * scaleFactor
                        + this.moveTransform.rect.width * scaleFactor * this.moveTransform.pivot.x;

            // 最大x坐标 = 容器当前x坐标 + 容器轴心距离右边界的距离 - UI轴心距离右边界的距离
            this.maxX = position.x
                        + (1 - this._limitContainer.pivot.x) * this._limitContainer.rect.width * scaleFactor
                        - this.moveTransform.rect.width * scaleFactor * (1 - this.moveTransform.pivot.x);

            // 最小y坐标 = 容器当前y坐标 - 容器轴心距离底边的距离 + UI轴心距离底边的距离
            this.minY = position.y
                        - this._limitContainer.pivot.y * this._limitContainer.rect.height * scaleFactor
                        + this.moveTransform.rect.height * scaleFactor * this.moveTransform.pivot.y;

            // 最大y坐标 = 容器当前x坐标 + 容器轴心距离顶边的距离 - UI轴心距离顶边的距离
            this.maxY = position.y
                        + (1 - this._limitContainer.pivot.y) * this._limitContainer.rect.height * scaleFactor
                        - this.moveTransform.rect.height * scaleFactor * (1 - this.moveTransform.pivot.y);
        }

        /// <summary>
        /// 限制坐标范围
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Vector3 DragRangeLimit(Vector3 pos) {
            pos.x = Mathf.Clamp(pos.x, this.minX, this.maxX);
            pos.y = Mathf.Clamp(pos.y, this.minY, this.maxY);
            return pos;
        }

        private void ResetPosition() {
            this.moveTransform.localPosition = this._originLocalPosition;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (this.uiCamera == null && eventData.enterEventCamera == null) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            var eventCamera = this.uiCamera == null ? eventData.enterEventCamera : this.uiCamera;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.moveTransform, eventData.position, eventCamera, out var pos)) {
                // 计算偏移量
                var position = this.moveTransform.position;
                this._offset = position - pos;
                // // 设置拖拽范围
                // this.SetDragRange();

                var parentCanvas = this.GetComponentInParent<Canvas>();
                var canvas = this.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = parentCanvas.sortingOrder + 1;
            }
        }

        public void OnDrag(PointerEventData eventData) {
            if (this.uiCamera == null && eventData.enterEventCamera == null) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            var eventCamera = this.uiCamera == null ? eventData.enterEventCamera : this.uiCamera;

            // 将屏幕空间上的点转换为位于给定RectTransform平面上的世界空间中的位置
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.moveTransform, eventData.position, eventCamera, out var pos)) {
                // this.moveTransform.position = this.DragRangeLimit(pos + this._offset);
                if (this.useOffset)
                    this.moveTransform.position = pos + this._offset;
                else
                    this.moveTransform.position = pos;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            this.onEndDrag?.Invoke(eventData);
            this.ResetPosition();
            var canvas = this.GetComponent<Canvas>();
            if (canvas != null) {
                UnityEngine.Object.Destroy(canvas);
            }
        }
    }
}