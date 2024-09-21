using System;
using System.Collections.Generic;
using Hsenl.casterevaluate;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 尝试选择目标, 如果找不到目标, 则返回失败. 相反, 则返回成功
    [Serializable]
    [MemoryPackable()]
    public partial class CePickTarget : CeInfo<PickTargetInfo> {
        private Selector _selector;
        private List<Numerator> _numerators = new(2);
        private Faction _faction;

        private bool _success;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._selector = owner?.GetComponent<Selector>();
                    // 技能的数值需要合并自己与持有者的数值组件
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

        protected override void OnDisable() {
            this._selector = null;
            this._numerators.Clear();
            this._faction = null;

            this._success = false;
        }

        protected override NodeStatus OnNodeTick() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (this._numerators.Count == 0) goto FAILURE;

                    var castRange = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Crange);
                    var targetCount = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Ttc);
                    var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                    ability.targets.Clear();
                    this._selector
                        .SearcherSphereBody(castRange)
                        .FilterAlive()
                        .FilterTags(constrainsTags, null)
                        .FilterObstacles()
                        .SelectNearests(targetCount)
                        .Wrap(ability.targets);
                    if (ability.targets.Count == 0) {
                        goto FAILURE; // todo 后续继续规范
                    }
                    
                    this._success = true;
                    break;

                    FAILURE:
                    this._success = false;
                    break;
                }
            }

            if (!this._success) {
                this.manager.status = CastEvaluateStatus.PickTargetFailure;
            }

            return this._success ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}