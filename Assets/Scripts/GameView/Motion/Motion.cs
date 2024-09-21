using System;
using UnityEngine;

namespace Hsenl.View {
    [Serializable]
    public class Motion : Unbodied {
        public Animation animation;
        public Animator animator;
        public float fadeLength = 0.35f;

        public Func<string, AnimationClip> getAnimClipInvoke;

        private static readonly int _stop = Animator.StringToHash("stop");

        protected override void OnDestroy() {
            this.animation = null;
            this.animator = null;
            this.fadeLength = 0.15f;
        }

        public void Play(in string clipName, float speed = 1f, bool reenter = true) {
            if (this.animation != null) {
                var state = this.animation[clipName];
                if (state == null) {
                    var clip = this.getAnimClipInvoke?.Invoke(clipName);
                    if (clip == null) {
                        Log.Error($"Load animation clip '{clipName}' fail!");
                        return;
                    }

                    this.animation.AddClip(clip, clipName);
                    state = this.animation[clipName];
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
            if (this.animation != null) {
                this.animation.Stop(clipName);
            }
            else { }
        }

        public void SetSpeed(in string clipName, float speed) {
            if (this.animation != null) {
                var state = this.animation[clipName];
                if (state == null) {
                    return;
                }

                state.speed = speed;
            }
            else {
                this.animator.speed = speed;
            }
        }

        public void Clear() {
            this.animation = null;
            this.animator = null;
        }
    }
}