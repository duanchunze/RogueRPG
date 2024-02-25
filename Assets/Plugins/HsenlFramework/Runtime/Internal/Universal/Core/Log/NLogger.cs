#if !UNITY_5_3_OR_NEWER
namespace Hsenl {
    public class NLogger : ILog {
        private readonly NLog.Logger logger;

        static NLogger() {
            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("./Log/NLog.config");
        }

        public NLogger() {
            this.logger = NLog.LogManager.GetLogger("xxxxx");
        }
        
        public void Trace(object message) {
            this.logger.Trace(message);
        }

        public void Warning(object message) {
            this.logger.Warn(message);
        }

        public void Info(object message) {
            this.logger.Info(message);
        }

        public void Debug(object message) {
            this.logger.Debug(message);
        }

        public void Error(object message) {
            this.logger.Error(message);
        }

        public void Trace(string message, params object[] args) {
            this.logger.Trace(message, args);
        }

        public void Warning(string message, params object[] args) {
            this.logger.Warn(message, args);
        }

        public void Info(string message, params object[] args) {
            this.logger.Info(message, args);
        }

        public void Debug(string message, params object[] args) {
            this.logger.Debug(message, args);
        }

        public void Error(string message, params object[] args) {
            this.logger.Error(message, args);
        }
    }
}
#endif