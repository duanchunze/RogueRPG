using System;

namespace Hsenl.MultiCombiner {
    public class Status_PriorityState_Combiner : MultiCombiner<Status, PriorityState> {
        protected override void OnCombin(Status arg1, PriorityState arg2) {
            arg1.beginInvoke += this.EnqueueAction<Action>(() => {
                if (!arg2.IsEntered) {
                    arg2.Enter();
                }
                else {
                    arg2.Refresh();
                }
            });
            arg1.finishInvoke += this.EnqueueAction<Action>(() => { arg2.Leave(); });
            arg1.isEnterInvoke += this.EnqueueAction<Func<bool>>(() => arg2.IsEntered);

            arg2.Duration = -1; // 优先器的持续时间交给状态器决定
            arg2.onEnter += this.EnqueueAction<Action<IPrioritizer>>((_) => { arg1.OnBegin(); });
            arg2.onRefresh += this.EnqueueAction<Action<IPrioritizer>>((_) => { arg1.OnBegin(); });
            arg2.onUpdate += this.EnqueueAction<Action<IPrioritizer, float>>((_, deltaTime) => { arg1.OnUpdate(deltaTime); });
            arg2.onLeaveDetails += this.EnqueueAction<Action<IPrioritizer, PriorityStateLeaveDetails>>((_, details) => {
                switch (details.LeaveType) {
                    case PriorityStateLeaveType.TimeOut: {
                        arg1.OnFinish(new StatusFinishDetails() { finishType = StatusFinishType.NormalFinish, initiator = (PriorityState)details.Initiator });
                        break;
                    }
                    case PriorityStateLeaveType.InitiativeInvoke: {
                        arg1.OnFinish(new StatusFinishDetails { finishType = StatusFinishType.InitiativeInvoke });
                        break;
                    }
                    case PriorityStateLeaveType.ReEnter:
                    case PriorityStateLeaveType.Exclusion: {
                        arg1.OnFinish(new StatusFinishDetails { finishType = StatusFinishType.ForcedFinish, initiator = (PriorityState)details.Initiator });
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        protected override void OnDecombin(Status arg1, PriorityState arg2) {
            arg1.beginInvoke -= this.DequeueAction<Action>();
            arg1.finishInvoke -= this.DequeueAction<Action>();
            arg1.isEnterInvoke -= this.DequeueAction<Func<bool>>();

            arg2.onEnter -= this.DequeueAction<Action<IPrioritizer>>();
            arg2.onRefresh -= this.DequeueAction<Action<IPrioritizer>>();
            arg2.onUpdate -= this.DequeueAction<Action<IPrioritizer, float>>();
            arg2.onLeaveDetails -= this.DequeueAction<Action<IPrioritizer, PriorityStateLeaveDetails>>();
        }
    }
}