using MemoryPack;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class AIPatrol : AIInfo<ai.PatrolInfo> {
        private float3 _originPosition;
        private float3 _targetPosition;

        private Substantive _self;
        private Control _selfControl;

        private float2 breathingTimeRange = new(6, 10);
        private float breathingTime;
        private float breathingTimer;

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Actor actor: {
                    this._self = actor;
                    this._selfControl = actor.GetComponent<Control>();
                    this._originPosition = actor.transform.Position;
                    this._targetPosition = this._originPosition;
                    break;
                }
            }
        }

        protected override bool Check() {
            return true;
        }

        protected override void Enter() { }

        protected override void Running() {
            if (!this._self.transform.IsNavMoveDone()) {
                return;
            }

            if (this.breathingTimer < this.breathingTime) {
                this.breathingTimer += TimeInfo.DeltaTime;
                return;
            }

            this.breathingTime = RandomHelper.mtRandom.NextFloat(this.breathingTimeRange.x, this.breathingTimeRange.y);
            this.breathingTimer = 0;
            var min = this._originPosition - this.info.PatrolRange;
            var max = this._originPosition + this.info.PatrolRange;
            this._targetPosition = RandomHelper.mtRandom.NextFloat3(min, max);
            this._targetPosition.y = this._self.transform.Position.y;
            this._selfControl.SetValue(ControlCode.MoveOfPoint, this._targetPosition);
        }

        protected override void Exit() { }
    }
}