using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure))]
    public static partial class ProcedureAdventure_Shadow {
        [ShadowFunction]
        private static void OnEnter(IFsm fsm, IFsmState prev) {
            GameManager.Instance.MainMan.GetComponent<HeadInfo>().Enable = true;
            
            var bar = UIManager.SingleOpen<UICardBar>(UILayer.High);
            bar.HideAbilityAssist();
            
            var followTarget = Camera.main.GetComponent<FollowTarget>();
            followTarget.positionOffset = new Vector3(0, 22.58f, -13.8f);
            followTarget.rotationOffset = new Vector3(0, 0, 0);
        }

        [ShadowFunction]
        private static void OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UICardBar>();
        }
    }
}