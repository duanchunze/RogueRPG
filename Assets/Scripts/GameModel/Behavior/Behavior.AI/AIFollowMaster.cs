using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class AIFollowMaster : AIInfo<ai.FollowMaster> {
        private Minion _minion;
        private MinionsBar _minionsBar;
        private Control _control;

        protected override void OnEnable() { }

        protected override bool Check() {
            return true;
        }

        protected override void Enter() {
            this._minion ??= this.manager.Bodied.MainBodied.GetComponent<Minion>();
            this._minionsBar ??= this._minion.master.GetComponent<MinionsBar>();
            this._control ??= this.manager.Bodied.MainBodied.GetComponent<Control>();
        }

        protected override void Running() {
            var owner = this.manager.Bodied.MainBodied;
            if (this._minionsBar.TryGetQueuePositionOfMinion(this._minion, out var followPoint)) {
                if (Vector3.Distance(owner.transform.Position, followPoint) > 0.2f) {
                    Shortcut.SimulatePointMove(this._control, followPoint);
                }
            }
        }

        protected override void Exit() { }
    }
}