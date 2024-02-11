using System;
using MemoryPack;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpResurgence : TpInfo<timeline.ResurgenceInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.AttachedBodied) {
                case Actor actor: {
                    var numerator = actor.GetComponent<Numerator>();
                    Shortcut.RecoverHealth(numerator, int.MaxValue);
                    Shortcut.RecoverMana(numerator, int.MaxValue);
                    
                    break;
                }
            }
        }
    }
}