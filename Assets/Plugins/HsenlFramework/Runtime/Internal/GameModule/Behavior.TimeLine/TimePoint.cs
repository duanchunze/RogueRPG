using System;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public abstract class TimePoint : ActionNode<ITimeLine>, ITimeNode {
        public TimePointModel checkModel;
        public float point;

        [ShowInInspector]
        protected bool isPassed;

        protected override void OnNodeReset() {
            this.isPassed = false;
        }

        private void GetRealValue(out float realPoint) {
            switch (this.checkModel) {
                case TimePointModel.Actual:
                    realPoint = this.point;
                    return;

                case TimePointModel.Percent:
                    // 如果时间线的总时间是无限, 且时间点的模式是百分比, 那么把时间点按照0来处理, 也就是一开始就触发
                    var currTime = this.manager.Time;
                    var totalTime = this.manager.TillTime;
                    if (totalTime < 0 || totalTime < currTime) {
                        realPoint = 0;
                    }
                    else {
                        realPoint = totalTime * this.point;
                    }

                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool OnNodeEvaluate() {
            if (this.isPassed) return false;
            this.GetRealValue(out var realPoint);
            var currTime = this.manager.Time;
            return currTime >= realPoint;
        }

        protected override NodeStatus OnNodeTick() {
            this.isPassed = true;
            this.OnTimePointTrigger();
            return NodeStatus.Success;
        }

        protected abstract void OnTimePointTrigger();
    }
}