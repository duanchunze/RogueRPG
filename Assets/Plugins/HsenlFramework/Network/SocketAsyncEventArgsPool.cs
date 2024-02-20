using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Hsenl.Network {
    internal class SocketAsyncEventArgsPool {
        private readonly Stack<SocketAsyncEventArgs> _pool;
        
        // 池中SocketAsyncEventArgs实例的数量
        public int Count => this._pool.Count;

        // 将对象池初始化为指定的大小
        // 
        // “capacity”参数为最大数目
        // 池中可以容纳的SocketAsyncEventArgs对象
        public SocketAsyncEventArgsPool(int capacity) {
            this._pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // 向池中添加一个SocketAsyncEventArg实例
        //
        // “item”参数是SocketAsyncEventArgs实例
        // 添加到池中
        public void Push(SocketAsyncEventArgs item) {
            if (item == null) {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock (this._pool) {
                this._pool.Push(item);
            }
        }

        // 从池中移除SocketAsyncEventArgs实例并返回从池中移除的对象
        public SocketAsyncEventArgs Pop() {
            lock (this._pool) {
                return this._pool.Pop();
            }
        }
    }
}