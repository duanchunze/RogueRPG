using System;
using System.Linq;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureMainInterface_Combat))]
    public static partial class ProcedureMainInterface_Combat_Shadow {
        [ShadowFunction]
        private static void OnEnter(ProcedureMainInterface_Combat self, IFsm fsm, IFsmState prev) {
            UIManager.SingleOpen<UIMainInterface_Combat>(UILayer.High);
            UIManager.SingleOpen<UIActorInfo>(UILayer.High);
            self.CurrentSelectHeroIndex = 0;
        }

        [ShadowFunction]
        private static void OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UIMainInterface_Combat>();
            UIManager.SingleClose<UIActorInfo>();
        }

        [ShadowFunction]
        private static void OnShowSelectHero(Hsenl.Actor actor) {
            actor.GetComponent<HeadInfo>().Enable = false;
            actor.transform.Forward = new Vector3(0, 0, -1);
            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.targetTransform = actor.UnityTransform;
            followTarget.positionOffset = new UnityEngine.Vector3(0, 1, -5);
            followTarget.rotationOffset = new UnityEngine.Vector3(-18, 0, 0);

            UIManager.GetSingleUI<UIActorInfo>()?.FillInActor(actor);
        }
    }
}