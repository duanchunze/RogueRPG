using System;
using Hsenl.EventType;
using MemoryPack;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpDie : TpInfo<timeline.DieInfo> {
        protected override async void OnTimePointTrigger() {
            switch (this.manager.Bodied.MainBodied) {
                case Actor actor: {
                    var dieForm = new PliDieForm() {
                        inflictor = (this.manager.Bodied as Status)?.inflictor,
                        target = actor,
                    };
                    actor.GetComponent<ProcedureLine>().StartLine(dieForm);
                    break;
                }
            }
        }
    }
}