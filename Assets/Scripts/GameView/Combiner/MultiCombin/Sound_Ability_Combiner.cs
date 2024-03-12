using System;
using UnityEngine;

namespace Hsenl.View.MultiCombiner {
    public class Sound_Ability_Combiner : MultiCombiner<Sound, Ability> {
        protected override void OnCombin(Sound arg1, Ability arg2) {
            arg1.getAudioClipInvoke += this.EnqueueAction<Func<string, AudioClip>>(LoadAudioClip);
            
            return;

            AudioClip LoadAudioClip(string clipName) {
                var clip = AppearanceSystem.LoadSoundClip(clipName);
                return clip;
            }
        }

        protected override void OnDecombin(Sound arg1, Ability arg2) {
            arg1.getAudioClipInvoke -= this.DequeueAction<Func<string, AudioClip>>();
        }
    }
}