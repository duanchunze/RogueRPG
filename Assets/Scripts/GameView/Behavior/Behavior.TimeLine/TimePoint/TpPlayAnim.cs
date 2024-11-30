using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl.View {
    [Serializable]
    [MemoryPackable]
    public partial class TpPlayAnim : TpInfo<timeline.PlayAnimInfo> {
        private Motion _motion;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability:
                    this._motion = owner?.GetComponent<Motion>();
                    break;

                case Status status:
                    this._motion = owner?.GetComponent<Motion>();
                    break;
            }
        }

        protected override void OnUpdate() {
            if (this._motion == null)
                return;

            if (this.isPassed) {
                this._motion.SetSpeed(this.info.Anim, this.manager.Speed);
            }
        }

        protected override void OnTimePointTrigger() {
            if (!this._motion.Lock) {
                this._motion.Play(this.info.Anim, this.manager.Speed);
            }
        }
    }
}