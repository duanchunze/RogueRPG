using System;
using System.Collections.Generic;

namespace Hsenl {
    [FrameworkMember]
    public static class BehaviorNodeManager {
        private static readonly Dictionary<Type, Type> caches = new(); // key: info的具体类型, value: node

        [OnEventSystemInitialized]
        private static void Cache() {
            caches.Clear();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(BehaviorNodeAttribute))) {
                var infoType = type.BaseType?.GetGenericArguments()[0];
                if (infoType == null) throw new Exception($"{type}'s base type is error");
                caches[infoType] = type;
            }
        }
        
        public static Type GetTypeOfInfoType(Type type) {
            caches.TryGetValue(type, out var result);
            return result;
        }
    }
}