using System;

namespace Hsenl.MultiCombiner {
    // 如果没有施法器, 就由优先器来驱动阶段线, 如果有施法器, 就由施法器来驱动
    [CombinerOverride(typeof(PriorityState_StageLine_Combiner))]
    public class Caster_PriorityState_StageLine_Combiner : MultiCombiner<Caster, PriorityState, StageLine> {
        private class Variable {
            public float duration;
        }

        protected override void OnCombin(Caster arg1, PriorityState arg2, StageLine arg3) {
            var variable = this.GetUserToken<Variable>();
            variable.duration = arg2.Duration;

            // 优先器的持续时间交给施法器来决定
            arg2.Duration = -1;

            // --------- 填充caster的逻辑
            // 由阶段线的总时长来决定施法器的模式
            arg1.getCastModelInvoke += this.EnqueueAction<Func<CastModel>>(() => arg3.TotalDuration switch {
                < 0 => CastModel.InfiniteTime,
                > 0 => CastModel.FiniteTime,
                _ => CastModel.Transient
            });

            arg1.startCastInvoke += this.EnqueueAction<Func<CastEvaluateState>>(() => {
                var ret = arg2.Enter();
                if (arg1.IsCasting) {
                    // 触发stage line下, 所有ICasterNode的事件
                    using var list = ListComponent<ICastEvents>.Rent();
                    arg3.EntryNode.GetNodesInChildren(list);
                    foreach (var casterNode in list) {
                        casterNode.CastStart();
                    }
                }

                if (!ret)
                    return CastEvaluateState.PriorityStateEnterFailure;

                return CastEvaluateState.Success;
            });

            arg1.isCastingInvoke += this.EnqueueAction<Func<bool>>(() => arg2.IsEntered);

            arg1.stopCastInvoke += this.EnqueueAction<Action>(() => {
                if (arg1.IsCasting) {
                    // 触发stage line下, 所有ICasterNode的事件
                    using var list = ListComponent<ICastEvents>.Rent();
                    arg3.EntryNode.GetNodesInChildren(list);
                    foreach (var casterNode in list) {
                        casterNode.CastEnd();
                    }
                }

                // 在这种组合下, cast不能主动的停止priority state, 所以, castEnd并不会调用priorityState.LeaveState(); 也不会触发OnCastEnd, 而是交给priority state
            });

            // ------------- 填充priority state的逻辑
            arg2.onEnter += this.EnqueueAction<Action<IPrioritizer>>(_ => {
                arg3.Start();
                arg1.OnCastStart();
            });
            arg2.onUpdate += this.EnqueueAction<Action<IPrioritizer, float>>((_, deltaTime) => {
                var status = arg3.Run(deltaTime);
                if (status != StageStatus.Running) {
                    arg2.Leave();
                    return;
                }

                arg1.OnCastRunning(deltaTime);
            });
            arg2.onLeaveDetails += this.EnqueueAction<Action<IPrioritizer, PriorityStateLeaveDetails>>((_, details) => {
                switch (details.LeaveType) {
                    case PriorityStateLeaveType.TimeOut: {
                        // 虽然priority系统也可以超时退出, 不过在与stageline合作的情况下, 都是使用stagetine的timer, 所以上面的duration才设置为-1
                        arg1.OnCastEnd(new CasterEndDetails { endType = CasterEndType.Complated, initiator = (PriorityState)details.Initiator });
                        break;
                    }
                    case PriorityStateLeaveType.InitiativeInvoke: {
                        // priority state在这种组合下, 只会被stage line主动退出, 所以, 如果priority state主动退出了, 就可以当成stage line已经执行完了
                        arg1.OnCastEnd(new CasterEndDetails { endType = CasterEndType.Complated, initiator = (PriorityState)details.Initiator });
                        break;
                    }
                    case PriorityStateLeaveType.ReEnter:
                    case PriorityStateLeaveType.Exclusion: { // 如果priority state是被排挤掉的, 则再根据当前施法的阶段来判断细节
                        switch ((StageType)arg3.CurrentStage) {
                            case StageType.None:
                                break;
                            case StageType.Enter: // 当施法只处于进入阶段下, 被排挤不属于打断, 而是被顶替
                                arg1.OnCastEnd(new CasterEndDetails { endType = CasterEndType.Exclusion, initiator = (PriorityState)details.Initiator });
                                break;
                            case StageType.Reading:
                            case StageType.Charging:
                            case StageType.Lifting:
                            case StageType.Casting: // 当施法进入读条、蓄力、抬手、施法这几个阶段的时候, priority state被排挤, 就属于被施法被打断
                                arg1.OnCastEnd(new CasterEndDetails { endType = CasterEndType.Break, initiator = (PriorityState)details.Initiator });
                                break;
                            case StageType.Recovering: // 如果施法已经进入收招阶段了, 那就算被排挤, 也算是施法完了
                                arg1.OnCastEnd(new CasterEndDetails { endType = CasterEndType.Complated, initiator = (PriorityState)details.Initiator });
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                arg3.Abort();
            });
        }

        protected override void OnDecombin(Caster arg1, PriorityState arg2, StageLine arg3) {
            arg1.getCastModelInvoke -= this.DequeueAction<Func<CastModel>>();
            arg1.startCastInvoke -= this.DequeueAction<Func<CastEvaluateState>>();
            arg1.isCastingInvoke -= this.DequeueAction<Func<bool>>();
            arg1.stopCastInvoke -= this.DequeueAction<Action>();
            arg2.onEnter -= this.DequeueAction<Action<IPrioritizer>>();
            arg2.onUpdate -= this.DequeueAction<Action<IPrioritizer, float>>();
            arg2.onLeaveDetails -= this.DequeueAction<Action<IPrioritizer, PriorityStateLeaveDetails>>();

            var variable = this.GetUserToken<Variable>();
            arg2.Duration = variable.duration;
        }
    }
}