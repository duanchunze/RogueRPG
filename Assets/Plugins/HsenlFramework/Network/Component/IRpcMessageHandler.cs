using System;

namespace Hsenl.Network {
    public interface IRpcMessageHandler : IMessageHandler {
        public Type RequestType { get; }
        public Type ResponseType { get; }
    }
}