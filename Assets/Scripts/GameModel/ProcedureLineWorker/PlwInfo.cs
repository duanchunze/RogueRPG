using System;
using Hsenl.procedureline;
using MemoryPack;

namespace Hsenl {
    [ProcedureLineWorker]
    public abstract class PlwInfo<T> : Plw, IConfigInfoInitializer<procedureline.WorkerInfo> where T : procedureline.WorkerInfo {
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