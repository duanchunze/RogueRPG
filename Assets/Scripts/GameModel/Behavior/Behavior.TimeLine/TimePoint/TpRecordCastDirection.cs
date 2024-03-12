using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable]
    public partial class TpRecordCastDirection : TpInfo<timeline.RecordCastDirectionInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.AttachedBodied;
                    if (owner == null)
                        break;

                    var directions = this.manager.Blackboard.GetOrCreateData<List<Vector3>>("AbilityCastDirections");
                    directions.Clear();
                    foreach (var target in ability.targets) {
                        var dir = target.transform.Position - owner.transform.Position;
                        directions.Add(dir.normalized);
                    }

                    break;
                }
            }
        }
    }
}