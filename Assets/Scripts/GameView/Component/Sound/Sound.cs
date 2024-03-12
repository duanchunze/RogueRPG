using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace Hsenl.View {
    [Serializable]
    public class Sound : Unbodied, IUpdate {
        private AudioSource _audioSource;

        public HashSet<int> clipsCurrentFrame = new();

        public Func<string, AudioClip> getAudioClipInvoke;

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

        protected override void OnAwake() {
            this._audioSource = this.GetMonoComponent<UnityEngine.AudioSource>();
            if (this._audioSource == null) {
                this._audioSource = this.Entity.GameObject.AddComponent<UnityEngine.AudioSource>();
            }
        }

        public void Play(string clipName, float volume = 1f, bool allowRepeatPlay = false) {
            if (!allowRepeatPlay) {
                if (!this.clipsCurrentFrame.Add(clipName.GetHashCode())) {
                    return;
                }
            }

            if (clipName == "xxx")
                return;

            var clip = this.getAudioClipInvoke?.Invoke(clipName);
            if (clip == null) {
                Log.Error($"Load audio clip '{clipName}' fail!");
                return;
            }

            this.Play(clip, volume, allowRepeatPlay);
        }

        public void Play(AudioClip clip, float volume = 1f, bool allowRepeatPlay = false) {
            this._audioSource.clip = clip;
            this._audioSource.volume = volume;
            this._audioSource.Play();
        }

        public void Update() {
            this.clipsCurrentFrame.Clear();
        }
    }
}