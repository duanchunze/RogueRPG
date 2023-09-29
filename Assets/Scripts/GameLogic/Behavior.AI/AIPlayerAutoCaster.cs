using System;
using System.Collections.Generic;
using Hsenl.behavior;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 玩家的自动施法器. 按照卡牌栏的头部顺序, 依次释放, 头部是技能卡牌
    [MemoryPackable()]
    public partial class AIPlayerAutoCaster : AIInfo<ai.PlayerAutoCasterInfo> {
        private CardBar _cardBar;
        private List<Caster> _casters = new();

        private float maxFrameTime = 1f / 30;
        private float frametime = 0;

        // private int currentTryingPosition;

        protected override void OnNodeOpen() {
            this._cardBar = this.manager.Substantive.GetSubstaintiveInChildren<CardBar>();
            if (this._cardBar != null) {
                this._cardBar.onChanged += this.OnCardBarChanged;
                this.OnCardBarChanged();
            }
        }

        protected override void OnNodeClose() {
            if (this._cardBar != null) {
                this._cardBar.onChanged -= this.OnCardBarChanged;
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

            // Debug.Log("card changed");
            // foreach (var caster in this._casters) {
            //     Debug.Log(caster.Name);
            // }
        }

        protected override bool Check() {
            return true;
        }

        protected override void Enter() { }

        protected override void Running() {
            // this.frametime += TimeInfo.DeltaTime;
            // if (this.frametime < this.maxFrameTime) {
            //     return;
            // }
            //
            // this.frametime = 0;

            for (var i = 0; i < this._casters.Count; i++) {
                var caster = this._casters[i];
                var castStatus = caster.CastStart();
                switch (castStatus) {
                    case CastEvaluateStatus.Success:
                        break;
                    case CastEvaluateStatus.Trying:
                        break;
                    case CastEvaluateStatus.PriorityStateEnterFailure:
                        break;
                    case CastEvaluateStatus.PickTargetFailure:
                        break;
                    case CastEvaluateStatus.Cooldown:
                        break;
                    case CastEvaluateStatus.Mana:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void Exit() { }
    }
}