using System;

namespace Hsenl.MultiCombiner {
    public class Status_StatusBar_Combiner : CrossCombiner<Status, StatusBar> {
        protected override void OnCombin(Status arg1, StatusBar arg2) {
            arg1.onBegin += this.EnqueueAction<Action>(() => {
                arg2.OnStatusEnter(arg1);
            });
            arg1.onFinish += this.EnqueueAction<Action<StatusFinishDetails>>((details) => {
                arg2.OnStatusLeave(arg1, details);
            });
        }

        protected override void OnDecombin(Status arg1, StatusBar arg2) {
            arg1.onBegin -= this.DequeueAction<Action>();
            arg1.onFinish -= this.DequeueAction<Action<StatusFinishDetails>>();
        }
    }
}