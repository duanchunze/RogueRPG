using TMPro;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIAdvSettlement : UI<UIAdvSettlement> {
        public TextMeshProUGUI timeSpentText;
        public Button returnMaininterfaceButton;

        protected override void OnCreate() {
            this.returnMaininterfaceButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Combat>();
                this.Close();
            });
        }
    }
}