using System;
using System.Diagnostics;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#endif

namespace Hsenl {
    public enum LogType {
        None = 0,
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warning = 4,
    }

    [Serializable]
    public class LogManager : Singleton<LogManager> {
#if UNITY_5_3_OR_NEWER
        [OdinSerialize]
#endif
#if UNITY_EDITOR
        [ShowInInspector, LabelText("记录者")]
#endif
        private ILog _iLog;

#if UNITY_EDITOR
        [PropertyRange(0, 4)]
#endif
        public int logLevel = 3;

        public void Init(ILog log, int lv) {
            this._iLog = log;
            this.logLevel = lv;
        }

        private const int InfoLevel = 1;
        private const int DebugLevel = 2;
        private const int TraceLevel = 3;
        private const int WarningLevel = 4;

        private bool CheckLogLevel(int level) {
            return this.logLevel >= level;
        }

        public void Trace(object msg) {
            if (!this.CheckLogLevel(DebugLevel)) {
                return;
            }

            StackTrace st = new(2, true);
            this._iLog.Trace($"{msg}\n{st}");
        }

        public void Debug(object msg) {
            if (!this.CheckLogLevel(DebugLevel)) {
                return;
            }

            this._iLog.Debug(msg);
        }

        public void Info(object msg) {
            if (!this.CheckLogLevel(InfoLevel)) {
                return;
            }

            this._iLog.Info(msg);
        }

        public void TraceInfo(object msg) {
            if (!this.CheckLogLevel(InfoLevel)) {
                return;
            }

            StackTrace st = new(2, true);
            this._iLog.Trace($"{msg}\n{st}");
        }

        public void Warning(object msg) {
            if (!this.CheckLogLevel(WarningLevel)) {
                return;
            }

            this._iLog.Warning(msg);
        }

        public void Error(object msg) {
            StackTrace st = new(2, true);
            this._iLog.Error($"{msg}\n{st}");
        }

        public void Error(Exception e) {
            if (e.Data.Contains("StackTrace")) {
                this._iLog.Error($"{e.Data["StackTrace"]}\n{e}");
                return;
            }

            var str = e.ToString();
            this._iLog.Error(str);
        }

        public void Trace(string message, params object[] args) {
            if (!this.CheckLogLevel(TraceLevel)) {
                return;
            }

            StackTrace st = new(2, true);
            this._iLog.Trace($"{string.Format(message, args)}\n{st}");
        }

        public void Warning(string message, params object[] args) {
            if (!this.CheckLogLevel(WarningLevel)) {
                return;
            }

            this._iLog.Warning(string.Format(message, args));
        }

        public void Info(string message, params object[] args) {
            if (!this.CheckLogLevel(InfoLevel)) {
                return;
            }

            this._iLog.Info(string.Format(message, args));
        }

        public void Debug(string message, params object[] args) {
            if (!this.CheckLogLevel(DebugLevel)) {
                return;
            }

            this._iLog.Debug(string.Format(message, args));
        }

        public void Error(string message, params object[] args) {
            StackTrace st = new(2, true);
            var s = string.Format(message, args) + '\n' + st;
            this._iLog.Error(s);
        }

        protected override void OnSingleUnregister() {
            this._iLog = null;
            this.logLevel = 0;
        }
    }
}