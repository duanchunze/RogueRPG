namespace Hsenl {
    public interface ILog {
        void Trace(object message);
        void Warning(object message);
        void Info(object message);
        void Debug(object message);
        void Error(object message);
        void Trace(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Info(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Error(string message, params object[] args);
    }
}