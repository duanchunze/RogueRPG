namespace Hsenl {
    public interface IFsmState {
        void Init(IFsm fsm);
        void Enter(IFsm fsm, IFsmState prev);
        void Update(IFsm fsm, float deltaTime);
        void Leave(IFsm fsm, IFsmState next);
        void Destroy(IFsm fsm);
        void SetData(object o);
        T GetData<T>();
    }
}