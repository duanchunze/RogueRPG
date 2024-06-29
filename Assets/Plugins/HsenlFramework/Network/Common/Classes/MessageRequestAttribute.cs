using System;

namespace Hsenl.Network {
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public class MessageRequestAttribute : MessageAttribute {
        public Type ResponseType { get; }

        public MessageRequestAttribute(Type responseType) {
            this.ResponseType = responseType;
        }
    }
}