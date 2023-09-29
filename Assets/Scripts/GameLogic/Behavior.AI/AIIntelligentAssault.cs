using System;
using System.Collections.Generic;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    public enum InterlligentAssaultType {
        Assault, // 进攻
        Opportunist, // 伺机而动
    }

    [MemoryPackable()]
    public partial class AIIntelligentAssault : AIInfo<ai.IntelligentAssaultInfo> {
        private Substantive _self;
        private Numerator _selfNumerator;
        private Selector _selfSelector;
        private Faction _selfFaction;
        private Control _selfControl;
        private StatusBar _statusBar;

        private CardBar _cardBar;
        private List<Caster> _casters = new();

        private float breathingTime;
        private float breathingTimer;
        private bool BreathingDone => this.breathingTimer >= this.breathingTime;
        private int ads; // 攻击欲望

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Actor actor: {
                    this._self = actor;
                    this._selfNumerator = actor.GetComponent<Numerator>();
                    this._selfSelector = actor.GetComponent<Selector>();
                    this._selfFaction = actor.GetComponent<Faction>();
                    this._selfControl = actor.GetComponent<Control>();
                    this._statusBar = actor.FindSubstaintiveInChildren<StatusBar>();
                    if (this._statusBar != null) {
                        this._statusBar.onStatusEnter += this.OnStatusAdd;
                    }

                    this._cardBar = actor.GetSubstaintiveInChildren<CardBar>();
                    if (this._cardBar != null) {
                        this._cardBar.onChanged += this.OnCardBarChanged;
                        this.OnCardBarChanged();
                    }

                    this.ads = this._selfNumerator.GetValue(NumericType.Ads);
                    this._selfNumerator.OnNumericChanged += this.OnNumericChanged;

                    this.breathingTime = (100 - this.ads) / 10f;
                    this.breathingTimer = this.breathingTime;
                    break;
                }
            }
        }

        protected override void OnNodeClose() {
            if (this._cardBar != null) {
                this._cardBar.onChanged -= this.OnCardBarChanged;
            }

            if (this._statusBar != null) {
                this._statusBar.onStatusEnter -= this.OnStatusAdd;
            }

            this._selfNumerator.OnNumericChanged -= this.OnNumericChanged;
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
            foreach (var headCard in this._cardBar.GetHeadCards()) {
                if (headCard.Source is not Ability ability) continue;
                var controlTrigger = ability.GetComponent<ControlTrigger>();
                if (controlTrigger == null) continue;
                if (controlTrigger.ControlCode != (int)ControlCode.AutoTrigger) continue;
                this._casters.Add(ability.GetComponent<Caster>());
            }
        }

        protected override bool Check() {
            if (this._selfSelector.PrimarySelection != null) {
                // 计算距离, 超出侦查范围时, 清空目标
                // var dis = math.distancesq(this._self.transform.position, this._selfSelections.PrimarySelection.transform.position);
                // var srange = this._selfNumerator.GetValue(NumericType.Srange);
                // if (dis > math.pow(srange, 2)) {
                //     this._selfSelections.PrimarySelection = null;
                // }
            }


            if (this._selfSelector.PrimarySelection == null) {
                var constrainsTags = this._selfFaction.GetTagsOfFactionType(FactionType.Enemy);
                var srange = this._selfNumerator.GetValue(NumericType.Srange);
                this._selfSelector.PrimarySelection = this._selfSelector.SelectShpereAliveNearestTarget(srange, constrainsTags);
            }

            return this._selfSelector.PrimarySelection != null;
        }

        protected override void Enter() { }

        protected override void Running() {
            if (this.breathingTimer < this.breathingTime) {
                this.breathingTimer += TimeInfo.DeltaTime;
            }

            var attackAbilityCastSuccess = false; // 是否成功释放了一个攻击类技能
            foreach (var caster in this._casters) {
                // 挨个评估每个施法器
                var castEvaluateStatus = caster.Evaluate();
                switch (castEvaluateStatus) {
                    case CastEvaluateStatus.Success:
                        // 如果一个攻击类技能评估成功, 则需要判断是否呼吸是否完成了
                        if (caster.Tags.Contains(TagType.AbilityAttack)) {
                            if (this.BreathingDone) {
                                caster.CastStart();
                                attackAbilityCastSuccess = true;
                            }
                        }

                        break;
                    case CastEvaluateStatus.PriorityStateEnterFailure:
                        break;
                    case CastEvaluateStatus.PickTargetFailure:
                        // 选择目标失败, 距离不够, 可以选择靠近目标, 可以选择徘徊, 都行
                        this._selfControl.SetValue(ControlCode.MoveOfPoint, this._selfSelector.PrimarySelection.transform.Position);
                        break;
                    case CastEvaluateStatus.Trying:
                        break;
                    case CastEvaluateStatus.Cooldown:
                        break;
                }
            }

            if (attackAbilityCastSuccess) {
                this.breathingTimer = 0;
            }
        }

        protected override void Exit() {
            this._selfControl.SetEnd(ControlCode.MoveOfPoint);
        }
    }
}