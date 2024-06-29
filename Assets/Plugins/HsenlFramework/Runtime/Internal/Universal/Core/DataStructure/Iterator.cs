using System;
using System.Collections;
using System.Collections.Generic;

namespace Hsenl {
    /*
     * 可以在需要遍历内部数组的时候采用. 例如class1中有个private list<xxx> 字段, 如果我们想让外部可以遍历他, 大概有以下几种方法
     * 返回他的IReadOnlyList、返回他的IEnumerable<xxx>、通过Foreach(Action<xxx> callback)回调的形式实现遍历. 但除了第一种方法外, 其他方法都可能产生gc.
     * 现在, 还可以使用这种方式, 快速的实现foreach的功能, 保持私密的同时, 且安全, 0gc.
     */
    public readonly struct Iterator<T> : IEnumerable<T> {
        private enum Type : byte {
            None,
            One,
            List,
        }

        private readonly Type _type;
        private readonly T _t;
        private readonly List<T>.Enumerator _list;

        public Iterator(T t) {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            this._type = Type.One;
            this._t = t;
            this._list = default;
        }

        public Iterator(List<T>.Enumerator list) {
            this._type = Type.List;
            this._list = list;
            this._t = default;
        }

        public Enumerator GetEnumerator() {
            return this._type switch {
                Type.None => default,
                Type.One => new Enumerator(this._t),
                Type.List => new Enumerator(this._list),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public struct Enumerator : IEnumerator<T> {
            private Type _type;
            private T _t;
            private bool _b;
            private List<T>.Enumerator _list;

            public T Current {
                get {
                    return this._type switch {
                        Type.None => default,
                        Type.One => this._t,
                        Type.List => this._list.Current,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            public Enumerator(T t) {
                this._type = Type.One;
                this._t = t;
                this._b = false;
                this._list = default;
            }

            public Enumerator(List<T>.Enumerator list) {
                this._type = Type.List;
                this._list = list;
                this._t = default;
                this._b = false;
            }

            public bool MoveNext() {
                switch (this._type) {
                    case Type.None:
                        return false;
                    case Type.One: {
                        if (!this._b) {
                            this._b = true;
                            return true;
                        }

                        return true;
                    }
                    case Type.List:
                        return this._list.MoveNext();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public void Reset() { }

            object IEnumerator.Current => this.Current;

            public void Dispose() {
                this.Reset();
            }
        }
    }
}