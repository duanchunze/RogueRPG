using System;
using MemoryPack;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpResurgence : TpInfo<timeline.ResurgenceInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied.MainBodied) {
                case Actor actor: {
                    actor.GetComponent<ProcedureLine>().StartLine(new PliResurgenceForm() {
                        inflictor = (this.manager.Bodied as Status)?.inflictor,
                        target = actor,
                    });

                    break;
                }
            }
        }
    }
}