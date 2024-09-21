using System;
using System.Collections.Generic;

namespace Hsenl {
    public class ListComponent<T> : List<T>, IDisposable {
        public static ListComponent<T> Rent() {
            return (ListComponent<T>)ObjectPool.Rent(typeof(ListComponent<T>));
        }
        
        public static ListComponent<T> Rent(int capacity) {
            var list = (ListComponent<T>)ObjectPool.Rent(typeof(ListComponent<T>));
            list.Capacity = capacity;
            return list;
        }

        public void Dispose() {
            this.Clear();
            ObjectPool.Return(this);
        }
    }
}