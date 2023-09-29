using UnityEngine;

namespace Hsenl {
    public class ProcedureStartAdventure : AProcedureState<int> {
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            if (GameManager.Instance.MainMan == null) {
                var config = Tables.Instance.TbActorConfig.GetById(this.data);
                var actor = ActorManager.Instance.Rent(config.Id, Vector3.zero);
                GameManager.Instance.SetMainMan(actor);
                GameManager.Instance.SetMainControl(actor.GetComponent<Control>());
                GameManager.Instance.AddControlTarget(actor.GetComponent<Control>());
                GameManager.Instance.SetCameraFocus(actor.UnityTransform);
                actor.Entity.DontDestroyOnLoadWithUnity();
            }

            var adv = AdventureManager.Instance.NewAdventure(10000);
            adv.SetRecord(new RcdDefaultCheckpointsAdventure() {
                totalCheckpoint = adv.Config.Checkpoints.Count,
            });
            adv.Begin();
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) { }
    }
}