using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TsMove : TsInfo<timeline.MoveInfo> {
        private Transform _moveTarget;
        private ControlTrigger _controlTrigger;
        private Numerator _numerator;

        protected override void OnReset() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._moveTarget = ability.MainBodied.transform;
                    this._controlTrigger = ability.GetComponent<ControlTrigger>();
                    this._numerator = this._moveTarget.GetComponent<Numerator>();
                    break;
                }
            }

            this._moveTarget.NavMeshAgent.Speed = this._numerator.GetValue(NumericType.Mspd);

            if (this.info.MoveModel == 1) {
                // 对于点击移动来说, 在一开始的时候, 给一个目标点, 就会一直朝向这个点移动, 如果这个过程中想修改的话, 只需要setvalue就行
                if (this._controlTrigger.GetValue(out var float3)) {
                    this._moveTarget.MoveToPoint(float3);
                }
            }
        }

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            if (this.info.MoveModel == 0) {
                // 按照方向去移动, 这种方式需要持续的指定移动方向, 否则就不会移动
                if (this._controlTrigger.GetValue(out var float3)) {
                    var dir3d = new Vector3(float3.x, 0, float3.z);
                    this._moveTarget.MoveToPoint(this._moveTarget.Position + dir3d);
                    this._moveTarget.LookAtLerp(float3, TimeInfo.DeltaTime * 25f);
                }
            }
            else if (this.info.MoveModel == 1) {
                // 按照目标点去移动只需要点一次, 就可以自动持续移动, 知道满足条件后, 自动退出
                if (this._controlTrigger.GetValue(out var float3))
                    this._moveTarget.MoveToPoint(float3);

                this._moveTarget.LookAtLerp(this._moveTarget.NavMeshAgent.Velocity, TimeInfo.DeltaTime * 25f);
                if (this._moveTarget.IsNavMoveDone()) {
                    // 移动完成, 退出移动
                    this.manager.Abort();
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            this._moveTarget.StopMove();
        }
    }
}