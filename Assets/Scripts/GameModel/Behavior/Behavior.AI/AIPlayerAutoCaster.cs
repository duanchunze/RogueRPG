using System;
using System.Collections.Generic;
using Hsenl.behavior;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 玩家的自动施法器. 按照卡牌栏的头部顺序, 依次释放, 头部是技能卡牌
    [MemoryPackable()]
    public partial class AIPlayerAutoCaster : AIInfo<ai.PlayerAutoCasterInfo> {
        private AbilitesBar _abilitesBar;
        private List<Caster> _casters = new();

        // private float maxFrameTime = 1f / 30;
        // private float frametime = 0;

        // private int currentTryingPosition;

        protected override void OnEnable() {
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

            // Debug.Log("card changed");
            // foreach (var caster in this._casters) {
            //     Debug.Log(caster.Name);
            // }
        }

        protected override bool Check() {
            // this.frametime += TimeInfo.DeltaTime;
            // if (this.frametime < this.maxFrameTime) {
            //     return;
            // }
            //
            // this.frametime = 0;

            var succ = false;
            for (var i = 0; i < this._casters.Count; i++) {
                var caster = this._casters[i];
                var castStatus = caster.CastStart();
                switch (castStatus) {
                    case CastEvaluateStatus.Success:
                        succ = true;
                        break;
                    case CastEvaluateStatus.Trying:
                        succ = true;
                        break;
                    case CastEvaluateStatus.PriorityStateEnterFailure:
                        break;
                    case CastEvaluateStatus.PickTargetFailure:
                        break;
                    case CastEvaluateStatus.Cooldown:
                        break;
                    case CastEvaluateStatus.Mana:
                        break;
                    case CastEvaluateStatus.MoreThanMaxSummoningNum:
                        break;
                }
            }

            return succ;
        }

        protected override void Enter() { }

        protected override void Running() { }

        protected override void Exit() { }
    }
}