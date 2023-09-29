using System;
using UnityEngine;

namespace Hsenl.View {
    public class GameManagerView : MonoBehaviour {
        private bool _uiopen;

        private void Update() {
            if (InputController.GetButtonDown(InputCode.LeftControl)) {
                this._uiopen = !this._uiopen;
                if (this._uiopen) {
                    UIManager.SingleOpen<UICardStore>(UILayer.High);
                    UIManager.SingleOpen<UICardBar>(UILayer.High);
                    UIManager.SingleOpen<UICardBackpack>(UILayer.High);
                }
                else {
                    UIManager.SingleClose<UICardStore>();
                    UIManager.SingleClose<UICardBar>();
                    UIManager.SingleClose<UICardBackpack>();
                }
            }
        }
    }
}