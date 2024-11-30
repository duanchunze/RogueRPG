using System;

namespace Hsenl {
    [ProcedureState]
    public abstract class ProcedureState : IFsmState {
        private object _data;
        private HTask _task;
        private bool _isEntering;
        private bool _isLeaving;

        bool IFsmState.IsEntering {
            get => this._isEntering;
            set => this._isEntering = value;
        }

        bool IFsmState.IsLeaving {
            get => this._isLeaving;
            set => this._isLeaving = value;
        }

        public bool IsEntering => this._isEntering;

        public bool IsLeaving => this._isLeaving;

        void IFsmState.Init(IFsm fsm) {
            try {
                this.OnInit(fsm);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        async HTask IFsmState.Enter(IFsm fsm, IFsmState prev) {
            try {
                await this.OnEnter(fsm, prev);
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

        async HTask IFsmState.Leave(IFsm fsm, IFsmState next) {
            try {
                await this.OnLeave(fsm, next);
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
        
        /*
         * 事件执行顺序
         * -> 清空上个State
         * -> 上个State开始OnLeave
         * -> 上个State完成OnLeave
         * -> 下个State开始OnEnter
         * -> 下个State完成OnEnter
         * -> 当前State赋值为下个State
         * -> 下个State OnUpdate
         */

        public virtual Type Group => null;

        protected virtual void OnInit(IFsm fsm) { }

        protected abstract HTask OnEnter(IFsm fsm, IFsmState prev);

        protected virtual void OnUpdate(IFsm fsm, float deltaTime) { }

        protected abstract HTask OnLeave(IFsm fsm, IFsmState next);

        protected virtual void OnDestroy(IFsm fsm) { }
    }
}