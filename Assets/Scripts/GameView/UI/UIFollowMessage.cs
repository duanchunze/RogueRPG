using System;
using TMPro;
using UnityEngine;

namespace Hsenl.View {
    public class UIFollowMessage : UI<UIFollowMessage> {
        public TextMeshProUGUI text;
        public UnityEngine.Transform followTarget;
        public UnityEngine.Vector3 followOffset;

        private RectTransform _rectTransform;

        private void Awake() {
            this._rectTransform = (RectTransform)this.transform;
        }

        private void Update() {
            if (this.followTarget) {
                if (UIManager.WorldToUIPosition(this._rectTransform, this.followTarget.position + this.followOffset, out var worldPos)) {
                    this._rectTransform.position = worldPos;
                }
            }
        }
    }
}