using System;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TsMove : TsInfo<timeline.MoveInfo> {
        private Substantive _moveTarget;
        private ControlTrigger _controlTrigger;
        private NavMeshAgent _meshAgent;
        private Numerator _numerator;

        protected override void OnNodeReset() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._moveTarget = ability.GetHolder();
                    this._controlTrigger = ability.GetComponent<ControlTrigger>();
                    this._meshAgent = this._moveTarget.GetMonoComponent<NavMeshAgent>();
                    this._numerator = this._moveTarget.GetComponent<Numerator>();
                    break;
                }
            }

            this._meshAgent.speed = this._numerator.GetValue(NumericType.Mspd);

            if (this.info.MoveModel == 1) {
                // 对于点击移动来说, 在一开始的时候, 给一个目标点, 就会一直朝向这个点移动, 如果这个过程中想修改的话, 只需要setvalue就行
                if (this._controlTrigger.GetValue(out var float3)) {
                    this._moveTarget.transform.MoveToPoint(float3);
                }
            }
        }

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            if (this.info.MoveModel == 0) {
                // 按照方向去移动, 这种方式需要持续的指定移动方向, 否则就不会移动
                if (this._controlTrigger.GetValue(out var float3)) {
                    var dir3d = new float3(float3.x, 0, float3.z);
                    this._moveTarget.transform.MoveToPoint(this._meshAgent.transform.position + (Vector3)dir3d);
                    this._moveTarget.transform.LookAtLerp(float3, TimeInfo.DeltaTime * 25f);
                }
            }
            else if (this.info.MoveModel == 1) {
                // 按照目标点去移动只需要点一次, 就可以自动持续移动, 知道满足条件后, 自动退出
                if (this._controlTrigger.GetValue(out var float3))
                    this._moveTarget.transform.MoveToPoint(float3);

                this._moveTarget.transform.LookAtLerp(this._meshAgent.velocity, TimeInfo.DeltaTime * 25f);
                if (this._meshAgent.remainingDistance <= this._meshAgent.stoppingDistance) {
                    // 移动完成, 退出移动
                    this.manager.Abort();
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            this._moveTarget.transform.MoveToPoint(this._meshAgent.transform.position);
        }
    }
}