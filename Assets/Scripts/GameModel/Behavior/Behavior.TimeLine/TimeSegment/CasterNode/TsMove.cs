using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class TsMove : TsInfo<timeline.MoveInfo>, ICastEvents {
        private Transform _moveTarget;
        private ControlTrigger _controlTrigger;
        private Numerator _numerator;

        private Vector3? _currTargetPoint;
        private Vector3 _newTargetPoint;
        private Vector3 _lookDirection;

        protected override void OnEnable() {
            base.OnEnable();

            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._moveTarget = ability.MainBodied.transform;
                    this._controlTrigger = ability.GetComponent<ControlTrigger>();
                    this._numerator = this._moveTarget.GetComponent<Numerator>();
                    break;
                }
            }
        }

        protected override void OnStart() { }

        public void CastStart() {
            if (this.info.MoveModel == 1) {
                if (this._controlTrigger.GetValue(out var float3)) {
                    this._newTargetPoint = float3;
                }
            }
        }

        public void CastEnd() {
            // 如果方向移动, 那么当按键抬起时, 就终止移动
            if (this.info.MoveModel == 0) {
                this.manager.Abort();
            }
        }

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            this._moveTarget.NavMeshAgent.Speed = this._numerator.GetValue(NumericType.Mspd);

            switch (this.info.MoveModel) {
                case 0: {
                    // 按照方向去移动, 这种方式需要持续的指定移动方向, 否则就不会移动
                    if (this._controlTrigger.GetValue(out var float3)) {
                        var dir3d = new Vector3(float3.x, 0, float3.z);
                        this._moveTarget.MoveToPoint(this._moveTarget.Position + dir3d);
                        this._moveTarget.LookRotationLerp(float3, TimeInfo.DeltaTime * 25f);
                    }

                    break;
                }
                case 1: {
                    // 如果目标点发生变化了, 则更改目的地
                    if (this._currTargetPoint == null || !this._currTargetPoint.Value.Equals(this._newTargetPoint)) {
                        this._currTargetPoint = this._newTargetPoint;
                        this._moveTarget.MoveToPoint(this._currTargetPoint.Value);
                        this._lookDirection = this._currTargetPoint.Value - this._moveTarget.Position;
                        this._lookDirection.y = 0;
                        this._lookDirection.Normalize();
                    }

                    var velocity = this._moveTarget.transform.NavMeshAgent.Velocity;
                    if (!velocity.IsZero()) {
                        this._moveTarget.LookRotationLerp(velocity, TimeInfo.DeltaTime * 25f);
                    }

                    if (this._moveTarget.IsMoveStop()) {
                        // 确保当寻路结束时, 人物能面向正确的方向
                        var angle = 0f;
                        if (!this._lookDirection.IsZero()) {
                            this._moveTarget.LookRotationLerp(this._lookDirection, TimeInfo.DeltaTime * 25f);
                            angle = Vector3.Angle(this._moveTarget.Forward, this._lookDirection);
                        }

                        if (angle < 0.1f) {
                            // 移动完成, 退出移动
                            this.manager.Abort();
                        }
                    }

                    break;
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            this._currTargetPoint = null;
            this._moveTarget.StopMove();
        }
    }
}