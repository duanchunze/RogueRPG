using System;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedurePracticeRoom))]
    public static partial class ShadowProcedurePracticeRoom {
        [ShadowFunction]
        private static async Hsenl.HTask OnEnter(Hsenl.IFsm fsm, Hsenl.IFsmState prev) {
            await HTask.Completed;
            GameManager.Instance.MainMan.GetComponent<HeadInfo>().Enable = true;

            try {
                UIManager.SingleOpen<UIAbilitesBar>(UILayer.High);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.positionOffset = new UnityEngine.Vector3(0, 22.58f, -13.8f);
            followTarget.rotationOffset = new UnityEngine.Vector3(0, 0, 0);
        }

        [ShadowFunction]
        private static void OnLeave(Hsenl.IFsm fsm, Hsenl.IFsmState next) {
            UIManager.SingleClose<UIAbilitesBar>();
        }
    }
}