using System;
using System.Collections.Generic;
using Hsenl.casterevaluate;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class CeApproachTarget : CeInfo<ApproachTargetInfo> {
        private Transform _tran;
        private Control _control;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._tran = owner?.transform;
                    this._control = owner?.GetComponent<Control>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            var target = this.manager.castEvaluateResult.CastedTarget;
            switch (this.manager.Bodied) {
                case Ability abi: {
                    if (target == null)
                        return NodeStatus.Failure;

                    switch (this.manager.castEvaluateResult.CastEvaluateState) {
                        case CastEvaluateState.DistanceDeficiency: {
                            var crange = this.manager.castEvaluateResult.CastRange;
                            var selfPosition = this._tran.Position;
                            var targetPosition = target.transform.Position;
                            var dis = Vector3.Distance(selfPosition, targetPosition);
                            dis -= crange;
                            dis += 0.1f; // 再靠近一点, 不要那么掐着距离
                            if (dis <= 0) {
                                Shortcut.TurnAround(this._control, target.transform.Position - this._tran.Position);
                                return NodeStatus.Running;
                            }

                            var dir = (targetPosition - selfPosition).normalized;
                            var point = selfPosition + dir * dis;
                            Shortcut.SimulatePointMove(this._control, point);
                            return NodeStatus.Running;
                        }
                        case CastEvaluateState.AngeleDeficiency: {
                            Shortcut.TurnAround(this._control, target.transform.Position - this._tran.Position);
                            return NodeStatus.Running;
                        }
                    }

                    return NodeStatus.Failure;
                }
            }

            return NodeStatus.Failure;
        }
    }
}