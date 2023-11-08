using System;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class TimeInfoManager : Singleton<TimeInfoManager> {
        [ShowInInspector, ReadOnly]
        private float _deltaTime;

        [ShowInInspector, ReadOnly]
        private int _frameCount;

        private DateTime _dt1970;
        private DateTime _dt;

        [ShowInInspector, ReadOnly]
        private long _now;

        private long _nowRaw;

        [ShowInInspector, ReadOnly]
        private float _time;

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
        public float DeltaTime => this._deltaTime;
        public int FrameCount => this._frameCount;
        public float Time => this._time;
#endif
        public long Now => this._now;

        public TimeInfoManager() {
            this._dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this._dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.UpdateNow();
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