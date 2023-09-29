using System;
using UnityEngine;

namespace Hsenl.MultiCombiner {
    // 施法器和阶段线
    // 施法器采用阶段线的形式来实现施法, 阶段线由施法器来驱动, 而阶段线给施法器提供状态变化 (当前阶段、阶段完成等)
    public class CasterStageLineCombiner : MultiCombiner<Caster, StageLine> {
        protected override void OnCombin(Caster arg1, StageLine arg2) {
            // 由阶段线的模式来决定施法器的模式
            arg1.getCastModelInvoke = () => (CastModel)(int)arg2.TimeStageLineModel;

            arg1.onEnter += this.EnqueueAction(new Action(arg2.Reset));

            arg1.onUpdate += this.EnqueueAction(new Action<float>(deltaTime => {
                var status = arg2.Run(deltaTime);
                if (status != StageStatus.Running) {
                    arg1.CastEnd();
                }
            }));

            // 施法器结束时, 做判断, 看是什么原因导致离开的, 如果是特殊情况, 比如被中断了, 或者被
            arg1.onLeaveDetails += this.EnqueueAction<Action<CasterLeaveDetails>>(details => {
                // Debug.LogError($"caster leave '{((PriorityState)details.initiator).Name}' '{arg1.Name}'");
                switch ((StageType)arg2.CurrentStage) {
                    case StageType.None:
                        break;
                    case StageType.Enter:
                        switch (details.leaveType) {
                            case CasterLeaveType.Complated:
                                break;
                            case CasterLeaveType.InitiativeInvoke:
                                break;
                            case CasterLeaveType.Exclusion:
                                arg1.OnIntercepted(details.initiator);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case StageType.Reading:
                    case StageType.Charging:
                    case StageType.Lifting:
                    case StageType.Casting:
                        switch (details.leaveType) {
                            case CasterLeaveType.Complated:
                                break;
                            case CasterLeaveType.InitiativeInvoke:
                                break;
                            case CasterLeaveType.Exclusion:
                                arg1.OnBreak(details.initiator);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case StageType.Recovering:
                        arg1.OnFinish();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                arg2.Abort();
            });
        }

        protected override void OnDecombin(Caster arg1, StageLine arg2) {
            arg1.getCastModelInvoke = null;
            arg1.onEnter -= this.DequeueAction<Action>();
            arg1.onUpdate -= this.DequeueAction<Action<float>>();
            arg1.onLeaveDetails -= this.DequeueAction<Action<CasterLeaveDetails>>();
        }
    }
}