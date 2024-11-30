using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class Caster : BehaviorTree<INode<Caster>> {
        public event Func<CastModel> getCastModelInvoke;

        // ---

        public event Func<float, CastEvaluateState> castEvaluateInvoke;
        public event Func<CastEvaluateState> startCastInvoke;
        public event Func<bool> isCastingInvoke;
        public event Action stopCastInvoke;
        public event Action startKeepTryInvoke;
        public event Func<bool> isKeepTringInvoke;
        public event Action stopKeepTringInvoke;

        // ---

        public event Action onCastStart;
        public event Action<float> onCastRunning;
        public event Action<CasterEndDetails> onCastEnd;

        // ---

        [MemoryPackIgnore]
        public CastParameter castParameter = new();

        [MemoryPackIgnore]
        public CastEvaluateResult castEvaluateResult = new();

        [MemoryPackIgnore]
        public CastModel CastModel => this.getCastModelInvoke?.Invoke() ?? default;

        [MemoryPackIgnore]
        public bool IsCasting => this.isCastingInvoke?.Invoke() ?? false;

        public CastEvaluateState Evaluate() {
            return this.Evaluate(TimeInfo.DeltaTime);
        }

        public CastEvaluateState Evaluate(float deltaTime) {
            if (this.castEvaluateInvoke != null) {
                return this.castEvaluateInvoke?.Invoke(deltaTime) ?? CastEvaluateState.Invalid;
            }

            this.castEvaluateResult.Reset();
            this.DeltaTime = deltaTime;
            var ret = this.Tick();

            // 评估行为的规则是, 成功代表true, running代表可以成功但要等一会, 其他的都是代表失败, 只是失败的类型很多, 这里使用一个变量, 由外部赋值
            switch (ret) {
                case NodeStatus.Success: {
                    return CastEvaluateState.Success;
                }

                case NodeStatus.Running: {
                    return CastEvaluateState.Trying;
                }

                default:
                    return this.castEvaluateResult.CastEvaluateState;
            }
        }

        public CastEvaluateState StartCast() {
            if (this.isKeepTringInvoke?.Invoke() ?? false)
                this.stopKeepTringInvoke!.Invoke();

            var evaluate = this.Evaluate();
            if (evaluate != CastEvaluateState.Success)
                return evaluate;

            var ret = this.startCastInvoke?.Invoke() ?? CastEvaluateState.Invalid;
            return ret;
        }

        /// <summary>
        /// 释放, 并在符合条件的情况下, 保持尝试
        /// </summary>
        /// <returns></returns>
        public CastEvaluateState StartCastWithKeepTrying() {
            if (this.isKeepTringInvoke?.Invoke() ?? false)
                return CastEvaluateState.Trying;

            var evaluate = this.Evaluate();
            if (evaluate != CastEvaluateState.Success) {
                if (evaluate == CastEvaluateState.Trying) {
                    this.startKeepTryInvoke?.Invoke();
                }

                // 评估失败
                return evaluate;
            }

            // 开始释放
            var ret = this.startCastInvoke?.Invoke() ?? CastEvaluateState.Invalid;
            return ret;
        }

        /// <summary>
        /// 直接释放, 跳过评估
        /// </summary>
        /// <returns></returns>
        public CastEvaluateState DirectStartCast() {
            if (this.isKeepTringInvoke?.Invoke() ?? false)
                this.stopKeepTringInvoke!.Invoke();

            return this.startCastInvoke?.Invoke() ?? CastEvaluateState.Invalid;
        }

        public void StopCast() {
            this.stopCastInvoke?.Invoke();
        }

        public void StopKeepTrying() {
            this.stopKeepTringInvoke?.Invoke();
        }

        public void OnCastStart() {
            try {
                this.onCastStart?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnCastRunning(float deltaTime) {
            try {
                this.onCastRunning?.Invoke(deltaTime);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnCastEnd(CasterEndDetails endDetails) {
            try {
                this.onCastEnd?.Invoke(endDetails);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}