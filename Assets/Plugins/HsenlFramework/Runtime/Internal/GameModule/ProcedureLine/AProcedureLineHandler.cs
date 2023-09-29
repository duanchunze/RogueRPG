using System;

namespace Hsenl {
    public interface IProcedureLineHandler {
        Type ItemType { get; }
        Type WorkerType { get; }
    }

    public interface IProcedureLineHandlerOfWorker<T> : IProcedureLineHandler {
        ProcedureLineHandleResult Run(ProcedureLine procedureLine, ref T item, IProcedureLineWorker wk);
    }

    public interface IProcedureLineHandlerNoWorker<T> : IProcedureLineHandler {
        ProcedureLineHandleResult Run(ProcedureLine procedureLine, ref T item);
    }

    public interface IProcedureLineHandlerOfWorkerAsync<T> : IProcedureLineHandler {
        ETTask<ProcedureLineHandleResult> Run(ProcedureLine procedureLine, T item, IProcedureLineWorker wk);
    }

    public interface IProcedureLineHandlerNoWorkerAsync<T> : IProcedureLineHandler {
        ETTask<ProcedureLineHandleResult> Run(ProcedureLine procedureLine, T item);
    }


    // 有工人的handler，就像是给Event系统加了热插拔的功能
    [ProcedureLineHandler]
    public abstract class AProcedureLineHandler<T1, T2> : IProcedureLineHandlerOfWorker<T1> where T2 : IProcedureLineWorker {
        public Type ItemType => typeof(T1);
        public Type WorkerType => typeof(T2);

        public ProcedureLineHandleResult Run(ProcedureLine procedureLine, ref T1 item, IProcedureLineWorker wk) {
            var worker = (T2)wk;
            try {
                return this.Handle(procedureLine, ref item, worker);
            }
            catch (Exception e) {
                Log.Error(e);
                return ProcedureLineHandleResult.Assert;
            }
        }

        protected abstract ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref T1 item, T2 worker);
    }

    // 没有工人的处理者，就类似于一个Event系统
    [ProcedureLineHandler]
    public abstract class AProcedureLineHandler<T1> : IProcedureLineHandlerNoWorker<T1> {
        public Type ItemType => typeof(T1);
        public Type WorkerType => null;

        public ProcedureLineHandleResult Run(ProcedureLine procedureLine, ref T1 item) {
            try {
                return this.Handle(procedureLine, ref item);
            }
            catch (Exception e) {
                Log.Error(e);
                return ProcedureLineHandleResult.Assert;
            }
        }

        protected abstract ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref T1 item);
    }

    // 有工人的handler，就像是给Event系统加了热插拔的功能
    [ProcedureLineHandler]
    public abstract class AProcedureLineHandlerAsync<T1, T2> : IProcedureLineHandlerOfWorkerAsync<T1> where T2 : IProcedureLineWorker {
        public Type ItemType => typeof(T1);
        public Type WorkerType => typeof(T2);

        public async ETTask<ProcedureLineHandleResult> Run(ProcedureLine procedureLine, T1 item, IProcedureLineWorker wk) {
            var worker = (T2)wk;
            try {
                return await this.Handle(procedureLine, item, worker);
            }
            catch (Exception e) {
                Log.Error(e);
                return ProcedureLineHandleResult.Assert;
            }
        }

        protected abstract ETTask<ProcedureLineHandleResult> Handle(ProcedureLine procedureLine, T1 item, T2 worker);
    }

    // 没有工人的处理者，就类似于一个Event系统
    [ProcedureLineHandler]
    public abstract class AProcedureLineHandlerAsync<T1> : IProcedureLineHandlerNoWorkerAsync<T1> {
        public Type ItemType => typeof(T1);
        public Type WorkerType => null;

        public async ETTask<ProcedureLineHandleResult> Run(ProcedureLine procedureLine, T1 item) {
            try {
                return await this.Handle(procedureLine, item);
            }
            catch (Exception e) {
                Log.Error(e);
                return ProcedureLineHandleResult.Assert;
            }
        }

        protected abstract ETTask<ProcedureLineHandleResult> Handle(ProcedureLine procedureLine, T1 item);
    }
}