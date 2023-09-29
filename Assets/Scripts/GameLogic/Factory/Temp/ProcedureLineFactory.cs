using System;
using Hsenl.cast;

namespace Hsenl {
    public static class ProcedureLineFactory {
        public static T CreateWorker<T>(ConditionCastOfWorkerInfo info) where T : IProcedureLineWorker {
            var nodeType = ProcedureLineWorkerManager.GetTypeOfInfoType(info.GetType());
            if (nodeType == null) throw new Exception($"cant find procedure line worker info of '{info.GetType()}'");
            var node = (T)Activator.CreateInstance(nodeType);
            ((IProcedureLineWorkerInitializer<ConditionCastOfWorkerInfo>)node).Init(info);
            return node;
        }
        
        public static T CreateWorker<T>(procedureline.WorkerInfo info) where T : IProcedureLineWorker {
            var nodeType = ProcedureLineWorkerManager.GetTypeOfInfoType(info.GetType());
            if (nodeType == null) throw new Exception($"cant find procedure line worker info of '{info.GetType()}'");
            var node = (T)Activator.CreateInstance(nodeType);
            ((IProcedureLineWorkerInitializer<procedureline.WorkerInfo>)node).Init(info);
            return node;
        }
    }
}