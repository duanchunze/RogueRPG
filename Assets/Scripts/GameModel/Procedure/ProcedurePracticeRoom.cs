namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePracticeRoom : AProcedureState {
        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            var actor = this.GetData<Actor>();
            if (actor == null)
                return;

            GameManager.Instance.SetMainMan(actor);
            GameManager.Instance.SetCameraFocus(actor.UnityTransform);
            actor.Entity.DontDestroyOnLoadWithUnity();

            await SceneManager.LoadSceneWithUnity("PracticeRoom", LoadSceneMode.Single);
            await this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override async HTask OnLeave(IFsm fsm, IFsmState next) {
            await this.OnLeaveShadow(fsm, next);
            GameManager.Instance.DestroyMainMain();
        }
    }
}