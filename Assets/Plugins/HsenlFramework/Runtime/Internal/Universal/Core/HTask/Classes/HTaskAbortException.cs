using System;

namespace Hsenl {
    public class HTaskAbortException : Exception {
        private static HTaskAbortException _taskAbortException;

        public static HTaskAbortException GetException() => _taskAbortException ??= new HTaskAbortException();
    }
}