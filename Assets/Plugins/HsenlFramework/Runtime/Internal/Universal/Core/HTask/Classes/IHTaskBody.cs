using System;
using System.Runtime.CompilerServices;

namespace Hsenl {
    // 谨慎实现自己的Task, 写不对的话, 运行不了, 或带着未知的风险运行.
    public interface IHTaskBody {
        uint Version { get; }
        HTaskStatus Status { get; }
        bool IsCompleted { get; }
        public Action MoveNext { get; }
        public IAsyncStateMachine StateMachine { get; set; }
        void GetResult();
        void SetResult();
        void SetException(Exception e);
        void Abort();
        void OnCompleted(Action continuation);
        void UnsafeOnCompleted(Action continuation);
        void Dispose();
    }
    
    public interface IHTaskBody<T> {
        uint Version { get; }
        HTaskStatus Status { get; }
        bool IsCompleted { get; }
        public Action MoveNext { get; }
        public IAsyncStateMachine StateMachine { get; set; }
        T GetResult();
        void SetResult(T value);
        void SetException(Exception e);
        void Abort();
        void OnCompleted(Action continuation);
        void UnsafeOnCompleted(Action continuation);
        void Dispose();
    }
}