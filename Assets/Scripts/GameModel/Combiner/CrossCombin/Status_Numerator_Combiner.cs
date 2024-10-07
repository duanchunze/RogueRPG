using System;

namespace Hsenl.CrossCombiner {
    public class Status_Numerator_Combiner : CrossCombiner<Status, Numerator> {
        protected override void OnCombin(Status arg1, Numerator arg2) {
            arg1.onFinish += this.EnqueueAction<Action<StatusFinishDetails>>(details => {
                // 状态结束时, 把叠加层数归零
                if (arg2.IsHasValue(NumericType.Stack)) {
                    arg2.SetValue(NumericType.Stack, 0);
                }
            });
        }

        protected override void OnDecombin(Status arg1, Numerator arg2) {
            arg1.onFinish -= this.DequeueAction<Action<StatusFinishDetails>>();
        }
    }
}