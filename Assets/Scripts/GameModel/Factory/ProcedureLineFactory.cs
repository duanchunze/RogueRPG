using System;
using System.Collections.Generic;

namespace Hsenl {
    [FrameworkMember]
    public static class ProcedureLineFactory {
        private static readonly Dictionary<Type, Type> _procedureTypeMap = new();

        [OnEventSystemInitialized]
        private static void Cache() {
            _procedureTypeMap.Clear();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(ProcedureLineWorkerAttribute))) {
                var infoType = type.BaseType?.GetGenericArguments()[0];
                if (infoType == null) throw new Exception($"{type}'s base type is error");
                _procedureTypeMap[infoType] = type;
            }
        }

        private static Type GetTypeOfInfoType(Type type) {
            _procedureTypeMap.TryGetValue(type, out var result);
            return result;
        }

        public static T CreateWorker<T>(procedureline.WorkerInfo info) where T : IProcedureLineWorker {
            var nodeType = GetTypeOfInfoType(info.GetType());
            if (nodeType == null) throw new Exception($"cant find procedure line worker info of '{info.GetType()}'");
            var node = (T)Activator.CreateInstance(nodeType);
            ((IConfigInfoInitializer<procedureline.WorkerInfo>)node).InitInfo(info);
            return node;
        }
    }
}