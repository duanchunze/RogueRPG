using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIMainInterface_Combat : UI<UIMainInterface_Combat> {
        public Button combatButton;
        public Button practiceRoomButton;
        public Button leftButton;
        public Button rightButton;

        protected override void OnCreate() {
            this.combatButton.onClick.AddListener(() => {
                var state = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>();
                var actor = state.TakeCurrentSelectHero();
                if (actor != null) {
                    ProcedureManager.Procedure.ChangeState<ProcedureAdventure>(actor);
                }
            });

            this.practiceRoomButton.onClick.AddListener(() => {
                var actor = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>().TakeCurrentSelectHero();
                if (actor != null) {
                    ProcedureManager.Procedure.ChangeState<ProcedurePracticeRoom>(actor);
                }
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

        private void Update() {
            if (InputController.GetButtonDown(InputCode.A)) {
                var state = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>();
                state.CurrentSelectHeroIndex--;
            }

            if (InputController.GetButtonDown(InputCode.D)) {
                var state = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>();
                state.CurrentSelectHeroIndex++;
            }
        }
    }
}