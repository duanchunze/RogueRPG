using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class AIIntelligentAssault : AIInfo<ai.IntelligentAssaultInfo> {
        private Numerator _numerator;
        private SelectorDefault _selector;
        private Faction _faction;
        private Control _control;

        private StatusBar _statusBar;
        private AbilitesBar _abiBar;
        private List<Caster> _casters = new();

        private float breathingTime;
        private float breathingTillTime;
        private bool BreathingDone => TimeInfo.Time >= this.breathingTillTime;
        private int ads; // 攻击欲望

        private Vector3 _prevPosition;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            this._numerator = owner.GetComponent<Numerator>();
            this._selector = owner.GetComponent<SelectorDefault>();
            this._faction = owner.GetComponent<Faction>();
            this._control = owner.GetComponent<Control>();
            this._statusBar = owner.FindBodiedInIndividual<StatusBar>();
            if (this._statusBar != null) {
                this._statusBar.onStatusEnter += this.OnStatusAdd;
            }

            this._abiBar = owner.FindBodiedInIndividual<AbilitesBar>();
            if (this._abiBar != null) {
                this._abiBar.OnAbilityChanged += this.OnCardBarChanged;
                this.OnCardBarChanged();
            }

            this.ads = this._numerator.GetValue(NumericType.Ads);
            this._numerator.OnNumericChanged += this.OnNumericChanged;

            this.breathingTime = (100 - this.ads) / 10f;
        }

        protected override void OnDisable() {
            if (this._abiBar != null) {
                this._abiBar.OnAbilityChanged -= this.OnCardBarChanged;
            }

            if (this._statusBar != null) {
                this._statusBar.onStatusEnter -= this.OnStatusAdd;
            }

            this._numerator.OnNumericChanged -= this.OnNumericChanged;
        }

        private void OnNumericChanged(Numerator arg1, int numType, Num old, Num now) {
            if (numType == (int)NumericType.Ads) {
                this.ads = now;
                this.breathingTime = (100 - this.ads) / 10f;
            }
        }

        private void OnStatusAdd(Status status) {
            // 如果中了控制类状态, 则进入一段时间的修整期(懵逼期)
            if (status.Tags.Contains(TagType.StatusContorl)) {
                // this.breathingTimer = 0;
            }
        }

        private void OnCardBarChanged() {
            this._casters.Clear();
            foreach (var ability in this._abiBar.ExplicitAbilies) {
                var controlTrigger = ability.GetComponent<ControlTrigger>();
                if (controlTrigger == null) continue;
                if (controlTrigger.ControlCode != (int)ControlCode.AutoTrigger) continue;
                this._casters.Add(ability.GetComponent<Caster>());
            }
        }

        private void OnAbilityChanged() {
            this._casters.Clear();
            for (int i = 0, len = this._abiBar.ExplicitAbilies.Count; i < len; i++) {
                var ability = this._abiBar.ExplicitAbilies[i];
                var controlTrigger = ability.GetComponent<ControlTrigger>();
                if (controlTrigger == null) continue;
                if (controlTrigger.ControlCode != (int)ControlCode.AutoTrigger) continue;
                this._casters.Add(ability.GetComponent<Caster>());
            }
        }


        protected override bool Check() {
            if (this._selector.PrimaryTarget != null) {
                // 计算距离, 超出侦查范围时, 清空目标
                // var dis = math.distancesq(this._self.transform.position, this._selfSelections.PrimarySelection.transform.position);
                // var srange = this._selfNumerator.GetValue(NumericType.Srange);
                // if (dis > math.pow(srange, 2)) {
                //     this._selfSelections.PrimarySelection = null;
                // }

                if (Shortcut.IsDead(this._selector.PrimaryTarget.Bodied)) {
                    this._selector.PrimaryTarget = null;
                }
            }


            if (this._selector.PrimaryTarget == null) {
                var constrainsTags = this._faction.GetTagsOfFactionType(FactionType.Enemy);
                var srange = this._numerator.GetValue(NumericType.Srange);
                var fov = this._numerator.GetValue(NumericType.Fov);
                GameAlgorithm.SpyTarget(this._selector, srange, fov, constrainsTags, null);
            }

            return this._selector.PrimaryTarget != null;
        }

        protected override void Enter() { }

        protected override void Running() {
            if (TimeInfo.Time < this.breathingTillTime) {
                return;
            }

            // if (Vector3.Distance(this._prevPosition, this._control.transform.Position) > 0.001f) {
            //     // 如果怪物处于移动中, 则最少附加0.15秒的喘息时间
            //     this.breathingTimer = this.breathingTime - 0.75f;
            // }
            //
            // this._prevPosition = this._control.transform.Position;

            var attackAbilityCastSuccess = false; // 是否成功释放了一个攻击类技能
            if (Timer.ClockTick(0.15f)) { // 不需要每帧执行, 大概和人按键的速度差不多就行
                foreach (var caster in this._casters) {
                    // 怪物会优先攻击指定的目标
                    caster.castParameter.target = this._selector.PrimaryTarget;
                    // 挨个评估每个施法器
                    var castEvaluateStatus = caster.Evaluate();
                    switch (castEvaluateStatus) {
                        case CastEvaluateState.Success:
                            // 如果一个攻击类技能评估成功, 则需要判断是否呼吸是否完成了
                            if (caster.Tags.Contains(TagType.AbilityAttack)) {
                                if (this.BreathingDone) {
                                    caster.DirectStartCast();
                                    attackAbilityCastSuccess = true;
                                }
                            }

                            break;
                        case CastEvaluateState.PriorityStateEnterFailure:
                            break;
                        case CastEvaluateState.HasObstacles:
                        case CastEvaluateState.PickTargetFailure:
                            // 选择目标失败, 距离不够, 可以选择靠近目标, 可以选择徘徊, 都行
                            Shortcut.SimulatePointMove(this._control, this._selector.PrimaryTarget.transform.Position);
                            break;
                        case CastEvaluateState.Trying:
                            break;
                        case CastEvaluateState.Cooldown:
                            break;
                    }
                }
            }

            if (attackAbilityCastSuccess) {
                this.breathingTillTime = TimeInfo.Time + this.breathingTime;
            }
        }

        protected override void Exit() { }
    }
}