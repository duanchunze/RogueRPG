using System;
using System.Collections.Generic;

namespace Hsenl {
    // 双向字典
    public class BidirectionalDictionary<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> _kv = new();
        private readonly Dictionary<TValue, TKey> _vk = new();

        public BidirectionalDictionary() { }

        public BidirectionalDictionary(int capacity) {
            this._kv = new Dictionary<TKey, TValue>(capacity);
            this._vk = new Dictionary<TValue, TKey>(capacity);
        }

        public void ForEach(Action<TKey, TValue> action) {
            if (action == null) {
                return;
            }

            var keys = this._kv.Keys;
            foreach (var key in keys) {
                action(key, this._kv[key]);
            }
        }

        public int Count => this._kv.Count;

        public List<TKey> Keys => new(this._kv.Keys);

        public List<TValue> Values => new(this._vk.Keys);
        
        public TValue this[TKey key] => this._kv[key];

        public TKey this[TValue value] => this._vk[value];

        public void Add(TKey key, TValue value) {
            if (key == null || value == null || this._kv.ContainsKey(key) || this._vk.ContainsKey(value)) {
                return;
            }

            this._kv.Add(key, value);
            this._vk.Add(value, key);
        }

        public bool TryGetValueByKey(TKey key, out TValue value) {
            return this._kv.TryGetValue(key, out value);
        }

        public bool TryGetKeyByValue(TValue value, out TKey key) {
            return this._vk.TryGetValue(value, out key);
        }

        public void RemoveByKey(TKey key) {
            if (key == null) {
                return;
            }

            if (!this._kv.TryGetValue(key, out var value)) {
                return;
            }

            this._kv.Remove(key);
            this._vk.Remove(value);
        }

        public void RemoveByValue(TValue value) {
            if (value == null) {
                return;
            }

            if (!this._vk.TryGetValue(value, out var key)) {
                return;
            }

            this._kv.Remove(key);
            this._vk.Remove(value);
        }

        public void Clear() {
            this._kv.Clear();
            this._vk.Clear();
        }

        public bool ContainsKey(TKey key) {
            if (key == null) {
                return false;
            }

            return this._kv.ContainsKey(key);
        }

        public bool ContainsValue(TValue value) {
            if (value == null) {
                return false;
            }

            return this._vk.ContainsKey(value);
        }

        public bool Contains(TKey key, TValue value) {
            if (key == null || value == null) {
                return false;
            }

            return this._kv.ContainsKey(key) && this._vk.ContainsKey(value);
        }
    }
}