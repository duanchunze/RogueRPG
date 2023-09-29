namespace Hsenl {
    public interface IFsm { }

    public interface IFsm<in T> : IFsm where T : FsmState {
        bool ChangeState(T fsmState);
    }
}