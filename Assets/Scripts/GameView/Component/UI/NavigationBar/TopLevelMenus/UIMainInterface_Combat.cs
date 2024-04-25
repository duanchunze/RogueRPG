using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIMainInterface_Combat : UI<UIMainInterface_Combat> {
        public Button combatButton;
        public Button leftButton;
        public Button rightButton;

        protected override void OnCreate() {
            this.combatButton.onClick.AddListener(() => {
                var state = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>();
                ProcedureManager.Procedure.ChangeState<ProcedureAdventure>(state.CurrentSelectHero);
            });
            
            this.leftButton.onClick.AddListener(() => {
                var state = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>();
                state.CurrentSelectHeroIndex--;
            });
            
            this.rightButton.onClick.AddListener(() => {
                var state = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>();
                state.CurrentSelectHeroIndex++;
            });
        }
    }
}