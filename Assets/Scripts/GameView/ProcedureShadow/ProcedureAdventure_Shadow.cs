using System;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure))]
    public static partial class ProcedureAdventure_Shadow {
        [ShadowFunction]
        private static async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await HTask.Completed;
            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.positionOffset = new UnityEngine.Vector3(0, 13f, -7);
            followTarget.rotationOffset = new UnityEngine.Vector3(0, 0, 0);
            
            GameManager.Instance.MainMan.GetComponent<HeadInfo>().Enable = true;

            UIManager.SingleOpen<UIAbilitesBar>(UILayer.High);
            UIManager.SingleOpen<UIPropBar>(UILayer.High);
        }

        [ShadowFunction]
        private static HTask OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UIAbilitesBar>();
            UIManager.SingleClose<UIPropBar>();
            return default;
        }
    }
}