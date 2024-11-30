namespace Hsenl {
    public interface IFsm {
        T GetState<T>() where T : IFsmState;
        HTask<bool> ChangeState<T>()  where T : IFsmState;
    }
}