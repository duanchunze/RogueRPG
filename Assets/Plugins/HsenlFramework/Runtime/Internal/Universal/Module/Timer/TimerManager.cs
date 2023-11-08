using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class TimerManager : Singleton<TimerManager> {
        [ShowInInspector, ReadOnly]
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
            var tcs = (ETTask)timerAction.obj;
            tcs.SetResult();
            timerAction.Recycle();
        }

        public async ETTask WaitFrame(ETCancellationToken cancellationToken = null) {
            await this.WaitTime(1, cancellationToken);
        }

        public async ETTask WaitTime(long time, ETCancellationToken cancellationToken = null) {
            if (time == 0) return;

            var timeNow = this.GetNow();
            var tcs = ETTask.Create(true);
            var timer = TimerAction.Create(this.GetId(), timeNow, time, tcs);
            this.AddTimeAction(timer);
            var timerId = timer.id;

            void CancelAction() {
                if (this.RemoveAction(timerId)) {
                    tcs.SetResult();
                }
            }

            try {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally {
                cancellationToken?.Remove(CancelAction);
            }
        }

        public async ETTask WaitTillTime(long tillTime, ETCancellationToken cancellationToken = null) {
            var timeNow = this.GetNow();
            if (timeNow >= tillTime) return;

            var tcs = ETTask.Create(true);
            var timer = TimerAction.Create(this.GetId(), timeNow, tillTime - timeNow, tcs);
            this.AddTimeAction(timer);
            var timerId = timer.id;

            void CancelAction() {
                if (this.RemoveAction(timerId)) {
                    tcs.SetResult();
                }
            }

            try {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally {
                cancellationToken?.Remove(CancelAction);
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
    }
}