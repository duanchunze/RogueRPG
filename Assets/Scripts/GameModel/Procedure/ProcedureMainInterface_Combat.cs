namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureMainInterface_Combat : AProcedureState<ProcedureMainInterface> {
        private int _currentSelectHeroIndex;

        public int CurrentSelectHeroIndex {
            get => this._currentSelectHeroIndex;
            set {
                var actor = this.ShowSelectHero(value);
                if (actor == null) // actor == null, 说明index无效
                    return;

                if (this.CurrentSelectHero != actor) {
                    if (this.CurrentSelectHero != null) {
                        Object.Destroy(this.CurrentSelectHero.Entity);
                    }

                    this.CurrentSelectHero = actor;
                }

                this._currentSelectHeroIndex = value;
            }
        }

        public Actor CurrentSelectHero { get; private set; }

        public Actor TakeCurrentSelectHero() {
            var actor = this.CurrentSelectHero;
            this.CurrentSelectHero = null;
            return actor;
        }

        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override async HTask OnLeave(IFsm fsm, IFsmState next) {
            await this.OnLeaveShadow(fsm, next);
            if (this.CurrentSelectHero != null) {
                Object.Destroy(this.CurrentSelectHero.Entity);
                this.CurrentSelectHero = null;
            }
        }

        private Actor ShowSelectHero(int index) {
            var configList = Tables.Instance.TbActorConfig.DataList;
            if (index < 0 || index >= configList.Count)
                return null;

            var config = configList[index];
            var actor = ActorManager.Instance.Rent(config.Id);
            actor.Tags.Remove(TagType.Monster);
            actor.Tags.Add(TagType.Hero);
            this.OnShowSelectHero(actor);
            return actor;
        }

        [ShadowFunction]
        private void OnShowSelectHero(Actor actor) {
            this.OnShowSelectHeroShadow(actor);
        }
    }
}