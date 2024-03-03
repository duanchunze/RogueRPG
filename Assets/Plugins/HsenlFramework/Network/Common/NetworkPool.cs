using System;
using System.Collections.Generic;

namespace Hsenl.Network {
    public class NetworkPool<T> where T : class {
        private readonly Stack<T> _pool = new();

        public T Rent() {
            lock (this._pool) {
                if (!this._pool.TryPop(out var result)) {
                    result = (T)Activator.CreateInstance(typeof(T));
                }

                return result;
            }
        }
        
        public bool TryRent(out T result) {
            lock (this._pool) {
                if (!this._pool.TryPop(out result)) {
                    return false;
                }

                return true;
            }
        }

        public void Return(T value) {
            if (value == null)
                throw new ArgumentNullException($"NetworkPool return '{nameof(value)}' is null!");

            lock (this._pool) {
                this._pool.Push(value);
            }
        }

        public void Clear() {
            lock (this._pool) {
                this._pool.Clear();
            }
        }
    }
}