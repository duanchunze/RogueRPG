using System;
using MemoryPack;

namespace Hsenl.Network {
    [MessageHandler]
    public abstract class AMessageHandler<T> : IMessageHandler where T : IMessage {
        public Type MessageType => typeof(T);

        void IMessageHandler.Handler(Span<byte> message, Network network, long channelId) {
            var t = MemoryPackSerializer.Deserialize<T>(message);
            this.Handle(t, network, channelId);
        }

        protected abstract void Handle(T message, Network network, long channelId);
    }

    [MessageHandler]
    public abstract class AMessageHandler<TRequest, TResponse> : IRpcMessageHandler where TRequest : IRpcMessage where TResponse : IRpcMessage {
        public Type MessageType => typeof(TRequest);
        public Type RequestType => typeof(TRequest);
        public Type ResponseType => typeof(TResponse);

        void IMessageHandler.Handler(Span<byte> message, Network network, long channelId) {
            var request = MemoryPackSerializer.Deserialize<TRequest>(message);
            var response = this.Handle(request, channelId);
            network.SendWithRpcId(response, request.RpcId, channelId);
        }

        protected abstract TResponse Handle(TRequest message, long channelId);
    }

    [MessageHandler]
    public abstract class AMessageHandlerAsync<TRequest, TResponse> : IRpcMessageHandler where TRequest : IRpcMessage where TResponse : IRpcMessage {
        public Type MessageType => typeof(TRequest);
        public Type RequestType => typeof(TRequest);
        public Type ResponseType => typeof(TResponse);

        void IMessageHandler.Handler(Span<byte> message, Network network, long channelId) {
            var request = MemoryPackSerializer.Deserialize<TRequest>(message);
            this.Run(request, network, channelId);
        }

        private async void Run(TRequest request, Network network, long channelId) {
            var response = await this.Handle(request, channelId);
            network.SendWithRpcId(response, request.RpcId, channelId);
        }

        protected abstract HTask<TResponse> Handle(TRequest message, long channelId);
    }
}