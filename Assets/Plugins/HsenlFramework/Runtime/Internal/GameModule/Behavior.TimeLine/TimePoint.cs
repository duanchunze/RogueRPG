using System;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public abstract class TimePoint : ActionNode<ITimeLine>, ITimeNode {
        public TimePointModel checkModel;
        public float point;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        protected bool isPassed;

        protected sealed override void OnNodeEnter() { }
        protected sealed override void OnNodeRunStart() { }
        protected sealed override void OnNodeRunning() { }
        protected sealed override void OnNodeRunEnd() { }
        protected sealed override void OnNodeExit() { }

        protected override void OnStart() {
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

        protected sealed override bool OnNodeEvaluate() {
            try {
                this.OnUpdate();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            if (this.isPassed) return false;
            this.GetRealValue(out var realPoint);
            var currTime = this.manager.Time;
            return currTime >= realPoint;
        }

        protected sealed override NodeStatus OnNodeTick() {
            this.isPassed = true;
            this.OnTimePointTrigger();
            return NodeStatus.Success;
        }

        protected virtual void OnUpdate() { }
        protected abstract void OnTimePointTrigger();
    }
}