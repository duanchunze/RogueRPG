using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    /* 把施法器单独摘出来. 为什么要把施法器从技能里拆出来? 而不是放在一起, 因为不是只有技能才能施法, 道具也可能可以施法, 装备也可能可以施法.
     */
    [Serializable]
    [MemoryPackable()]
    public partial class Caster : Unbodied, IUpdate {
        [MemoryPackIgnore]
        public Func<float, CastEvaluateStatus> evaluateInvokes;

        [MemoryPackIgnore]
        public Action castStartInvoke;

        [MemoryPackIgnore]
        public Action castEndInvoke;

        [MemoryPackIgnore]
        public Func<CastModel> getCastModelInvoke;

        [MemoryPackIgnore]
        public Action onEnter;

        [MemoryPackIgnore]
        public Action<float> onUpdate;

        [MemoryPackIgnore]
        public Action onLeave;

        [MemoryPackIgnore]
        public Action<CasterLeaveDetails> onLeaveDetails;

        [MemoryPackIgnore]
        public Action<Object> onIntercepted; // 当被拦截

        [MemoryPackIgnore]
        public Action<Object> onBreak; // 当被打断

        [MemoryPackIgnore]
        public Action onFinish; // 当完成

        [MemoryPackIgnore]
        private bool _keepTrying;

        [MemoryPackIgnore]
        public CastModel CastModel => this.getCastModelInvoke?.Invoke() ?? CastModel.InfiniteTime;

        public void Update() {
            if (this._keepTrying) {
                var evaluateStatus = this.Evaluate(TimeInfo.DeltaTime);
                if (evaluateStatus != CastEvaluateStatus.Success) {
                    if (evaluateStatus != CastEvaluateStatus.Trying) {
                        // 如果评估不成功, 且没处于尝试状态, 则直接放弃, 不用再继续尝试了.
                        this._keepTrying = false;
                    }

                    return;
                }

                this.castStartInvoke?.Invoke();
                this._keepTrying = false;
            }
        }

        public CastEvaluateStatus Evaluate() {
            return this.Evaluate(TimeInfo.DeltaTime);
        }

        public CastEvaluateStatus Evaluate(float deltaTime) {
            if (this.evaluateInvokes == null) {
                return CastEvaluateStatus.Success;
            }

            return this.evaluateInvokes.Invoke(deltaTime);
        }

        /// <param name="keepTrying">为true则会不断的尝试, 即便不在调用该方法, 或者CastEnd, 也不会停止, 直到caster成功, 或者尝试失败</param>
        /// <returns></returns>
        public CastEvaluateStatus CastStart(bool keepTrying = false) {
            if (this._keepTrying)
                return CastEvaluateStatus.Trying;

            var evaluateStatus = this.Evaluate(TimeInfo.DeltaTime);
            if (evaluateStatus != CastEvaluateStatus.Success) {
                this._keepTrying = keepTrying && evaluateStatus == CastEvaluateStatus.Trying;
                return evaluateStatus;
            }

            this.castStartInvoke?.Invoke();
            return CastEvaluateStatus.Success;
        }

        /// <summary>
        /// 直接开始, 不进行评估
        /// </summary>
        public void CastStartDirect() {
            this.castStartInvoke?.Invoke();
        }

        public void CastEnd() {
            this.castEndInvoke?.Invoke();
        }

        public void OnEnter() {
            this.onEnter?.Invoke();
        }

        public void OnUpdate(float deltaTime) {
            this.onUpdate?.Invoke(deltaTime);
        }

        public void OnLeave() {
            this.onLeave?.Invoke();
        }

        public void OnLeaveDetail(CasterLeaveDetails leaveDetails) {
            this.onLeaveDetails?.Invoke(leaveDetails);
        }

        // 技能被拦截 (在准备阶段之前技能就被取消了, 视为被拦截, 技能并没有被真正释放, 比如一些靠普攻触发的技能, 又能触发的时候, 就把普攻中断了)
        public void OnIntercepted(Object interceptor) {
            this.onIntercepted?.Invoke(interceptor);
        }

        // 技能被打断了 (在准备、蓄力、释放三个阶段退出, 都视为打断)
        public void OnBreak(Object breaker) {
            this.onBreak?.Invoke(breaker);
        }

        public void OnFinish() {
            this.onFinish?.Invoke();
        }
    }
}