using System;
using System.Collections.Generic;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [MemoryPackable()]
    public partial class ProcedureLineNode : Unbodied {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        [MemoryPackInclude]
        private List<IProcedureLineWorker> _workers = new();

        [MemoryPackIgnore]
        public IReadOnlyList<IProcedureLineWorker> Workers => this._workers;

        [MemoryPackIgnore]
        private readonly List<ProcedureLine> _linkProcedureLines = new();

        protected override void OnDeserializedOverall() {
            for (int i = 0, len = this._workers.Count; i < len; i++) {
                var worker = this._workers[i];
                worker.OnAddToNode(this);
            }
        }

        internal override void OnDisposedInternal() {
            base.OnDisposedInternal();
            for (int i = this._linkProcedureLines.Count - 1; i >= 0; i--) {
                var pl = this._linkProcedureLines[i];
                pl.Detach(this);
            }

            this._linkProcedureLines.Clear();
            this._workers.Clear();
        }

        internal bool LinkProcedureLine(ProcedureLine procedureLine) {
            if (this._linkProcedureLines.Contains(procedureLine)) return false;
            this._linkProcedureLines.Add(procedureLine);
            return true;
        }

        internal bool UnlinkProcedureLine(ProcedureLine procedureLine) {
            return this._linkProcedureLines.Remove(procedureLine);
        }

        public void AddWorker(IProcedureLineWorker worker) {
            this._workers ??= new List<IProcedureLineWorker>();
            this._workers.Add(worker);
            for (int i = 0; i < this._linkProcedureLines.Count; i++) {
                var procedureline = this._linkProcedureLines[i];
                procedureline.AddWorker_Internal(worker);
            }

            worker.OnAddToNode(this);
        }

        public void RemoveWorker(IProcedureLineWorker worker) {
            var succ = this._workers.Remove(worker);
            if (!succ) return;
            if (this._workers.Count == 0) this._workers = null;
            for (int i = 0; i < this._linkProcedureLines.Count; i++) {
                var procedureline = this._linkProcedureLines[i];
                procedureline.RemoveWorker_Internal(worker);
            }

            worker.OnRemoveFromNode(this);
        }

        public T[] GetWorkers<T>() where T : IProcedureLineWorker {
            using var list = ListComponent<T>.Rent();
            for (int i = 0, len = this._workers.Count; i < len; i++) {
                if (this._workers[i] is not T t)
                    continue;

                list.Add(t);
            }

            return list.ToArray();
        }

        public T GetWorker<T>() where T : IProcedureLineWorker {
            using var list = ListComponent<T>.Rent();
            for (int i = 0, len = this._workers.Count; i < len; i++) {
                if (this._workers[i] is not T t)
                    continue;

                return t;
            }

            return default;
        }
    }
}