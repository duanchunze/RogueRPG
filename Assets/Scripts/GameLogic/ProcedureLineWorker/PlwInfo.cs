using System;
using Hsenl.cast;
using MemoryPack;

namespace Hsenl {
    public abstract class PlwInfo : IProcedureLineWorker {
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
    public abstract class PlwInfo<T> : PlwInfo, IProcedureLineWorkerInitializer<procedureline.WorkerInfo> where T : procedureline.WorkerInfo {
        [MemoryPackIgnore]
        public T info;

        public int infoInstanceId;

        protected override void OnAddToNode(ProcedureLineNode node) {
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = procedureline.WorkerInfo.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.Init(inf);
                }
            }
        }

        public void Init(procedureline.WorkerInfo inf) {
            var t = (T)inf;
            this.info = t;
            this.infoInstanceId = t.InstanceId;

            this.OnInit(t);
        }

        protected virtual void OnInit(T arg) { }
    }
}