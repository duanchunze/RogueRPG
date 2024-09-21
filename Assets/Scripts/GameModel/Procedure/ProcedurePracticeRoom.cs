namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePracticeRoom : AProcedureState {
        public Actor actor;
        public string sceneName;
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        
        [ShadowFunction]
        protected override async void OnEnter(IFsm fsm, IFsmState prev) {
            GameManager.Instance.SetMainMan(this.actor);
            GameManager.Instance.SetMainControl(this.actor.GetComponent<Control>());
            GameManager.Instance.AddControlTarget(this.actor.GetComponent<Control>());
            GameManager.Instance.SetCameraFocus(this.actor.UnityTransform);
            this.actor.Entity.DontDestroyOnLoadWithUnity();
            
            this.OnEnterShadow(fsm, prev).Tail();
            
            await SceneManager.LoadSceneWithUnity(this.sceneName, this.loadSceneMode);
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
        }
    }
}