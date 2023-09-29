using System;
using System.Collections.Generic;

namespace Hsenl {
    public class HashSetComponent<T> : HashSet<T>, IDisposable {
        public static HashSetComponent<T> Create() {
            return ObjectPool.Fetch(typeof(HashSetComponent<T>)) as HashSetComponent<T>;
        }

        public void Dispose() {
            this.Clear();
            ObjectPool.Recycle(this);
        }
    }
}