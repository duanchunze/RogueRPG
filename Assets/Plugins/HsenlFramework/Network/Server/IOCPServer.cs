using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hsenl.Network.Server {
    public class IOCPServer {
        /*
         * SocketAsyncEventArgs内部并没有开启多线程, SocketAsyncEventArgs之所以能实现单线程的并发操作, 因为他把请求交给了外部网络子系统去处理, 自己就呆在主线程里等消息.
         * 而Socket的其他方案, 如同步或者异步方案, 都是把请求放在自己的主线程里进行处理.
         */
        private const int opsToPreAlloc = 2; // 一部分用来收, 一部分用来发, 加起来2部分

        private Socket _listener;
        private SocketAsyncEventArgs _listenerEventArgs; // 服务器自己的event args, 用于接受连接
        private readonly int _maximumConnections; // 最大连接数
        private int _receiveBufferSize; // 每一个接收消息缓存区大小
        private readonly SocketAsyncEventArgsBufferPool _socketAsyncEventArgsBufferPool;
        private readonly SocketAsyncEventArgsPool _recvEventArgsPool;
        private readonly SocketAsyncEventArgsPool _sendEventArgsPool;
        private int _totalBytesRead; // 记录总共读取字节数
        private int _numConnectedSockets; // 当前客户端连接数

        private readonly Semaphore _maximumAcceptedClients;

        private List<SocketAsyncEventArgs> _connectedEventArgs = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maximumConnections">服务器最大连接数</param>
        /// <param name="receiveBufferSize">每个消息的缓存区大小</param>
        public IOCPServer(int maximumConnections, int receiveBufferSize) {
            this._totalBytesRead = 0;
            this._numConnectedSockets = 0;
            this._maximumConnections = maximumConnections;
            this._receiveBufferSize = receiveBufferSize;
            // 分配缓冲区，使最大数量的套接字可以同时有一个未完成的读和写发送到套接字
            this._socketAsyncEventArgsBufferPool =
                new SocketAsyncEventArgsBufferPool(receiveBufferSize * maximumConnections * opsToPreAlloc, receiveBufferSize);
            this._recvEventArgsPool = new SocketAsyncEventArgsPool(maximumConnections);
            this._maximumAcceptedClients = new Semaphore(maximumConnections, maximumConnections);
        }

        public void Init() {
            // 初始化总缓存区
            this._socketAsyncEventArgsBufferPool.InitBuffer();

            // 根据最大连接数, 准备好所有EventArg, 并且给每个EventArg都设置好缓存区
            for (int i = 0; i < this._maximumConnections; i++) {
                var recvEventArg = new SocketAsyncEventArgs();
                // 这里注册预先注册了消息完成时的回调
                recvEventArg.Completed += this.IOEventArgs_Completed;

                this._socketAsyncEventArgsBufferPool.SetBuffer(recvEventArg);
                this._recvEventArgsPool.Push(recvEventArg);

                var sendEventArg = new SocketAsyncEventArgs();
                sendEventArg.Completed += this.IOEventArgs_Completed;

                this._socketAsyncEventArgsBufferPool.SetBuffer(sendEventArg);
                this._sendEventArgsPool.Push(sendEventArg);
            }
        }

        // 启动服务器并监听
        public void StartListening(IPEndPoint localEndPoint) {
            this._listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(localEndPoint);
            // 连接需要三次握手的过程, 需要时间, 所以可能会产生积压, 当排队连接的客户端超出足最大积压数后, 后续再尝试连接的客户端, 在此时会被服务端直接拒绝
            this._listener.Listen(100);

            // 这个eventArg是用来监听客户端连接的
            this._listenerEventArgs = new SocketAsyncEventArgs();
            this._listenerEventArgs.Completed += this.AcceptEventArg_Completed;
            this.StartAccept(this._listenerEventArgs);

            Log.Info("IOCP_Tcp服务器启动成功...");
        }

        // 同\异步开启对客户端请求的监听
        // private void StartAccept(SocketAsyncEventArgs acceptEventArg) {
        //     bool willRaiseEvent = false;
        //     while (!willRaiseEvent) {
        //         // 当调用该函数时, 当前线程会尝试获得一个信号许可, 如果当前信号量的计数大于0, 则减少计数, 并允许当前线程继续执行. 而如果
        //         // 当前信号量的计数==0时, 当前线程将被阻塞, 等待其他线程释放信号量许可证(调用了Semaphore.Release())方法增加了信号量计数, 
        //         // 使得至少有一个许可证可用, 被阻塞的线程才会继续执行
        //         this._maximumAcceptedClients.WaitOne();
        //
        //         // Socket必须被清除，因为上下文对象正在被重用
        //         acceptEventArg.AcceptSocket = null;
        //         // 返回false, 代表该操作没有挂起, 是一个同步操作, 结果直接拿去用了, 当然了, acceptEventArg里的Complete也不会触发.
        //         // 如果返回true, 代表该操作被挂起了, 我们要把他当成个异步操作, 要等Complete的回调才能知道结果.
        //         willRaiseEvent = this._listener.AcceptAsync(acceptEventArg); // 虽然名字带个Async, 但并不像Task那样可以await, 是一种更通用的异步思路
        //         if (!willRaiseEvent) {
        //             // 如果上面返回的是同步操作, 我们处理完, 就继续while循环好了, 如果是异步的话, 就不由这里处理了, 且while循环也终止掉
        //             this.ProcessAccept(acceptEventArg);
        //         }
        //     }
        // }

        private void StartAccept(SocketAsyncEventArgs e) {
            e.AcceptSocket = null;
            var pending = this._listener.AcceptAsync(e);
            if (!pending) {
                this.ProcessAccept(e);
            }
        }

        // 尝试接受客户端连接的结果下来了(通过Complete)
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e) {
            // 处理这个连接请求
            this.ProcessAccept(e);

            // 重新开启 StartAccept
            this.StartAccept(e);
        }

        // 尝试接受客户端连接的结果下来了
        private void ProcessAccept(SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Accept)
                return;

            // 当listener被关闭时, 会触发listenerEventArgs的所有的Complete事件, 但传入的AcceptSocket却是一个无效的socket, 我们忽略他
            try {
                if (e.AcceptSocket.Available == 0)
                    return;
            }
            catch (Exception) {
                return;
            }

            // Interlocked.Increment可以给值上锁, 同时又比使用lock的方式要快, 特别在线程争用较少的情况下, 但只支持简单的数据, 更复杂的数据, 还是需要lock或其他方案
            Interlocked.Increment(ref this._numConnectedSockets);
            Log.Info("接受客户端的连接! 现在有{0}个客户端在连接服务器", this._numConnectedSockets);

            // 进来了一个新的客户端, 给他分配一个专门的eventArgs
            var readEventArgs = this._recvEventArgsPool.Pop();
            readEventArgs.UserToken = e.AcceptSocket;
            this._connectedEventArgs.Add(readEventArgs);

            // 一旦客户端连接上，就开始监听获取客户端的消息
            this.ReceiveAsync(readEventArgs);
        }

        // 同\异步接收客户端发来的消息
        private void ReceiveAsync(SocketAsyncEventArgs e) {
            var socket = (Socket)e.UserToken;
            bool willRaiseEvent = socket.ReceiveAsync(e);
            if (!willRaiseEvent) {
                this.ProcessReceive(e);
            }
        }

        // 同\异步给客户端发消息
        private void SendAsync(SocketAsyncEventArgs e) {
            var socket = (Socket)e.UserToken;
            bool willRaiseEvent = socket.SendAsync(e);
            if (!willRaiseEvent) {
                this.ProcessSend(e);
            }
        }

        // 尝试收发客户端消息的结果下来了(通过Complete)
        private void IOEventArgs_Completed(object sender, SocketAsyncEventArgs e) {
            // 判断是完成了什么, 是接受消息, 还是发送消息
            switch (e.LastOperation) {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        // 尝试接收客户端的消息的结果下来了
        private void ProcessReceive(SocketAsyncEventArgs e) {
            // 先检查对方客户端是否关闭了连接, 且接收消息是否成功
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) {
                // 增加数据接收记录
                Interlocked.Add(ref this._totalBytesRead, e.BytesTransferred);
                Log.Info("服务器总共接收了 {0} 字节", this._totalBytesRead);

                var str = e.Buffer.ToStr();
                Log.Info($"收到客户端的消息: {str}");
                this.ReceiveAsync(e);
            }
            else {
                this.CloseClientSocket(e);
            }
        }

        // 尝试给客户端发送消息的结果下来了
        private void ProcessSend(SocketAsyncEventArgs e) {
            // 如果发送成功, 则正常的继续接收客户端的消息
            if (e.SocketError == SocketError.Success) { }
            else {
                // 如果发送失败, 则关闭对方客户端
                this.CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e) {
            this._connectedEventArgs.Remove(e);
            var socket = (Socket)e.UserToken;

            // try {
            //     socket.Shutdown(SocketShutdown.Both);
            // }
            // catch (Exception) {
            //     // ignored
            // }

            socket.Close();

            Interlocked.Decrement(ref this._numConnectedSockets);

            this._recvEventArgsPool.Push(e);
            this._maximumAcceptedClients.Release();
            Log.Info("一个客户端从服务器断开! 现在有{0}个客户端在连接服务器", this._numConnectedSockets);
        }

        public void SendMessage(string message) {
            var bytes = message.ToUtf8();
            foreach (var connectedEventArg in this._connectedEventArgs) {
                connectedEventArg.SetBuffer(bytes, 0, bytes.Length);
                this.SendAsync(connectedEventArg);
            }
        }

        public void ReceiveMessage() {
            foreach (var connectedEventArg in this._connectedEventArgs) {
                this.ReceiveAsync(connectedEventArg);
            }
        }

        public void Close() {
            // 服务器的listener只需要close, 不需要shutdown, 因为服务器的listener压根没有I/O操作, 所以谈不上要shutdown, shutdown反而会报错
            this._listener.Close();
            this._listener = null;
            this._listenerEventArgs.Dispose();

            foreach (var connecterEventArg in this._connectedEventArgs) {
                var connectSocket = (Socket)connecterEventArg.UserToken;
                connectSocket.Close();
                connecterEventArg.Dispose();
            }

            this._connectedEventArgs.Clear();
        }
    }
}