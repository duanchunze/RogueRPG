using System;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedurePracticeRoom))]
    public static partial class ShadowProcedurePracticeRoom {
        [ShadowFunction]
        private static async Hsenl.HTask OnEnter(Hsenl.IFsm fsm, Hsenl.IFsmState prev) {
            await HTask.Completed;
            
            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.positionOffset = new UnityEngine.Vector3(0, 22.58f, -13.8f);
            followTarget.rotationOffset = new UnityEngine.Vector3(0, 0, 0);
            
            GameManager.Instance.MainMan.GetComponent<HeadInfo>().Enable = true;

            UIManager.SingleOpen<UIAbilitesBar>(UILayer.High);
            UIManager.SingleOpen<UIPropBar>(UILayer.High);
        }

        [ShadowFunction]
        private static HTask OnLeave(Hsenl.IFsm fsm, Hsenl.IFsmState next) {
            UIManager.SingleClose<UIAbilitesBar>();
            UIManager.SingleClose<UIPropBar>();
            return default;
        }
    }
}