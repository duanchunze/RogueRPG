namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureMainInterface_Combat : AProcedureState<ProcedureMainInterface> {
        private int _currentSelectHeroIndex;

        public int CurrentSelectHeroIndex {
            get => this._currentSelectHeroIndex;
            set {
                var actor = this.ShowSelectHero(value);
                if (actor == null)
                    return;
                
                if (this.CurrentSelectHero == actor)
                    return;

                if (this.CurrentSelectHero != null) {
                    Object.Destroy(this.CurrentSelectHero.Entity);
                }
                
                this._currentSelectHeroIndex = value;
                this.CurrentSelectHero = actor;
            }
        }

        public Actor CurrentSelectHero { get; private set; }

        [ShadowFunction]
        protected override void OnEnter(IFsm fsm, IFsmState prev) {
            this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
        }

        [ShadowFunction]
        private Actor ShowSelectHero(int index) {
            Actor actor = null;
            this.ShowSelectHeroShadow(index, x => { actor = x; });
            return actor;
        }
    }
}