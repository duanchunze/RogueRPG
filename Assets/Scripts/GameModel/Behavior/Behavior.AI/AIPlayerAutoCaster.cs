using System;
using System.Collections.Generic;
using Hsenl.behavior;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 玩家的自动施法器. 按照卡牌栏的头部顺序, 依次释放, 头部是技能卡牌
    [MemoryPackable]
    public partial class AIPlayerAutoCaster : AIInfo<ai.PlayerAutoCasterInfo> {
        private SelectorDefault _selector;
        private Control _control;
        private AbilitesBar _abilitesBar;
        private List<Caster> _casters = new();

        protected override void OnEnable() {
            this._selector = this.manager.Bodied.MainBodied.GetComponent<SelectorDefault>();
            this._control = this.manager.Bodied.MainBodied.GetComponent<Control>();
            this._abilitesBar = this.manager.Bodied.MainBodied.FindBodiedInIndividual<AbilitesBar>();
            if (this._abilitesBar != null) {
                this._abilitesBar.OnAbilityChanged += this.OnAbilitesBarChanged;
                this.OnAbilitesBarChanged();
            }
        }

        protected override void OnDisable() {
            if (this._abilitesBar != null) {
                this._abilitesBar.OnAbilityChanged -= this.OnAbilitesBarChanged;
            }
        }

        private void OnAbilitesBarChanged() {
            this._casters.Clear();
            foreach (var ability in this._abilitesBar.ExplicitAbilies) {
                var controlTrigger = ability.GetComponent<ControlTrigger>();
                if (controlTrigger == null) continue;
                if (controlTrigger.ControlCode != (int)ControlCode.AutoTrigger) continue;
                this._casters.Add(ability.GetComponent<Caster>());
            }
        }

        protected override bool Check() {
            return true;
        }

        protected override void Enter() { }

        protected override void Running() {
            {
                var target = this._selector.PrimaryTarget;
                if (target != null) {
                    if (Shortcut.IsDead(target.Bodied)) {
                        this._selector.PrimaryTarget = null;
                    }
                }
            }

            Caster caster;
            // if (Timer.ClockTick(0.2f)) { // todo 这个相当与每秒按5下的手速, 后续可以把这个作为参数放到游戏设置里
                for (var i = 0; i < this._casters.Count; i++) {
                    var isbreak = false;
                    caster = this._casters[i];
                    // 这个ai是模仿手按的, 所以, 调用这个
                    var castStatus = caster.StartCastWithKeepTrying();
                    switch (castStatus) {
                        case CastEvaluateState.Invalid:
                            break;
                        case CastEvaluateState.Success:
                            break;
                        case CastEvaluateState.Trying:
                            isbreak = true;
                            break;
                        case CastEvaluateState.Cooldown:
                            break;
                        case CastEvaluateState.Mana:
                            break;
                        case CastEvaluateState.PriorityStateEnterFailure:
                            break;
                        case CastEvaluateState.NoTarget:
                            break;
                        case CastEvaluateState.TargetFactionDiscrepancy:
                            break;
                        case CastEvaluateState.PickTargetFailure:
                            break;
                        case CastEvaluateState.DistanceDeficiency:
                            break;
                        case CastEvaluateState.AngeleDeficiency:
                            break;
                        case CastEvaluateState.MoreThanMaxSummoningNum:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // 以技能选中的目标作为自己的主目标
                    if (caster.castEvaluateResult.CastedTarget != null)
                        this._selector.PrimaryTarget = caster.castEvaluateResult.CastedTarget;

                    if (isbreak)
                        break;
                }
            // }

            return;

            // SEARCH_TARGET: // 
            // {
            //     var crange = caster.castEvaluateResult.CastRange;
            //     var drange = caster.castEvaluateResult.DetectRange;
            //     var constrainsTags = caster.castEvaluateResult.ConstrainsTags;
            //     this._selector.PrimarySelection = GameAlgorithm.SelectTarget(this._selector, crange + drange, constrainsTags, null).Target;
            //     return;
            // }
            //
            // APPROACH_TARGET:
            // // 检测距离够, 但施法距离不够
            // {
            //     var crange = caster.castEvaluateResult.CastRange;
            //     var drange = caster.castEvaluateResult.DetectRange;
            //     var target = caster.castEvaluateResult.CastedTarget;
            //     var selfPosition = this._selector.transform.Position;
            //     var targetPosition = target.transform.Position;
            //     var dis = Vector3.Distance(selfPosition, targetPosition);
            //     dis -= crange;
            //     var dir = (targetPosition - selfPosition).normalized;
            //     var point = selfPosition + dir * dis;
            //     Shortcut.SimulatePointMove(this._control, point);
            //     return;
            // }
            //
            // TURNAROUND:
            // {
            //     var target = caster.castEvaluateResult.CastedTarget;
            //     Shortcut.TurnAround(this._control, target.transform.Position - this._selector.transform.Position);
            //     return;
            // }
        }

        protected override void Exit() { }
    }
}