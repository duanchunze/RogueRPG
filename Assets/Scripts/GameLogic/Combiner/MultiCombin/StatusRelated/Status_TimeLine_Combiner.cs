using System;

namespace Hsenl.MultiCombiner {
    public class Status_TimeLine_Combiner : MultiCombiner<Status, TimeLine> {
        protected override void OnCombin(Status arg1, TimeLine arg2) {
            arg1.onBegin += this.EnqueueAction<Action>(arg2.Reset);

            arg1.onUpdate += this.EnqueueAction<Action<float>>(deltaTime => {
                arg2.Run(deltaTime);
                if (arg2.IsFinish) {
                    arg1.Finish();
                }
            });

            arg1.onFinish += this.EnqueueAction<Action<StatusFinishDetails>>(details => { arg2.Abort(); });
        }

        protected override void OnDecombin(Status arg1, TimeLine arg2) {
            arg1.onBegin -= this.DequeueAction<Action>();
            arg1.onUpdate -= this.DequeueAction<Action<float>>();
            arg1.onFinish -= this.DequeueAction<Action<StatusFinishDetails>>();
        }
    }
}