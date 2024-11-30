using System;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(GameManager))]
    public partial class GameManagerView : MonoBehaviour {
        private bool _uiopen;
        private Actor _mainActor;

        private void Update() {
            if (InputController.GetButtonDown(InputCode.Escape)) {
                UIManager.SwitchSingleUI<UIESC>(UILayer.High);
            }

            if (InputController.GetButtonDown(InputCode.LeftCtrl)) {
                this._uiopen = !this._uiopen;
                if (this._uiopen) {
                    UIManager.SingleOpen<UICardPool>(UILayer.High);
                    // UIManager.SingleOpen<UICardStore>(UILayer.High);
                    // UIManager.SingleOpen<UICardBar>(UILayer.High);
                    // UIManager.SingleOpen<UICardBackpack>(UILayer.High);
                }
                else {
                    UIManager.SingleClose<UICardPool>();
                    // UIManager.SingleClose<UICardStore>();
                    // UIManager.SingleClose<UICardBar>();
                    // UIManager.SingleClose<UICardBackpack>();
                }
            }
        }

        [ShadowFunction]
        private void SetMainMan(Hsenl.Actor mainMan) {
            if (this._mainActor is { IsDisposed: false }) {
                var sf = this._mainActor.GetComponent<SelectorFlag>();
                if (sf != null)
                    Object.Destroy(sf);
            }

            this._mainActor = mainMan;
            if (this._mainActor != null) {
                var sf = this._mainActor.GetComponent<SelectorFlag>();
                if (sf == null) {
                    this._mainActor.Entity.AddComponent<SelectorFlag>();
                }
            }
        }
    }
}