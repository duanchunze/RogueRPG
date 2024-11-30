using UnityEngine;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureAdventure : AProcedureState {

        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            var actor = this.GetData<Actor>();
            GameManager.Instance.SetMainMan(actor);
            GameManager.Instance.SetCameraFocus(actor.UnityTransform);
            actor.Entity.DontDestroyOnLoadWithUnity();

            var adv = AdventureManager.Instance.NewAdventure(10000);
            adv.SetRecord(new RcdDefaultCheckpointsAdventure() {
                totalCheckpoint = adv.Config.Checkpoints.Count,
            });
            adv.Start();

            await this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override async HTask OnLeave(IFsm fsm, IFsmState next) {
            await this.OnLeaveShadow(fsm, next);
            GameManager.Instance.DestroyMainMain();
        }
    }
}