using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class TimerManagerOld : Singleton<TimerManagerOld> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly SortedMultiList<long, long> _times = new(); // key: time, value: timer id;

        private readonly Dictionary<long, TimerAction> _timerActions = new();
        private readonly Queue<long> _timeOUtTimers = new();
        private readonly Queue<long> _timeOUtTimerIds = new();
        private readonly Dictionary<float, Clock> _timeLockers = new();

        private long _idGenerator;
        private long _minTime = long.MaxValue;
        private long GetId() => this._idGenerator++;
        private long GetNow() => TimeInfo.Now;

        protected override void OnUnregister() {
            this._idGenerator = 0;
            this._times.Clear();
            this._timerActions.Clear();
            this._timeOUtTimers.Clear();
            this._timeOUtTimerIds.Clear();
            this._timeLockers.Clear();
        }

        public void Update() {
            this.LockClock();

            if (this._times.Count == 0) return;

            var timeNow = this.GetNow();
            if (timeNow < this._minTime) return;

            foreach (var kv in this._times) {
                var tillTime = kv.Key;
                if (tillTime > timeNow) {
                    this._minTime = tillTime;
                    break;
                }

                this._timeOUtTimers.Enqueue(tillTime);
            }

            while (this._timeOUtTimers.Count > 0) {
                var tillTime = this._timeOUtTimers.Dequeue();
                var list = this._times[tillTime];
                for (int i = 0, len = list.Count; i < len; i++) {
                    var timerId = list[i];
                    this._timeOUtTimerIds.Enqueue(timerId);
                }

                this._times.Remove(tillTime);
            }

            while (this._timeOUtTimerIds.Count > 0) {
                var timerId = this._timeOUtTimerIds.Dequeue();
                this.InvokeAction(timerId);
            }
        }

        private void AddTimeAction(TimerAction timerAction) {
            var tillTime = timerAction.startTime + timerAction.time;
            this._times.Add(tillTime, timerAction.id);
            this._timerActions.Add(timerAction.id, timerAction);
            if (tillTime < this._minTime) {
                this._minTime = tillTime;
            }
        }

        private bool RemoveAction(long id) {
            if (id == 0) return false;
            if (!this._timerActions.Remove(id, out var timerAction)) return false;
            timerAction.Recycle();
            return true;
        }

        private void InvokeAction(long id) {
            if (!this._timerActions.Remove(id, out var timerAction)) return;
            var tcs = (HTask)timerAction.obj;
            tcs.SetResult();
            timerAction.Recycle();
        }

        public async HTask WaitFrame() {
            await this.WaitTime(1);
        }

        public async HTask WaitTime(long time) {
            if (time == 0) return;

            var timeNow = this.GetNow();
            var tcs = HTask.Create();
            var timer = TimerAction.Create(this.GetId(), timeNow, time, tcs);
            this.AddTimeAction(timer);
            await tcs;
        }

        public async HTask WaitTillTime(long tillTime) {
            var timeNow = this.GetNow();
            if (timeNow >= tillTime) return;

            var tcs = HTask.Create();
            var timer = TimerAction.Create(this.GetId(), timeNow, tillTime - timeNow, tcs);
            this.AddTimeAction(timer);
            await tcs;
        }

        /// <summary>
        /// 用于代替 Time.FrameCount % 10 == 0 这种写法, 这种写法会受到帧数的影响, 导致间隔时间不稳定, 而闹钟不会.
        /// 不支持多线程.
        /// 要确保每帧都能调用Tick.(因为相同时间的闹钟是公共的, 所以不会单独为谁等待)
        /// </summary>
        /// <param name="timeInterval">闹钟的间隔时间(s)</param>
        /// <returns></returns>
        public bool ClockTick(float timeInterval) {
            if (!this._timeLockers.TryGetValue(timeInterval, out var locker)) {
                locker = new Clock(timeInterval);
                this._timeLockers[timeInterval] = locker;
                if (this._timeLockers.Count > 1000) {
                    Log.Warning($"Clock is too much '{this._timeLockers.Count}'");
                }
            }

            return locker.Tick();
        }

        private void LockClock() {
            List<float> waitClears = null;
            foreach (var kv in this._timeLockers) {
                var locker = kv.Value;
                // 空转超过一定次数, 就清除该闹钟
                if (locker.NullTickCount >= 1000) {
                    waitClears ??= ListComponent<float>.Rent();
                    waitClears.Add(kv.Key);
                    continue;
                }

                if (locker.NUllTick()) { }
            }

            if (waitClears != null) {
                for (int i = 0; i < waitClears.Count; i++) {
                    this._timeLockers.Remove(waitClears[i]);
                }
            }
        }

        [Serializable]
        public class TimerAction {
            public long id;
            public long startTime;
            public long time;
            public object obj;

            public static TimerAction Create(long id, long startTime, long time, object obj) {
                var timerAction = ObjectPool.Rent<TimerAction>();
                timerAction.id = id;
                timerAction.startTime = startTime;
                timerAction.time = time;
                timerAction.obj = obj;
                return timerAction;
            }

            public void Recycle() {
                this.id = 0;
                this.startTime = 0;
                this.time = 0;
                ObjectPool.Return(this);
            }
        }

        private class Clock {
            private readonly float _timeInterval;
            private float _tillTime;
            public int NullTickCount { get; private set; } // 空tick数量, 超出一定次数后, 就代表该timelocker长时间没有被使用了
            public int FrameCount { get; private set; }

            public Clock(float timeInterval) {
                this._timeInterval = timeInterval;
            }

            public bool NUllTick() {
                var ret = TimeInfo.Time >= this._tillTime;
                this.NullTickCount++;
                if (ret) {
                    this.FrameCount = TimeInfo.FrameCount;
                    this.Lock();
                }

                return ret;
            }

            public bool Tick() {
                var ret = TimeInfo.Time >= this._tillTime;
                this.NullTickCount = 0;

                if (!ret && TimeInfo.FrameCount == this.FrameCount)
                    return true;

                return ret;
            }

            private void Lock() {
                this._tillTime = TimeInfo.Time + this._timeInterval;
            }
        }
    }
}