using System;

namespace Hsenl {
    public class UnityLogger : ILog {
        public void Trace(object msg) {
            UnityEngine.Debug.Log(msg);
        }

        public void Debug(object msg) {
            UnityEngine.Debug.Log(msg);
        }

        public void Info(object msg) {
            UnityEngine.Debug.Log(msg);
        }

        public void Warning(object msg) {
            UnityEngine.Debug.LogWarning(msg);
        }

        public void Error(object msg) {
            UnityEngine.Debug.LogError(msg);
        }

        public void Error(Exception e) {
            UnityEngine.Debug.LogException(e);
        }

        public void Trace(string message, params object[] args) {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public void Warning(string message, params object[] args) {
            UnityEngine.Debug.LogWarningFormat(message, args);
        }

        public void Info(string message, params object[] args) {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public void Debug(string message, params object[] args) {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public void Error(string message, params object[] args) {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }
    }
}