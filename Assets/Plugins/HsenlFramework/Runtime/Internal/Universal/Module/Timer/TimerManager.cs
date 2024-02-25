using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class TimerManager : Singleton<TimerManager> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly SortedMultiList<long, long> _times = new(); // key: time, value: timer id;

        private readonly Dictionary<long, TimerAction> _timerActions = new();
        private readonly Queue<long> _timeOUtTimers = new();
        private readonly Queue<long> _timeOUtTimerIds = new();

        private long _idGenerator;
        private long _minTime = long.MaxValue;
        private long GetId() => this._idGenerator++;
        private long GetNow() => TimeInfo.Now;

        public void Update() {
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
    }
}