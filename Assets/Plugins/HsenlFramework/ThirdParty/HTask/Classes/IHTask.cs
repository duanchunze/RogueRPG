using System;

namespace Hsenl {
    // 谨慎实现自己的Task, 写不对的话, 运行不了, 或带着未知的风险运行.
    public interface IHTask {
        HTaskStatus Status { get; }
        bool IsCompleted { get; }
        void GetResult();
        void SetResult();
        void SetException(Exception e);
        void Abort();
        void OnCompleted(Action continuation);
        void UnsafeOnCompleted(Action continuation);
        void Dispose();
    }
    
    public interface IHTask<T> {
        HTaskStatus Status { get; }
        bool IsCompleted { get; }
        T GetResult();
        void SetResult(T value);
        void SetException(Exception e);
        void Abort();
        void OnCompleted(Action continuation);
        void UnsafeOnCompleted(Action continuation);
        void Dispose();
    }
}