namespace Hsenl {
    public interface IFsm {
        T GetState<T>() where T : IFsmState;
        bool ChangeState<T>()  where T : IFsmState;
    }
}