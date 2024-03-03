using System;

namespace Hsenl {
    public class ParseMessageException : Exception {
        public ParseMessageException(string message) : base(message) { }
    }
}