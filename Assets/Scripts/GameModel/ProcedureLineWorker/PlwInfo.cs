using System;
using Hsenl.procedureline;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    public abstract class PlwInfo : IProcedureLineWorker {
        [ShowInInspector]
        [MemoryPackIgnore]
        public ProcedureLineNode ProcedureLineNode { get; private set; }

        void IProcedureLineWorker.OnAddToNode(ProcedureLineNode node) {
            try {
                this.ProcedureLineNode = node;
                this.OnAddToNode(node);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IProcedureLineWorker.OnRemoveFromNode(ProcedureLineNode node) {
            try {
                this.ProcedureLineNode = null;
                this.OnRemoveFromNode(node);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IProcedureLineWorker.OnAddToProcedureLine(ProcedureLine procedureLine) {
            try {
                this.OnAddToProcedureLine(procedureLine);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IProcedureLineWorker.OnRemoveFromProcedureLine(ProcedureLine procedureLine) {
            try {
                this.OnRemoveFromProcedureLine(procedureLine);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnAddToNode(ProcedureLineNode node) { }

        protected virtual void OnRemoveFromNode(ProcedureLineNode node) { }

        protected virtual void OnAddToProcedureLine(ProcedureLine procedureLine) { }

        protected virtual void OnRemoveFromProcedureLine(ProcedureLine procedureLine) { }
    }

    [ProcedureLineWorker]
    public abstract class PlwInfo<T> : PlwInfo, IConfigInfoInitializer<procedureline.WorkerInfo> where T : procedureline.WorkerInfo {
        [MemoryPackIgnore]
        public T info;

        public int infoInstanceId;
        
        public Type InfoType => typeof(T);

        protected override void OnAddToNode(ProcedureLineNode node) {
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = procedureline.WorkerInfo.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.InitInfo(inf);
                }
            }
        }
        
        public void InitInfo(object configInfo) {
            if (configInfo is behavior.Info i) {
                this.InitInfo(i);
            }
        }

        public void InitInfo(WorkerInfo configInfo) {
            var t = (T)configInfo;
            this.info = t;
            this.infoInstanceId = t.InstanceId;

            this.OnConfigInfoInit(t);
        }

        protected virtual void OnConfigInfoInit(T arg) { }
    }
}