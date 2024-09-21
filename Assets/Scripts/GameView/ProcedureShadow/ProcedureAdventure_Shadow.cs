using System;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure))]
    public static partial class ProcedureAdventure_Shadow {
        [ShadowFunction]
        private static void OnEnter(IFsm fsm, IFsmState prev) {
            GameManager.Instance.MainMan.GetComponent<HeadInfo>().Enable = true;

            try {
                var bar = UIManager.SingleOpen<UIAbilitesBar>(UILayer.High);
                // bar.HideAbilityAssist();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.positionOffset = new UnityEngine.Vector3(0, 22.58f, -13.8f);
            followTarget.rotationOffset = new UnityEngine.Vector3(0, 0, 0);
        }

        [ShadowFunction]
        private static void OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UIAbilitesBar>();
        }
    }
}