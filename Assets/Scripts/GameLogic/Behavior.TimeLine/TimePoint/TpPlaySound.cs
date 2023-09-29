using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpPlaySound : TpInfo<timeline.PlaySoundInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var sound = ability.ParentSubstantive.GetComponent<Sound>();
                    sound.Play(this.info.ClipName);
                    break;
                }
            }
        }
    }
}