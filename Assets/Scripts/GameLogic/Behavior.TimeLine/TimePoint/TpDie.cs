using System;
using Hsenl.EventType;
using MemoryPack;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpDie : TpInfo<timeline.DieInfo> {
        protected override async void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Status status: {
                    var actor = status.GetHolder().GetComponent<Actor>();
                    actor.GetMonoComponent<NavMeshAgent>().enabled = false;

                    await Timer.WaitTime(3000);

                    if (actor == GameManager.Instance.MainMan)
                        Shortcut.InflictionStatus(null, actor, StatusAlias.Resurgence);
                    else
                        ActorManager.Instance.Return(actor);
                    break;
                }
            }
        }
    }
}