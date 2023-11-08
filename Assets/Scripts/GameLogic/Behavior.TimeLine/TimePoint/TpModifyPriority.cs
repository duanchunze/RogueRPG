using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpModifyPriority : TpInfo<timeline.ModifyPriorityInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var priorityState = ability.GetComponent<IPriorityState>();
                    if (priorityState == null) return;
                    if (this.info.ResistPriority != -1) {
                        priorityState.ResistPriority = this.info.ResistPriority;
                    }
                    else {
                        priorityState.ResistPriority = priorityState.ResistPriorityAnchor;
                    }

                    break;
                }
            }
        }
    }
}