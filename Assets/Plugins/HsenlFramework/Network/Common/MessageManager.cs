using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl.Network {
    [FrameworkMember]
    public static class MessageManager {
        private static readonly BidirectionalDictionary<ushort, Type> _messageTypeLookupTable = new();

        [OnEventSystemInitialized]
        private static void InitMessages() {
            _messageTypeLookupTable.Clear();
            var messageTypes = EventSystem.GetTypesOfAttribute(typeof(MessageAttribute));
            ushort id = 1;
            foreach (var type in messageTypes) {
                var attr = type.GetCustomAttribute<MessageAttribute>();
                if (attr == null)
                    continue;

                if (attr is MessageRequestAttribute requestAttribute) {
                    _messageTypeLookupTable.Add(id++, type);
                }
                else if (attr is MessageResponseAttribute responseAttribute) {
                    _messageTypeLookupTable.Add(id++, type);
                }
                else {
                    _messageTypeLookupTable.Add(id++, type);
                }
            }
        }

        public static Type GetMessageTypeOfId(ushort id) {
            if (_messageTypeLookupTable.TryGetValueByKey(id, out var type)) {
                return type;
            }

            return null;
        }

        public static ushort GetMessageIdOfType(Type type) {
            if (_messageTypeLookupTable.TryGetKeyByValue(type, out var id)) {
                return id;
            }

            return 0;
        }
    }
}