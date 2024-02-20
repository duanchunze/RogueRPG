using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Hsenl {
    public class LogProxy : SerializedMonoBehaviour {
        [OdinSerialize, HideLabel]
        private LogManager _logManager;
        
        private void Awake() {
            if (!SingletonManager.IsDisposed<LogManager>()) {
                SingletonManager.Unregister<LogManager>();
            }
            
            SingletonManager.Register(ref this._logManager);
            HTask.ExceptionHandler += Log.Error;
        }
    }
}