namespace Hsenl {
    public interface IFsmState {
        bool IsEntering { get; internal set; }
        bool IsLeaving { get; internal set; }
        void Init(IFsm fsm);
        HTask Enter(IFsm fsm, IFsmState prev);
        void Update(IFsm fsm, float deltaTime);
        HTask Leave(IFsm fsm, IFsmState next);
        void Destroy(IFsm fsm);
        void SetData(object o);
        T GetData<T>();
    }
}