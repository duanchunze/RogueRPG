using System;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIESC : UI<UIESC> {
        public Button returnMainInterfaceButton;

        protected override void OnCreate() {
            this.returnMainInterfaceButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Combat>();
                this.Close();
            });
        }
    }
}