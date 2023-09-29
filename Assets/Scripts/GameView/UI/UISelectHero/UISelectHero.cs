using Hsenl.EventType;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UISelectHero : UI<UISelectHero> {
        public static UISelectHero instance;

        public UnityEngine.Transform holder;
        public UnityEngine.Transform template;

        public Button confirmButton;
        public Button returnButton;

        [ReadOnly]
        public UISelectHeroSlot selectSlot;


        private void Awake() {
            instance = this;
        }

        private void OnDestroy() {
            instance = null;
        }

        protected override void OnCreate() {
            this.confirmButton.onClick.AddListener(() => {
                if (this.selectSlot != null) {
                    Procedure.ChangeState<ProcedureStartAdventure, int>(this.selectSlot.Filler.Id);
                }
            });

            this.returnButton.onClick.AddListener(() => { Procedure.ChangeState<ProcedureMainInterface>(); });
        }

        protected override void OnOpen() {
            EventSystem.Publish(new OnHeroUnlockUpdate());
        }
    }
}