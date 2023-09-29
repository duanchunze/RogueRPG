using System;
using MemoryPack;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpResurgence : TpInfo<timeline.ResurgenceInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Status status: {
                    var holder = status.GetHolder();
                    var numerator = holder.GetComponent<Numerator>();
                    Shortcut.RecoverHealth(numerator, int.MaxValue);
                    
                    break;
                }
            }
        }
    }
}