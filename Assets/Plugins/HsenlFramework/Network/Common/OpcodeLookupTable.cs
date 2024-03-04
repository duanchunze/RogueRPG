using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl.Network {
    [FrameworkMember]
    public static class OpcodeLookupTable {
        private static readonly BidirectionalDictionary<ushort, Type> _opcodeLookupTable = new();

        [OnEventSystemInitialized]
        private static void InitOpcodes() {
            _opcodeLookupTable.Clear();
            var messageTypes = EventSystem.GetTypesOfAttribute(typeof(MessageAttribute), true);
            ushort id = 1;
            foreach (var type in messageTypes) {
                var attr = type.GetCustomAttribute<MessageAttribute>();
                if (attr == null)
                    continue;

                if (attr is MessageRequestAttribute requestAttribute) {
                    _opcodeLookupTable.Add(id++, type);
                }
                else if (attr is MessageResponseAttribute responseAttribute) {
                    _opcodeLookupTable.Add(id++, type);
                }
                else {
                    _opcodeLookupTable.Add(id++, type);
                }
            }
        }

        public static Type GetTypeOfOpcode(ushort id) {
            if (_opcodeLookupTable.TryGetValueByKey(id, out var type)) {
                return type;
            }

            return null;
        }

        public static ushort GetOpcodeOfType(Type type) {
            if (_opcodeLookupTable.TryGetKeyByValue(type, out var id)) {
                return id;
            }

            return 0;
        }
    }
}