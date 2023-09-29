using System;
using Hsenl.EventType;
using UnityEngine;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIMainInterface : UI<UIMainInterface> {
        public Button startGameButton;

        protected override void OnCreate() {
            this.startGameButton.onClick.AddListener(() => { Procedure.ChangeState<ProcedureSelectHero>(); });
        }
    }
}