using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class AIPatrol : AIInfo<ai.PatrolInfo> {
        private Vector3 _originPosition;
        private Vector3 _targetPosition;

        private Control _selfControl;

        private Vector2 breathingTimeRange = new(6, 10);
        private float breathingTime;
        private float breathingTimer;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            this._selfControl = owner.GetComponent<Control>();
            this._originPosition = owner.transform.Position;
            this._targetPosition = this._originPosition;
        }

        protected override bool Check() {
            return true;
        }

        protected override void Enter() { }

        protected override void Running() {
            var owner = this.manager.Bodied.MainBodied;
            if (!owner.transform.IsNavMoveDone()) {
                return;
            }

            if (this.breathingTimer < this.breathingTime) {
                this.breathingTimer += TimeInfo.DeltaTime;
                return;
            }

            this.breathingTime = RandomHelper.NextFloat(this.breathingTimeRange.x, this.breathingTimeRange.y);
            this.breathingTimer = 0;
            var min = this._originPosition - new Vector3(this.info.PatrolRange);
            var max = this._originPosition + new Vector3(this.info.PatrolRange);
            var pos = RandomHelper.NextFloat3(min, max);
            this._targetPosition = new Vector3(pos.x, pos.y, pos.z);
            this._targetPosition.y = owner.transform.Position.y;
            this._selfControl.SetValue(ControlCode.MoveOfPoint, this._targetPosition);
        }

        protected override void Exit() { }
    }
}