using System;

namespace Hsenl.Network {
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public class MessageResponseAttribute : BaseAttribute {
        public Type RequestType { get; }

        public MessageResponseAttribute(Type requestType) {
            this.RequestType = requestType;
        }
    }
}