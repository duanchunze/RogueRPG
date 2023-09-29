using System;

namespace Hsenl {
    [Serializable]
    public abstract class TimeSegment : ActionNode<ITimeLine>, ITimeNode {
        public TimePointModel checkModel;
        public float origin;
        public float destination;

        private void GetRealValue(out float realOrig, out float realDest) {
            if (this.origin > this.destination) throw new Exception($"time segment orig can't be greater than dest: '{this.origin}' - '{this.destination}'");
            switch (this.checkModel) {
                case TimePointModel.Actual:
                    realOrig = this.origin;
                    realDest = this.destination;
                    return;

                case TimePointModel.Percent:
                    var currTime = this.manager.Time;
                    var totalTime = this.manager.TillTime;
                    if (totalTime < 0) {
                        realOrig = 0;
                        realDest = float.MaxValue;
                    }
                    else {
                        if (totalTime < currTime) {
                            realOrig = 0;
                            realDest = -1;
                        }
                        else {
                            realOrig = totalTime * this.origin;
                            realDest = totalTime * this.destination;
                        }
                    }

                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool OnNodeEvaluate() {
            this.GetRealValue(out var realOrig, out var realDest);
            var currTime = this.manager.Time;
            return currTime >= realOrig && currTime <= realDest;
        }

        protected override void OnNodeEnter() {
            this.OnTimeSegmentOrigin();
        }

        protected override NodeStatus OnNodeTick() {
            this.OnTimeSegmentRunning();
            return NodeStatus.Running;
        }

        protected override void OnNodeExit() {
            // 因为节点有可能被意外终止, 所以离开时要做判断, 看时间是不是真的到了
            this.GetRealValue(out _, out var realDest);
            var currTime = this.manager.Time;
            if (currTime >= realDest) {
                this.OnTimeSegmentTerminate(true);
            }
            else {
                this.OnTimeSegmentTerminate(false);
            }
        }

        protected abstract void OnTimeSegmentOrigin();

        protected abstract void OnTimeSegmentRunning();

        // timeout: 如果为false, 说明虽然时间段终止了, 但却不是因为时间到了而终止的, 是被迫终止的
        protected abstract void OnTimeSegmentTerminate(bool timeout);
    }
}