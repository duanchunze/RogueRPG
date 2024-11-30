﻿using Hsenl.timeline;
using MemoryPack;

namespace Hsenl.View {
    [MemoryPackable]
    public partial class TpUnlockAnim : TpInfo<UnlockAnimaInfo> {
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

        protected override void OnTimePointTrigger() {
            if (this._motion == null)
                return;
            
            this._motion.Lock = false;
        }
    }
}