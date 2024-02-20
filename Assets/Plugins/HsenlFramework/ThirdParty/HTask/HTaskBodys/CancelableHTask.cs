using System.Runtime.ExceptionServices;

namespace Hsenl {
    internal class CancelableHTask : NormalHTask<bool> {
        public void Cancel() {
            this._status = HTaskStatus.Succeeded;
            this._value = true;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }
    }
}