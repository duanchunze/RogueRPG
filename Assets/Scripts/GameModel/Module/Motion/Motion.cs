using System;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class Motion : Unbodied {
        public bool useLegacy;
        public Animation animation;
        public Animator animator;
        public float fadeLength = 0.35f;

        private static readonly int _stop = Animator.StringToHash("stop");

        protected override void OnDestroy() {
            this.animation = null;
            this.animator = null;
            this.fadeLength = 0.15f;
        }

        public void Play(in string clipName, float speed = 1f, bool reenter = true) {
            if (this.useLegacy) {
                var state = this.animation[clipName];
                if (state == null) {
                    Log.Error($"animation clip '{clipName}' not exist");
                    return;
                }

                state.speed = speed;
                if (reenter) {
                    this.animation.Stop(clipName);
                }

                this.animation.CrossFade(clipName, this.fadeLength);
            }
            else {
                this.animator.SetTrigger(clipName);
                this.animator.speed = speed;
            }
        }

        public void Stop(in string clipName) {
            if (this.useLegacy) {
                this.animation.Stop(clipName);
            }
            else { }
        }

        public void SetSpeed(in string clipName, float speed) {
            if (this.useLegacy) {
                var state = this.animation[clipName];
                if (state == null) {
                    Log.Error($"set anim speed error, animation clip '{clipName}' not exist");
                    return;
                }

                state.speed = speed;
            }
            else {
                this.animator.speed = speed;
            }
        }
    }
}