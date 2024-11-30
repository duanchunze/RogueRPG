using System;

namespace Hsenl {
    public class LogStopwatch {
        private string _stopwatchTitle;

        private DateTime _now;

        public LogStopwatch(string title) {
            this._stopwatchTitle = title;
            this._now = DateTime.Now;
            Log.Debug($"{title} (开始)");
        }

        public void Peek(string content) {
            Log.Debug($"{content} ({(DateTime.Now - this._now).TotalMilliseconds} ms)");
            this._now = DateTime.Now;
        }

        public void Peek(ref long tick) {
            tick = (DateTime.Now - this._now).Ticks;
            this._now = DateTime.Now;
        }
        
        public void Peek() {
            this._now = DateTime.Now;
        }
    }
}