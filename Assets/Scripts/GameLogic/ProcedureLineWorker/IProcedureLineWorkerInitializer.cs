using Hsenl.cast;

namespace Hsenl {
    public interface IProcedureLineWorkerInitializer<in T> {
        void Init(T inf);
    }
}