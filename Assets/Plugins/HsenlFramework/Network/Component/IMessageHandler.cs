using System;

namespace Hsenl.Network {
    public interface IMessageHandler {
        public Type MessageType { get; }
        public void Handler(Span<byte> message, Network network, long channelId);
    }
}