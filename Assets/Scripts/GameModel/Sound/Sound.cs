using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class Sound : Unbodied, IUpdate {
        public AudioSource audioSource;

        public HashSet<int> clipsCurrentFrame = new();

        public void Play(string clipName, float volume = 1f, bool allowRepeatPlay = false) {
            if (!allowRepeatPlay) {
                if (this.clipsCurrentFrame.Contains(clipName.GetHashCode())) {
                    return;
                }

                this.clipsCurrentFrame.Add(clipName.GetHashCode());
            }

            if (clipName == "xxx")
                return;
            var clip = ResourcesHelper.GetAsset<AudioClip>(Constant.AudioBundleName, clipName);
            this.audioSource.clip = clip;
            this.audioSource.volume = volume;
            this.audioSource.Play();
        }

        public void Update() {
            this.clipsCurrentFrame.Clear();
        }
    }
}