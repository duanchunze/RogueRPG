﻿using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpPlayAnim : TpInfo<timeline.PlayAnimInfo> {
        private Motion _motion;

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Ability ability:
                    this._motion = ability.GetHolder()?.GetComponent<Motion>();
                    break;

                case Status status:
                    this._motion = status.GetHolder()?.GetComponent<Motion>();
                    break;
            }
        }

        protected override bool OnNodeEvaluate() {
            var ret = base.OnNodeEvaluate();
            if (this.isPassed) {
                // if (this._motion != null)
                //     Debug.Log($"set anim speed '{this.manager.Speed}'");
                this._motion?.SetSpeed(this.info.Anim, this.manager.Speed);
            }

            return ret;
        }

        protected override void OnTimePointTrigger() {
            this._motion?.Play(this.info.Anim, this.manager.Speed);
        }
    }
}