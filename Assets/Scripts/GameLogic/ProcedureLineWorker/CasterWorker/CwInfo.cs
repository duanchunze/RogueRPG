using System;
using Hsenl.cast;
using MemoryPack;

namespace Hsenl {
    [ProcedureLineWorker]
    public abstract class CwInfo<T> : PlwInfo, IProcedureLineWorkerInitializer<ConditionCastOfWorkerInfo> where T : ConditionCastOfWorkerInfo {
        [MemoryPackIgnore]
        public T info;

        public int infoInstanceId;

        [MemoryPackIgnore]
        public Caster caster;

        protected override void OnAddToNode(ProcedureLineNode node) {
            this.caster = node.GetComponent<Caster>();
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = ConditionCastOfWorkerInfo.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.Init(inf);
                }
            }
        }

        protected override void OnRemoveFromNode(ProcedureLineNode node) {
            this.caster = null;
        }

        public void Init(ConditionCastOfWorkerInfo inf) {
            var t = (T)inf;
            this.info = t;
            this.infoInstanceId = t.InstanceId;

            this.OnInit(t);
        }

        protected virtual void OnInit(T arg) { }
    }
}