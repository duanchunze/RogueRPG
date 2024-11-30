using System;
using Hsenl.procedureline;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    public abstract class Plw : IProcedureLineWorker {
        // 携带该worker的组件, 可能是procedure line 或者 node
        [ShowInInspector]
        [MemoryPackIgnore]
        public Unbodied Component { get; private set; }

        void IProcedureLineWorker.OnAddToNode(ProcedureLineNode node) {
            try {
                this.Component = node;
                this.OnAddToNode(node);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IProcedureLineWorker.OnRemoveFromNode(ProcedureLineNode node) {
            try {
                this.Component = null;
                this.OnRemoveFromNode(node);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IProcedureLineWorker.OnAddToProcedureLine(ProcedureLine procedureLine) {
            try {
                this.Component = procedureLine;
                this.OnAddToProcedureLine(procedureLine);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IProcedureLineWorker.OnRemoveFromProcedureLine(ProcedureLine procedureLine) {
            try {
                this.Component = null;
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
}