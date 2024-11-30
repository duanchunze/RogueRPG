using System;
using System.Collections.Generic;
using Hsenl.casterevaluate;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class CeTargetEvaluate : CeInfo<TargetEvaluateInfo> {
        private Transform _tran;
        private SelectorDefault _selector;
        private List<Numerator> _numerators = new(2);
        private Faction _faction;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._tran = owner?.transform;
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
            var target = this._selector.PrimaryTarget;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (this._numerators.Count == 0)
                        // 没数值器
                        return NodeStatus.Failure;

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
                    this.manager.castEvaluateResult.CastedTarget = target;

                    if (target == null) {
                        this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.NoTarget;
                        return NodeStatus.Failure;
                    }

                    if (constrainsTags != null) {
                        if (!target.Tags.ContainsAny(constrainsTags)) {
                            this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.TargetFactionDiscrepancy;
                            return NodeStatus.Failure;
                        }
                    }

                    if (hasCrange) {
                        var dis = Vector3.Distance(this._tran.Position, target.transform.Position);
                        if (dis > castRange) {
                            this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.DistanceDeficiency;
                            return NodeStatus.Failure;
                        }
                    }

                    if (hasCangle) {
                        if (!Shortcut.IsTargetInView(this._tran, target.transform, castAngle)) {
                            this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.AngeleDeficiency;
                            return NodeStatus.Failure;
                        }
                    }

                    // 如果目标评估合格, 则放入技能目标列表里.
                    ability.targets.Clear();
                    ability.targets.Add(target);
                    return NodeStatus.Success;
                }
            }

            return NodeStatus.Failure;
        }
    }
}