using System;
using System.Collections.Generic;

namespace Hsenl.Network {
    [FrameworkMember]
    public static class MessageDispatcher {
        private static readonly Dictionary<Type, IMessageHandler> _messageHandlers = new();

        public static void DispatcherMessage(Type type, Span<byte> message, Network network, long channelId) {
            if (_messageHandlers.TryGetValue(type, out var messageHandler)) {
                messageHandler.Handler(message, network, channelId);
            }
        }

        [OnEventSystemInitialized]
        private static void CacheHandlers() {
            _messageHandlers.Clear();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(MessageHandlerAttribute))) {
                if (type.IsGenericType || type.IsAbstract) continue;
                var obj = Activator.CreateInstance(type);
                if (obj is not IMessageHandler handler) {
                    throw new InvalidOperationException($"type '{type}' is not MessageHandler");
                }

                if (handler is IRpcMessageHandler rpcMessageHandler) {
                    var responseType = OpcodeLookupTable.GetResponseTypeOfRequestType(rpcMessageHandler.RequestType);
                    if (rpcMessageHandler.ResponseType != responseType) {
                        Log.Error($"RpcMessageHandler error: RequestMessage does not match RequestMessage '{rpcMessageHandler.RequestType}' '{rpcMessageHandler.ResponseType}'");
                        continue;
                    }
                }

                _messageHandlers.Add(handler.MessageType, handler);
            }
        }
    }
}