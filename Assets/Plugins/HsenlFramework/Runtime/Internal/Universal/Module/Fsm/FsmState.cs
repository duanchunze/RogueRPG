namespace Hsenl {
    public abstract class FsmState { }

    public abstract class FsmState<T> : FsmState where T : IFsm {
        protected abstract void OnInit(T manager);
        protected abstract void OnEnter(T manager, FsmState<T> prev);
        protected abstract void OnUpdate(T manager, float deltaTime);
        protected abstract void OnLeave(T manager, FsmState<T> next);
        protected abstract void OnDestroy(T manager);
    }
}