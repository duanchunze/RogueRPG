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
                ProcedureManager.Procedure.ChangeState<ProcedureAdventure>(state.CurrentSelectHero);
            });

            this.practiceRoomButton.onClick.AddListener(async () => {
                var actor = ProcedureManager.Procedure.GetState<ProcedureMainInterface_Combat>().CurrentSelectHero;
                var state = ProcedureManager.Procedure.GetState<ProcedurePracticeRoom>();
                state.actor = actor;
                state.sceneName = "PracticeRoom";
                state.loadSceneMode = LoadSceneMode.Single;
                ProcedureManager.Procedure.ChangeState<ProcedurePracticeRoom>();
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