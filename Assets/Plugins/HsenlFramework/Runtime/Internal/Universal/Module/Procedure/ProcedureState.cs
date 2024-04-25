using System;

namespace Hsenl {
    [ProcedureState]
    public abstract class ProcedureState : IFsmState {
        private object _data;
        private HTask _task;

        void IFsmState.Init(IFsm fsm) {
            try {
                this.OnInit(fsm);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IFsmState.Enter(IFsm fsm, IFsmState prev) {
            try {
                this.OnEnter(fsm, prev);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IFsmState.Update(IFsm fsm, float deltaTime) {
            try {
                this.OnUpdate(fsm, deltaTime);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IFsmState.Leave(IFsm fsm, IFsmState next) {
            try {
                this.OnLeave(fsm, next);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        void IFsmState.Destroy(IFsm fsm) {
            try {
                this.OnDestroy(fsm);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void SetData(object o) {
            this._data = o;
        }

        public T GetData<T>() {
            if (this._data is not T t)
                throw new InvalidCastException($"'{this._data}' can not cast to {typeof(T)}");

            return t;
        }

        public virtual Type Group => null;

        protected virtual void OnInit(IFsm fsm) { }

        protected abstract void OnEnter(IFsm fsm, IFsmState prev);

        protected virtual void OnUpdate(IFsm fsm, float deltaTime) { }

        protected abstract void OnLeave(IFsm fsm, IFsmState next);

        protected virtual void OnDestroy(IFsm fsm) { }

        protected virtual void Wait() {
            this._task = HTask.Create();
        }

        protected virtual void Done() {
            this._task.SetResult();
        }

        public async HTask AsyncDone() {
            await this._task;
        }
    }
}