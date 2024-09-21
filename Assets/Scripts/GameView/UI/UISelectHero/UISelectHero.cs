using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UISelectHero : UI<UISelectHero> {
        public UnityEngine.Transform holder;
        public UnityEngine.Transform template;

        public Button confirmButton;
        public Button returnButton;

        [ReadOnly]
        public UISelectHeroSlot selectSlot;

        protected override void OnCreate() {
            this.confirmButton.onClick.AddListener(() => {
                if (this.selectSlot != null) {
                    ProcedureManager.Procedure.ChangeState<ProcedureAdventure>(this.selectSlot.Filler.Id);
                }
            });

            this.returnButton.onClick.AddListener(() => { ProcedureManager.Procedure.ChangeState<ProcedureMainInterface>(); });
        }

        protected override void OnOpen() {
            EventStation.OnHeroUnlockUpdate();
        }
    }
}