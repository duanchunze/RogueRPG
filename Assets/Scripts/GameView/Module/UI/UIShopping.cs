using System;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIShopping : UI<UIShopping> {
        public Button goonButton;

        public Action onShoppingFinish;

        protected override void OnCreate() {
            this.goonButton.onClick.AddListener(() => {
                this.onShoppingFinish?.Invoke();
            });
        }
    }
}