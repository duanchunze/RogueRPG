using System;

namespace Hsenl {
    public static class HTaskExtension {
        public static void Cancel(this HTask<bool> self) {
            if (self.body is not CancelableHTask cancelableHTask) {
                throw new OperationCanceledException("Only cancelable task can Cancel");
            }
            
            cancelableHTask.Cancel();
        }
    }
}