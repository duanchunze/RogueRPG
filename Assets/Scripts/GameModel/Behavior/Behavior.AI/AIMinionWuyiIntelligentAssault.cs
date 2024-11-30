using System.Collections.Generic;
using Hsenl.ai;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class AIMinionWuyiIntelligentAssault : AIInfo<MinionWuyiIntelligentAssaultInfo> {
        private Minion _minion;
        private Numerator _numerator;
        private SelectorDefault _selector;
        private Faction _faction;
        private Control _control;

        private StatusBar _statusBar;
        private AbilitesBar _abiBar;
        private List<Caster> _casters = new();

        private Vector3 _prevPosition;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            this._minion = owner.GetComponent<Minion>();
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
            if (numType == (int)NumericType.Ads) { }
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

        protected override bool Check() {
            SelectionTargetDefault target = null;
            // 如果主人有目标, 则优先以主人的目标
            target = this._minion.master?.GetComponent<SelectorDefault>()?.PrimaryTarget;
            // 如果没主人目标, 则检测自己的目标
            if (target == null)
                target = this._selector.PrimaryTarget;
            // 如果还没有目标, 则尝试侦查敌人
            if (target == null) {
                var constrainsTags = this._faction.GetTagsOfFactionType(FactionType.Enemy);
                var srange = this._numerator.GetValue(NumericType.Srange);
                var fov = this._numerator.GetValue(NumericType.Fov);
                GameAlgorithm.SpyTarget(this._selector, srange, fov, constrainsTags, null);
            }

            if (target != null) {
                if (Shortcut.IsDead(target.Bodied)) {
                    target = null;
                }
            }

            this._selector.PrimaryTarget = target;
            return this._selector.PrimaryTarget != null;
        }

        protected override void Enter() { }

        protected override void Running() {
            Caster currCaster = null;
            // 不需要每帧执行, 大概和人按键的速度差不多就行
            if (Timer.ClockTick(0.15f)) {
                foreach (var caster in this._casters) {
                    currCaster = caster;
                    currCaster.castParameter.target = this._selector.PrimaryTarget;
                    // 挨个评估每个施法器
                    var castEvaluateStatus = caster.Evaluate();
                    switch (castEvaluateStatus) {
                        case CastEvaluateState.Success:
                            caster.DirectStartCast();
                            break;
                        case CastEvaluateState.PriorityStateEnterFailure:
                            break;
                        case CastEvaluateState.PickTargetFailure: {
                            goto 徘徊;
                            break;
                        }
                        case CastEvaluateState.Trying:
                            break;
                        case CastEvaluateState.Cooldown: {
                            // goto 徘徊;
                            break;
                        }
                        case CastEvaluateState.NotBackForMe:
                            goto 去背后;
                    }
                }
            }

            return;

            去背后:
            if (this._minion.transform.IsMoveStop()) {
                var target = this._selector.PrimaryTarget.transform;
                var randomDir = -target.Forward * Quaternion.CreateAngleAxis(Vector3.Up, RandomHelper.NextFloat(-90f, 90f));
                var range = currCaster.castEvaluateResult.CastRange;
                var randomPos = target.Position + randomDir * RandomHelper.NextFloat(1f, range);
                Shortcut.SimulatePointMove(this._control, randomPos);
                return;
            }

            徘徊:
            if (this._minion.transform.IsMoveStop()) {
                var target = this._selector.PrimaryTarget.transform;
                var randomDir = -target.Forward * Quaternion.CreateAngleAxis(Vector3.Up, RandomHelper.NextFloat(-90f, 90f));
                var range = currCaster.castEvaluateResult.CastRange + currCaster.castEvaluateResult.DetectRange;
                var randomPos = target.Position + randomDir * RandomHelper.NextFloat(1f, range);
                Shortcut.SimulatePointMove(this._control, randomPos);
                return;
            }
        }

        protected override void Exit() { }
    }
}