using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class AIMinionDefaultIntelligentAssault : AIInfo<ai.MinionDefaultIntelligentAssaultInfo> {
        private Numerator _numerator;
        private Selector _selector;
        private Faction _faction;
        private Control _control;

        private StatusBar _statusBar;
        private AbilitesBar _abiBar;
        private List<Caster> _casters = new();

        private float breathingTime;
        private float breathingTimer;
        private bool BreathingDone => this.breathingTimer >= this.breathingTime;
        private int ads; // 攻击欲望

        private Vector3 _prevPosition;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            this._numerator = owner.GetComponent<Numerator>();
            this._selector = owner.GetComponent<Selector>();
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
            this.breathingTimer = this.breathingTime;
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
            if (this._selector.PrimarySelection != null) {
                // 计算距离, 超出侦查范围时, 清空目标
                // var dis = math.distancesq(this._self.transform.position, this._selfSelections.PrimarySelection.transform.position);
                // var srange = this._selfNumerator.GetValue(NumericType.Srange);
                // if (dis > math.pow(srange, 2)) {
                //     this._selfSelections.PrimarySelection = null;
                // }

                if (Shortcut.IsDead(this._selector.PrimarySelection.Bodied)) {
                    this._selector.PrimarySelection = null;
                }
            }


            if (this._selector.PrimarySelection == null) {
                var constrainsTags = this._faction.GetTagsOfFactionType(FactionType.Enemy);
                var srange = this._numerator.GetValue(NumericType.Srange);
                var fov = this._numerator.GetValue(NumericType.Fov);
                this._selector.PrimarySelection = this._selector
                    .SearcherSectorBody(srange, fov)
                    .FilterAlive()
                    .FilterTags(constrainsTags, null)
                    .FilterObstacles()
                    .SelectNearest()
                    .Target;
            }

            return this._selector.PrimarySelection != null;
        }

        protected override void Enter() { }

        protected override void Running() {
            if (this.breathingTimer < this.breathingTime) {
                this.breathingTimer += TimeInfo.DeltaTime;
            }

            foreach (var caster in this._casters) {
                // 挨个评估每个施法器
                var castEvaluateStatus = caster.Evaluate();
                switch (castEvaluateStatus) {
                    case CastEvaluateStatus.Success:
                        // 如果一个攻击类技能评估成功, 则需要判断是否呼吸是否完成了
                        if (caster.Tags.Contains(TagType.AbilityAttack)) {
                            if (this.BreathingDone) {
                                caster.CastStart();
                            }
                        }

                        break;
                    case CastEvaluateStatus.PriorityStateEnterFailure:
                        break;
                    case CastEvaluateStatus.PickTargetFailure:
                        // 选择目标失败, 距离不够, 可以选择靠近目标, 可以选择徘徊, 都行
                        this._control.SetValue(ControlCode.MoveOfPoint, this._selector.PrimarySelection.transform.Position);
                        break;
                    case CastEvaluateStatus.Trying:
                        break;
                    case CastEvaluateStatus.Cooldown:
                        break;
                }
            }
        }

        protected override void Exit() {
            this._control.SetEnd(ControlCode.MoveOfPoint);
        }
    }
}