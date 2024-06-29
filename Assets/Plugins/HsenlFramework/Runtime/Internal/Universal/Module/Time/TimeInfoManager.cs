using System;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class TimeInfoManager : Singleton<TimeInfoManager> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private float _deltaTime; // s

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private int _frameCount;

        private DateTime _dt1970;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private long _now; // ms

        private long _nowRaw; // microsecond

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private float _time; // s

        private long _previousTime;


#if UNITY_5_3_OR_NEWER
        public float DeltaTime => UnityEngine.Time.deltaTime;
        public int FrameCount => UnityEngine.Time.frameCount;
        public float Time => UnityEngine.Time.time;
        public float TimeScale {
            get => UnityEngine.Time.timeScale;
            set => UnityEngine.Time.timeScale = value;
        }
#else
        private float _timeScale = 1;

        public float DeltaTime => this._deltaTime;
        public int FrameCount => this._frameCount;
        public float Time => this._time;
        public float TimeScale {
            get => this._timeScale;
            set => this._timeScale = value;
        }
#endif
        public long Now => this._now;

        public TimeInfoManager() {
            this._dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.UpdateNow();
        }

        protected override void OnUnregister() {
            this._deltaTime = 0;
            this._frameCount = 0;
            this._now = 0;
            this._nowRaw = 0;
            this._time = 0;
            this._previousTime = 0;
        }

        public void Update() {
            this.UpdateNow();
            this._frameCount++;
            this._deltaTime = (this._nowRaw - this._previousTime) * 0.0000001f;
            this._previousTime = this._nowRaw;
            this._time += this.DeltaTime;
        }

        // 当前时间总量（毫秒）
        private void UpdateNow() {
            this._nowRaw = DateTime.UtcNow.Ticks - this._dt1970.Ticks;
            this._now = this._nowRaw / 10000;
        }
    }
}