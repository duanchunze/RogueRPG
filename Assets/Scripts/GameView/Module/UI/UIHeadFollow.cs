using System;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl.View {
    public class UIHeadFollow : UI<UIHeadFollow> {
        public RectTransform location;
        public VolumeSlider healthSlider;
        public VolumeSlider energySlider;
        public VolumeSlider manaSlider;
        public UnityEngine.Transform statusHolder;
        public UnityEngine.Transform statusTemplate;
        public UnityEngine.Transform followTarget;
        public Vector3 followOffset;

        private RectTransform _rectTransform;

        private void Awake() {
            this._rectTransform = (RectTransform)this.transform;
        }

        private void Update() {
            if (this.followTarget) {
                if (UIManager.WorldToUIPosition(this._rectTransform, this.followTarget.position + this.followOffset, out var uiWorldPos)) {
                    this._rectTransform.position = uiWorldPos;
                }
            }
        }
    }
}