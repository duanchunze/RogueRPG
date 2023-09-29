namespace Hsenl {
    public static class Procedure {
        public static bool ChangeState<T>() where T : AProcedureState => ProcedureManager.Instance.ChangeState<T>();
        public static bool ChangeState<T, TData>(TData data) where T : AProcedureState => ProcedureManager.Instance.ChangeState<T, TData>(data);
        public static T GetState<T>() where T : AProcedureState => ProcedureManager.Instance.GetState<T>();
    }
}