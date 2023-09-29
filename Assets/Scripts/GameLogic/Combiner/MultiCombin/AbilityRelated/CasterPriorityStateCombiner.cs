using System;
using UnityEngine;

namespace Hsenl.MultiCombiner {
    // 如果没有施法器, 就由优先器来驱动阶段线, 如果有施法器, 就由施法器来驱动
    [CombinerOverride(typeof(PriorityStateStageLineCombiner))]
    public class CasterPriorityStateCombiner : MultiCombiner<Caster, PriorityState> {
        protected override void OnCombin(Caster caster, PriorityState priorityState) {
            // caster 控制 priority的 进与出
            // 而 priority 会触发 进入、运行、退出的回调, 调用回 caster
            caster.castStartInvoke += this.EnqueueAction(new Action(() => { priorityState.EnterState(); }));

            caster.castEndInvoke += this.EnqueueAction<Action>(() => { priorityState.LeaveState(); });

            priorityState.duration = -1; // 优先器的持续时间交给施法器来决定
            priorityState.onEnter += this.EnqueueAction<Action<IPrioritizer>>((manager) => { caster.OnEnter(); });
            priorityState.onUpdate += this.EnqueueAction<Action<IPrioritizer, float>>((manager, deltaTime) => { caster.OnUpdate(deltaTime); });
            priorityState.onLeaveDetails += this.EnqueueAction<Action<IPrioritizer, PriorityStateLeaveDetails>>((manager, details) => {
                switch (details.leaveType) {
                    case PriorityStateLeaveType.TimeOut: {
                        caster.OnLeaveDetail(new CasterLeaveDetails { leaveType = CasterLeaveType.Complated, initiator = (PriorityState)details.initiator });
                        break;
                    }
                    case PriorityStateLeaveType.InitiativeInvoke: {
                        caster.OnLeaveDetail(new CasterLeaveDetails { leaveType = CasterLeaveType.InitiativeInvoke });
                        break;
                    }
                    case PriorityStateLeaveType.ReEnter:
                    case PriorityStateLeaveType.Exclusion: {
                        caster.OnLeaveDetail(new CasterLeaveDetails { leaveType = CasterLeaveType.Exclusion, initiator = (PriorityState)details.initiator });
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        protected override void OnDecombin(Caster caster, PriorityState priorityState) {
            caster.castStartInvoke -= this.DequeueAction<Action>();
            caster.castEndInvoke -= this.DequeueAction<Action>();

            priorityState.onEnter -= this.DequeueAction<Action<IPrioritizer>>();
            priorityState.onUpdate -= this.DequeueAction<Action<IPrioritizer, float>>();
            priorityState.onLeaveDetails -= this.DequeueAction<Action<IPrioritizer, PriorityStateLeaveDetails>>();
        }
    }
}