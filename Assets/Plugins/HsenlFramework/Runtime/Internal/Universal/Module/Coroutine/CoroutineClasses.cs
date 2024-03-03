using System;

namespace Hsenl {
    public interface IWait {
        bool Tick();
    }

    /// <summary>
    /// 等待毫秒
    /// </summary>
    public sealed class WaitSeconds : IWait {
        private long _tillTime;

        public WaitSeconds(long ms) {
            this._tillTime = DateTime.Now.Ticks + ms * 10000;
        }

        bool IWait.Tick() {
            return DateTime.Now.Ticks >= this._tillTime;
        }

        public void Reset(long ms) {
            this._tillTime = DateTime.Now.Ticks + ms * 10000;
        }
    }

    /// <summary>
    /// 等待帧
    /// </summary>
    public sealed class WaitFrame : IWait {
        private int _waitFrame;

        public WaitFrame(int frame) {
            this._waitFrame = frame;
        }

        bool IWait.Tick() {
            this._waitFrame--;
            return this._waitFrame <= 0;
        }

        public void Reset() {
            this._waitFrame = 0;
        }
    }

    /// <summary>
    /// 等待条件
    /// </summary>
    public sealed class WaitCondition : IWait {
        private Func<bool> _condition;

        public WaitCondition(Func<bool> condition) {
            this._condition = condition;
        }

        bool IWait.Tick() {
            if (this._condition == null) return true;
            return this._condition.Invoke();
        }

        public void Reset() {
            this._condition = null;
        }
    }

    /// <summary>
    /// 当协程意外退出时，执行回调
    /// </summary>
    public sealed class WhenBreak : IDisposable {
        /// <summary>
        /// 当协程自然结束时，是否执行回调（当值为true时，无论是意外退出，还是正常退出，都会必然执行该回调）
        /// </summary>
        public bool ActingOnEnd { get; private set; }

        public Action Action { get; private set; }

        public WhenBreak(Action action, bool actingOnEnd = true) {
            this.Action = action;
            this.ActingOnEnd = actingOnEnd;
        }

        public void Dispose() {
            this.ActingOnEnd = false;
            this.Action = null;
        }
    }
}