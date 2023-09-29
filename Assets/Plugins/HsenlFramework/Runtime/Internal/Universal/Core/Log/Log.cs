using System;
using System.Globalization;

namespace Hsenl {
    public static class Log {
        public static void Trace(object msg) {
            LogManager.Instance.Trace(msg);
        }

        public static void Debug(object msg) {
            LogManager.Instance.Debug(msg);
        }

        public static void Info(object msg) {
            LogManager.Instance.Info(msg);
        }

        public static void TraceInfo(object msg) {
            LogManager.Instance.Trace(msg);
        }

        public static void Warning(object msg) {
            LogManager.Instance.Warning(msg);
        }

        public static void Error(object msg) {
            LogManager.Instance.Error(msg);
        }

        public static void Error(Exception e) {
            LogManager.Instance.Error(e);
        }

        public static void Trace(string message, params object[] args) {
            LogManager.Instance.Trace(message, args);
        }

        public static void Warning(string message, params object[] args) {
            LogManager.Instance.Warning(string.Format(message, args));
        }

        public static void Info(string message, params object[] args) {
            LogManager.Instance.Info(string.Format(message, args));
        }

        public static void Debug(string message, params object[] args) {
            LogManager.Instance.Debug(string.Format(message, args));
        }

        public static void Error(string message, params object[] args) {
            LogManager.Instance.Error(message, args);
        }

        private static string GetString(object message) {
            if (message == null)
                return "Null";
            return message is IFormattable formattable ? formattable.ToString((string)null, (IFormatProvider)CultureInfo.InvariantCulture) : message.ToString();
        }
    }
}