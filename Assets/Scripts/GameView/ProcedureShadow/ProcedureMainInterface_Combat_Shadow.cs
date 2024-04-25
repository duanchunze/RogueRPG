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
        private static Hsenl.Actor ShowSelectHero(int index) {
            var configList = Tables.Instance.TbActorConfig.DataList;
            if (index < 0 || index >= configList.Count)
                return null;

            var config = configList[index];

            var actor = ActorManager.Instance.Rent(config.Id);
            actor.GetComponent<HeadInfo>().Enable = false;
            actor.transform.Forward = new Vector3(0, 0, -1);
            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.targetTransform = actor.UnityTransform;
            followTarget.positionOffset = new Vector3(0, 1, -5);
            followTarget.rotationOffset = new Vector3(-18, 0, 0);
            
            UIManager.GetSingleUI<UIActorInfo>()?.FillInActor(actor);
            
            return actor;
        }
    }
}