using UnityEngine.UI;

namespace Hsenl.View {
    public class UINavigationBar : UI<UINavigationBar> {
        public Button combatButton;
        public Button actorButton;
        public Button abilityButton;
        public Button collectiblesButton;
        public Button achievementButton;

        protected override void OnCreate() {
            this.combatButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Combat>();
            });
            
            this.actorButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Actor>();
            });
            
            this.abilityButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Ability>();
            });
            
            this.collectiblesButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Collectibles>();
            });
            
            this.achievementButton.onClick.AddListener(() => {
                ProcedureManager.Procedure.ChangeState<ProcedureMainInterface_Achievement>();
            });
        }
    }
}