using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hsenl.Network {
    [FrameworkMember]
    public static class OpcodeLookupTable {
        private static readonly BidirectionalDictionary<ushort, Type> _opcodeLookupTable = new();
        private static readonly BidirectionalDictionary<ushort, ushort> _rpcOpcodeMap = new(); // key: request, value: response
        private static readonly BidirectionalDictionary<Type, Type> _rpcTypeMap = new(); // key: request, value: response

        [OnEventSystemInitialized]
        private static void InitOpcodes() {
            _opcodeLookupTable.Clear();
            _rpcOpcodeMap.Clear();
            _rpcTypeMap.Clear();
            var messageTypes = EventSystem.GetTypesOfAttribute(typeof(MessageAttribute), true);
            ushort id = 1;
            foreach (var type in messageTypes) {
                var attr = type.GetCustomAttribute<MessageAttribute>();
                if (attr == null)
                    continue;

                if (attr is MessageRequestAttribute requestAttribute) {
                    _opcodeLookupTable.Add(id, type);
                    if (requestAttribute.ResponseType == null) {
                        Log.Error($"The response message to the request message is not declared '{type}'"); // Rpc消息必须成对声明
                    }
                    else {
                        if (_rpcTypeMap.ContainsValue(requestAttribute.ResponseType)) {
                            Log.Error($"Response message is already exist '{requestAttribute.ResponseType}'"); // 两个 请求消息 指定了同一个 回复消息
                        }
                        else {
                            _rpcTypeMap.Add(type, requestAttribute.ResponseType);
                        }
                    }
                }
                else if (attr is MessageResponseAttribute) {
                    _opcodeLookupTable.Add(id, type);
                }
                else {
                    _opcodeLookupTable.Add(id, type);
                }

                id++;
            }

            foreach (var kv in _rpcTypeMap) {
                var op1 = _opcodeLookupTable[kv.Key];
                var op2 = _opcodeLookupTable[kv.Value];
                _rpcOpcodeMap.Add(op1, op2);
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

        public static Type GetResponseTypeOfRequestType(Type type) {
            if (_rpcTypeMap.TryGetValueByKey(type, out var result)) {
                return result;
            }

            return null;
        }

        public static Type GetRequestTypeOfResponseType(Type type) {
            if (_rpcTypeMap.TryGetKeyByValue(type, out var result)) {
                return result;
            }

            return null;
        }

        public static ushort GetResponseOfRequest(ushort op) {
            if (_rpcOpcodeMap.TryGetValueByKey(op, out var result)) {
                return result;
            }

            return 0;
        }

        public static ushort GetRequestOfResponse(ushort op) {
            if (_rpcOpcodeMap.TryGetKeyByValue(op, out var result)) {
                return result;
            }

            return 0;
        }

        public static bool ContainsRequestOpcode(ushort op) {
            return _rpcOpcodeMap.ContainsKey(op);
        }

        public static bool ContainsResponseOpcode(ushort op) {
            return _rpcOpcodeMap.ContainsValue(op);
        }

        public static bool ContainsRequestType(Type type) {
            return _rpcTypeMap.ContainsKey(type);
        }

        public static bool ContainsResponseType(Type type) {
            return _rpcTypeMap.ContainsValue(type);
        }
    }
}