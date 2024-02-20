using System.Collections.Generic;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 这个类创建一个单独的大缓冲区，它可以被分割并分配给SocketAsyncEventArgs对象，用于每个套接字I/O操作。
    // 这样可以很容易地重用缓冲区，并防止堆内存碎片化。
    // BufferManager类上公开的操作不是线程安全的。
    internal class SocketAsyncEventArgsBufferPool {
        private readonly int _numBytes; // 缓冲池控制的总字节数
        private byte[] _buffer; // 由缓冲区管理器维护的字节数组
        private readonly Stack<int> _freeIndexPool; //
        private int _currentIndex;
        private readonly int _bufferSize;

        public SocketAsyncEventArgsBufferPool(int totalBytes, int bufferSize) {
            this._numBytes = totalBytes;
            this._currentIndex = 0;
            this._bufferSize = bufferSize;
            this._freeIndexPool = new Stack<int>();
        }

        // 分配缓冲池使用的缓冲空间
        public void InitBuffer() {
            // 创建一个大的缓冲区，并将其划分给每个SocketAsyncEventArg对象
            this._buffer = new byte[this._numBytes];
        }

        // 从缓冲池中分配一个缓冲区给指定的SocketAsyncEventArgs对象
        // <returns>如果缓冲区设置成功，则为True，否则为false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args) {
            if (this._freeIndexPool.Count > 0) {
                args.SetBuffer(this._buffer, this._freeIndexPool.Pop(), this._bufferSize);
            }
            else {
                if ((this._numBytes - this._bufferSize) < this._currentIndex) {
                    return false;
                }

                args.SetBuffer(this._buffer, this._currentIndex, this._bufferSize);
                this._currentIndex += this._bufferSize;
            }

            return true;
        }

        // 从SocketAsyncEventArg对象中移除缓冲区, 这将把缓冲区释放回缓冲池
        public void FreeBuffer(SocketAsyncEventArgs args) {
            this._freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}