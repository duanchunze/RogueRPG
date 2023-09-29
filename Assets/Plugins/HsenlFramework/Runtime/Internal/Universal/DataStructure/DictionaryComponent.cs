using System;
using System.Collections.Generic;

namespace Hsenl {
    public class DictionaryComponent<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable {
        public static DictionaryComponent<TKey, TValue> Create() {
            return ObjectPool.Fetch(typeof(DictionaryComponent<TKey, TValue>)) as DictionaryComponent<TKey, TValue>;
        }

        public void Dispose() {
            this.Clear();
            ObjectPool.Recycle(this);
        }
    }
}