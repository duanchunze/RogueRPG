using System;

namespace Hsenl {
    public interface IInvoke {
        Type Type { get; }
    }

    [Invoke]
    public abstract class AInvokeHandler<TArg> : IInvoke where TArg : struct {
        public Type Type => typeof(TArg);

        public abstract void Handle(TArg arg);
    }

    [Invoke]
    public abstract class AInvokeHandler<TArg, TReturn> : IInvoke where TArg : struct {
        public Type Type => typeof(TArg);

        public abstract TReturn Handle(TArg arg);
    }
}