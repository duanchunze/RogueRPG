using System;
using System.Collections.Generic;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    [MemoryPackable()]
    public partial class ProcedureLineNode : Unbodied {
        [ShowInInspector]
        [MemoryPackInclude]
        private List<IProcedureLineWorker> _workers = new();

        [MemoryPackIgnore]
        public IReadOnlyList<IProcedureLineWorker> Workers => this._workers;

        [MemoryPackIgnore]
        public Action<IProcedureLineWorker> onWorkerAdd;

        [MemoryPackIgnore]
        public Action<IProcedureLineWorker> onWorkerRemove;

        protected override void OnDeserializedOverall() {
            for (int i = 0, len = this._workers.Count; i < len; i++) {
                var worker = this._workers[i];
                worker.OnAddToNode(this);
                this.onWorkerAdd?.Invoke(worker);
            }
        }

        public void AddWorker(IProcedureLineWorker worker) {
            this._workers ??= new List<IProcedureLineWorker>();
            this._workers.Add(worker);
            worker.OnAddToNode(this);
            this.onWorkerAdd?.Invoke(worker);
        }

        public void RemoveWorker(IProcedureLineWorker worker) {
            this._workers.Remove(worker);
            worker.OnRemoveFromNode(this);
            if (this._workers.Count == 0) this._workers = null;
            this.onWorkerRemove?.Invoke(worker);
        }

        public T[] GetWorkers<T>() where T : IProcedureLineWorker {
            using var list = ListComponent<T>.Create();
            for (int i = 0, len = this._workers.Count; i < len; i++) {
                if (this._workers[i] is not T t)
                    continue;

                list.Add(t);
            }

            return list.ToArray();
        }

        public T GetWorker<T>() where T : IProcedureLineWorker {
            using var list = ListComponent<T>.Create();
            for (int i = 0, len = this._workers.Count; i < len; i++) {
                if (this._workers[i] is not T t)
                    continue;

                return t;
            }

            return default;
        }
    }
}