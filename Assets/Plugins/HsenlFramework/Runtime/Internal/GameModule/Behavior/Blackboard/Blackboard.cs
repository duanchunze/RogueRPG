using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class Blackboard : IBlackboard {
        [MemoryPackIgnore]
        private Dictionary<string, object> _data = new();

        public T GetData<T>(string name) {
            if (this._data.TryGetValue(name, out var o)) {
                if (o is T t) {
                    return t;
                }

                return default;
            }

            return default;
        }

        public T GetOrCreateData<T>(string name) where T : new() {
            if (!this._data.TryGetValue(name, out var o)) {
                var newT = new T();
                o = newT;
                this._data[name] = o;
                return newT;
            }

            if (o is T t)
                return t;

            return default;
        }

        public bool TryGetData<T>(string name, out T result) {
            if (this._data.TryGetValue(name, out var o)) {
                result = (T)o;
                return true;
            }

            result = default;
            return false;
        }

        public void SetData<T>(string name, T t) {
            this._data[name] = t;
        }

        public bool Contains(string name) {
            return this._data.ContainsKey(name);
        }

        public void Clear() {
            this._data.Clear();
        }
    }
}