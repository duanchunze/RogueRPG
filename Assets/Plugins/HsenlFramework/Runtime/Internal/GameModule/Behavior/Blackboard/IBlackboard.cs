using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface IBlackboard<in TKey, in TValue> {
        T GetData<T>(TKey key) where T : TValue;
        T GetOrCreateData<T>(string name) where T : new();
        bool TryGetData<T>(TKey key, out T result) where T : TValue;
        void SetData<T>(TKey key, T data) where T : TValue;
        bool Contains(TKey key);
        void Clear();
    }
    
    public partial interface IBlackboard : IBlackboard<string, object> { }
}