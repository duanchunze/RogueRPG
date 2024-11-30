using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class TimerManager : Singleton<TimerManager> {
        private readonly Queue<ITimerAction> _timerActionQueue = new();
        private readonly Dictionary<float, Clock> _timeLockers = new();

        private long _now;
        private float _time;
        private int _frameCount;

        protected override void OnUnregister() {
            this._now = 0;
            this._time = 0;
            this._frameCount = 0;
            this._timerActionQueue.Clear();
            this._timeLockers.Clear();
        }

        public void Update() {
            this.LockClock();

            this._now = TimeInfo.Now;
            this._time = TimeInfo.Time;
            this._frameCount = TimeInfo.FrameCount;

            var count = this._timerActionQueue.Count;
            while (count-- > 0) {
                var action = this._timerActionQueue.Dequeue();
                if (action.Check(this)) {
                    try {
                        action.Invoke();
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }

                    continue;
                }

                this._timerActionQueue.Enqueue(action);
            }
        }

        private void AddTimerAction(ITimerAction timerAction) {
            this._timerActionQueue.Enqueue(timerAction);
        }

        public async HTask WaitFrame(int count = 1) {
            if (count == 0) return;

            var countNow = this._frameCount;
            var task = HTask.Create();
            var timer = TimerAction3.Create(countNow + count, task);
            this.AddTimerAction(timer);
            await task;
        }

        public async HTask WaitTime(long time) {
            if (time == 0) return;

            var timeNow = this._now;
            var task = HTask.Create();
            var timer = TimerAction.Create(timeNow + time, task);
            this.AddTimerAction(timer);
            await task;
        }

        public async HTask WaitTimeWithScale(float time) {
            if (time == 0) return;

            var timeNow = this._time;
            var task = HTask.Create();
            var timer = TimerAction2.Create(timeNow + time, task);
            this.AddTimerAction(timer);
            await task;
        }

        public async HTask WaitTillTime(long tillTime) {
            var timeNow = this._now;
            if (timeNow >= tillTime) return;

            var task = HTask.Create();
            var timer = TimerAction.Create(tillTime, task);
            this.AddTimerAction(timer);
            await task;
        }

        public async HTask WaitTillTimeWithScale(float tillTime) {
            var timeNow = this._time;
            if (timeNow >= tillTime) return;

            var task = HTask.Create();
            var timer = TimerAction2.Create(tillTime, task);
            this.AddTimerAction(timer);
            await task;
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

        private interface ITimerAction {
            public bool Check(TimerManager manager);
            public void Invoke();
        }

        private class TimerAction : ITimerAction {
            private long _tillTime;
            private HTask _task;

            public static TimerAction Create(long tillTime, HTask task) {
                var timerAction = ObjectPool.Rent<TimerAction>();
                timerAction._tillTime = tillTime;
                timerAction._task = task;
                return timerAction;
            }

            public bool Check(TimerManager manager) {
                var now = manager._now;
                return now >= this._tillTime;
            }

            public void Invoke() {
                this._task.SetResult();
                this.Recycle();
            }

            private void Recycle() {
                this._tillTime = 0;
                this._task = default;
                ObjectPool.Return(this);
            }
        }

        private class TimerAction2 : ITimerAction {
            private float _tillTime;
            private HTask _task;

            public static TimerAction2 Create(float tillTime, HTask task) {
                var timerAction = ObjectPool.Rent<TimerAction2>();
                timerAction._tillTime = tillTime;
                timerAction._task = task;
                return timerAction;
            }

            public bool Check(TimerManager manager) {
                var now = manager._time;
                return now >= this._tillTime;
            }

            public void Invoke() {
                this._task.SetResult();
                this.Recycle();
            }

            private void Recycle() {
                this._tillTime = 0;
                this._task = default;
                ObjectPool.Return(this);
            }
        }

        private class TimerAction3 : ITimerAction {
            private int _frameCount;
            private HTask _task;

            public static TimerAction3 Create(int frameCount, HTask task) {
                var timerAction = ObjectPool.Rent<TimerAction3>();
                timerAction._frameCount = frameCount;
                timerAction._task = task;
                return timerAction;
            }

            public bool Check(TimerManager manager) {
                var count = manager._frameCount;
                return count >= this._frameCount;
            }

            public void Invoke() {
                this._task.SetResult();
                this.Recycle();
            }

            private void Recycle() {
                this._frameCount = 0;
                this._task = default;
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