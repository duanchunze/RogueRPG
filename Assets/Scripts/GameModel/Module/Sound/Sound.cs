using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    [Serializable]
    public class Sound : Unbodied, IUpdate {
        private AudioSource _audioSource;

        public HashSet<int> clipsCurrentFrame = new();

        public bool PlayOnAwake {
            get => this._audioSource.playOnAwake;
            set => this._audioSource.playOnAwake = value;
        }

        protected override void OnDeserialized() {
            this._audioSource = this.GetMonoComponent<UnityEngine.AudioSource>();
            if (this._audioSource == null) {
                this._audioSource = this.Entity.GameObject.AddComponent<UnityEngine.AudioSource>();
            }
        }

        protected override void OnConstruction() {
            this._audioSource = this.GetMonoComponent<UnityEngine.AudioSource>();
            if (this._audioSource == null) {
                this._audioSource = this.Entity.GameObject.AddComponent<UnityEngine.AudioSource>();
            }
        }

        public void Play(string clipName, float volume = 1f, bool allowRepeatPlay = false) {
            if (!allowRepeatPlay) {
                if (this.clipsCurrentFrame.Contains(clipName.GetHashCode())) {
                    return;
                }

                this.clipsCurrentFrame.Add(clipName.GetHashCode());
            }

            if (clipName == "xxx")
                return;

            var clip = YooAssets.LoadAssetSync<AudioClip>(clipName).AssetObject as AudioClip;
            this._audioSource.clip = clip;
            this._audioSource.volume = volume;
            this._audioSource.Play();
        }

        public void Update() {
            this.clipsCurrentFrame.Clear();
        }
    }
}