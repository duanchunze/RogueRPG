using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    public abstract partial class CePickTargetBase<T> : CeInfo<T> where T : casterevaluate.CasterEvaluateInfo {
        protected Transform tran;
        protected SelectorDefault _selector;
        protected List<Numerator> _numerators = new(2);
        protected Faction _faction;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this.tran = owner?.transform;
                    this._selector = owner?.GetComponent<SelectorDefault>();

                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = owner?.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);

                    this._faction = owner?.GetComponent<Faction>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            var target = this.manager.castParameter.target;
            SelectionTargetDefault needApproachTarget = null;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (this._numerators.Count == 0)
                        goto SUCC;

                    // 如果施法器指定了目标, 就用施法器指定的目标
                    if (target != null) {
                        ability.targets.Clear();

                        // 障碍物的优先级很高, 因为如果有障碍物, 某种程度相当于该目标不存在, 所以他在距离判断, 角度判断等等的前面
                        if (Shortcut.HasObstacles(this.tran.Position, target.transform.Position)) {
                            // 有障碍物
                            this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.HasObstacles;
                            goto FAIL;
                        }

                        // 通过阵营获取目标的标签范围
                        var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                        // 如果没有设置基础数值, 说明没有对这些条件进行限制
                        var hasCrange = this._numerators[0].IsHasValue(NumericType.Crange);
                        var hasDrange = this._numerators[0].IsHasValue(NumericType.Drange);
                        var hasCangle = this._numerators[0].IsHasValue(NumericType.CastAngle);
                        // 获取施法范围
                        Num castRange = hasCrange ? GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Crange) : 0f;
                        Num detectRange = hasDrange ? GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Drange) : 0f;
                        Num castAngle = hasCangle ? GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.CastAngle) : 0f;

                        this.manager.castEvaluateResult.ConstrainsTags = constrainsTags;
                        this.manager.castEvaluateResult.CastRange = castRange;
                        this.manager.castEvaluateResult.DetectRange = detectRange;

                        var extraRet = this.ExtraCheckTarget_Front(ref target);
                        if (extraRet != NodeStatus.Success) {
                            goto FAIL;
                        }

                        if (hasCrange) {
                            if (Vector3.Distance(this.tran.Position, target.transform.Position) > castRange) {
                                this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.DistanceDeficiency;
                                goto FAIL;
                            }
                        }

                        if (hasCangle) {
                            if (!Shortcut.IsTargetInView(this.tran, target.transform, castAngle)) {
                                // 距离够了, 但角度不够
                                this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.AngeleDeficiency;
                                goto FAIL;
                            }
                        }

                        extraRet = this.ExtraCheckTarget_Latter(ref target);
                        if (extraRet != NodeStatus.Success) {
                            goto FAIL;
                        }

                        ability.targets.Add(target);

                        goto SUCC;
                    }
                    else {
                        // 通过阵营获取目标的标签范围
                        var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                        // 获取目标数
                        var targetCount = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Ttc);
                        if (targetCount <= 0) {
                            targetCount = 1;
                        }

                        // 如果没有设置基础数值, 说明没有对这些条件进行限制
                        var hasCrange = this._numerators[0].IsHasValue(NumericType.Crange);
                        var hasDrange = this._numerators[0].IsHasValue(NumericType.Drange);
                        var hasCangle = this._numerators[0].IsHasValue(NumericType.CastAngle);
                        // 获取施法范围
                        Num castRange = hasCrange ? GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Crange) : 0f;
                        Num detectRange = hasDrange ? GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Drange) : 0f;
                        Num castAngle = hasCangle ? GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.CastAngle) : 0f;

                        this.manager.castEvaluateResult.ConstrainsTags = constrainsTags;
                        this.manager.castEvaluateResult.CastRange = castRange;
                        this.manager.castEvaluateResult.DetectRange = detectRange;

                        // 根据施法范围获取目标
                        ability.targets.Clear();
                        if (hasCrange) {
                            this.SelectTarget(this._selector, castRange, constrainsTags, null, targetCount).WrapAs(ability.targets);
                        }

                        if (ability.targets.Count != 0) {
                            target = ability.targets[0];
                            // 注释原因: 让目标的碰撞体积也算在距离检测参考之内, 比如面对一个巨大的目标, 则不需要非要跑到其脚底下
                            // // 因为上面是通过collider来判断的, 有体型的误差, 所以这里再计算一次精确的
                            // if (hasCrange) {
                            //     if (Vector3.Distance(this.tran.Position, target.transform.Position) > castRange) {
                            //         target = null;
                            //     }
                            // }
                        }

                        // 对目标的状况进行判断
                        if (target != null) {
                            if (hasCangle) {
                                if (!Shortcut.IsTargetInView(this.tran, target.transform, castAngle)) {
                                    // 距离够了, 但角度不够
                                    this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.AngeleDeficiency;
                                    goto FAIL;
                                }
                            }

                            // 距离够, 角度也够
                            goto SUCC;
                        }

                        // 如果施法范围内没有获取到目标, 则根据检测范围来获取目标
                        // 获取检测范围 = 技能的检测范围 + 技能本身的施法范围
                        if (hasCrange) {
                            if (detectRange > 0f) {
                                var finalDetectRange = detectRange + castRange;
                                ability.targets.Clear();
                                this.SelectTarget(this._selector, finalDetectRange, constrainsTags, null, targetCount).WrapAs(ability.targets);
                                if (ability.targets.Count != 0) {
                                    needApproachTarget = ability.targets[0];
                                    this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.DistanceDeficiency;
                                    goto FAIL;
                                }
                            }
                        }

                        // 找不到目标
                        this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.PickTargetFailure;
                        goto FAIL;
                    }
                }
            }

            NodeStatus ret;
            RUNNING:
            ret = NodeStatus.Running;
            goto RET;

            SUCC:
            ret = NodeStatus.Success;
            goto RET;

            FAIL:
            ret = NodeStatus.Failure;

            RET:
            if (target != null)
                this.manager.castEvaluateResult.CastedTarget = target;
            else if (needApproachTarget != null)
                this.manager.castEvaluateResult.CastedTarget = needApproachTarget;

            return ret;
        }

        public virtual NodeStatus ExtraCheckTarget_Front(ref SelectionTargetDefault target) {
            return NodeStatus.Success;
        }

        public virtual NodeStatus ExtraCheckTarget_Latter(ref SelectionTargetDefault target) {
            return NodeStatus.Success;
        }

        protected abstract ASelectionsSelect SelectTarget(SelectorDefault selector, float range, IReadOnlyBitlist constrainsTags,
            IReadOnlyBitlist exclusiveTags,
            int count);
    }
}