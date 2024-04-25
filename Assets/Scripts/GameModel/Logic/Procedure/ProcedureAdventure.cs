using UnityEngine;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureAdventure : AProcedureState {

        [ShadowFunction]
        protected override void OnEnter(IFsm fsm, IFsmState prev) {
            var actor = this.GetData<Actor>();
            GameManager.Instance.SetMainMan(actor);
            GameManager.Instance.SetMainControl(actor.GetComponent<Control>());
            GameManager.Instance.AddControlTarget(actor.GetComponent<Control>());
            GameManager.Instance.SetCameraFocus(actor.UnityTransform);
            actor.Entity.DontDestroyOnLoadWithUnity();

            var adv = AdventureManager.Instance.NewAdventure(10000);
            adv.SetRecord(new RcdDefaultCheckpointsAdventure() {
                totalCheckpoint = adv.Config.Checkpoints.Count,
            });
            adv.Begin();

            this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
        }
    }
}