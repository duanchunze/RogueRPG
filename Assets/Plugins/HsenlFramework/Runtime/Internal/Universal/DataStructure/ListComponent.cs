using System;
using System.Collections.Generic;

namespace Hsenl {
    public class ListComponent<T> : List<T>, IDisposable {
        public static ListComponent<T> Create() {
            return (ListComponent<T>)ObjectPool.Fetch(typeof(ListComponent<T>));
        }
        
        public static ListComponent<T> Create(int capacity) {
            var list = (ListComponent<T>)ObjectPool.Fetch(typeof(ListComponent<T>));
            list.Capacity = capacity;
            return list;
        }

        public void Dispose() {
            this.Clear();
            ObjectPool.Recycle(this);
        }
    }
}