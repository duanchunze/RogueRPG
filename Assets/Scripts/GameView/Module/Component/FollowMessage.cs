using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl.View {
    [Serializable]
    public class FollowMessage : Unbodied, IUpdate {
        public float uiStayTime;

        private UIFollowMessage _current;
        private float _timer;

        [ShowInInspector]
        private float _height;

        public void Update() {
            if (this._timer >= this.uiStayTime) {
                if (this._current != null) {
                    UIManager.MultiClose(this._current);
                    this._current = null;
                }

                return;
            }

            this._timer += TimeInfo.DeltaTime;
        }

        public void ShowFollowMessage(string content) {
            var ui = UIManager.MultiOpen<UIFollowMessage>(UILayer.Low);
            if (this._current != null) {
                UIManager.MultiClose(this._current);
            }

            this._current = ui;

            ui.text.text = content;
            ui.followTarget = this.UnityTransform;
            ui.followOffset = new Vector3(0, 0, this._height);
            this._timer = 0;
        }

        public void UpdateFollowHeight(float height) {
            this._height = height;
            if (this._current == null) return;
            this._current.followOffset = new Vector3(0, 0, height);
        }
    }
}