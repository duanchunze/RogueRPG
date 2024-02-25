using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Hsenl.Network {
    public class IOCPServer {
        /*
         * SocketAsyncEventArgs内部并没有开启多线程, SocketAsyncEventArgs之所以能实现单线程的并发操作, 因为他把请求交给了外部网络子系统去处理, 自己就呆在主线程里等消息.
         * 而Socket的其他方案, 如同步或者异步方案, 都是把请求放在自己的主线程里进行处理.
         *
         * SocketAsyncEventArgs接收数据的时候, 发生了以下几次复制
         * 1、当数据通过网络到达时, 操作系统的TCP/IP栈会将数据从网络接口卡(NIC)接收并缓存到内核缓冲区
         * 2、当ReceiveAsync方法被调用时, 操作系统会将数据从内核缓冲区复制到用户空间的缓冲区, 这个缓冲区就是EventArgs的Buffer或BufferList
         * 3、最后就是我们使用这些数据时, 可能需要把数据从Buffer中拷贝出来
         */
        private Socket _listener;
        private SocketAsyncEventArgs _listenerEventArgs; // 服务器自己的event args, 用于接受连接
        private int _receiveBufferSize; // 每一个接收消息缓存区大小
        private int _maximumMessageBodySize; // 允许的包体的最大大小
        private int _maxinumSendSizeOnce; // 每一个接收消息缓存区大小
        private int _totalBytesRead; // 记录总共读取字节数
        private int _numConnectedSockets; // 当前客户端连接数
        private readonly NetworkPool<IOCPChannel> channelPool = new();
        private readonly Dictionary<long, IOCPChannel> _channels = new();

        public event Action<IOCPChannel, Memory<byte>> OnRecvData;

        /// <param name="receiveBufferSize">每个接收消息的缓存区大小</param>
        /// <param name="maxinumSendSizeOnce">每次发送的最大数据</param>
        /// <param name="maximumMessageBodySize">允许用户发送的最大的包体大小</param>
        public IOCPServer(int receiveBufferSize, int maxinumSendSizeOnce, int maximumMessageBodySize = 0) {
            this._receiveBufferSize = receiveBufferSize;
            this._maximumMessageBodySize = maximumMessageBodySize == 0 ? receiveBufferSize * 1024 : maximumMessageBodySize;
            this._maxinumSendSizeOnce = maxinumSendSizeOnce;
            this._totalBytesRead = 0;
            this._numConnectedSockets = 0;
        }

        // 启动服务器并监听
        public void StartListening(IPEndPoint localEndPoint, int backlog = 1000) {
            if (this._listener != null)
                throw new Exception("TcpServer is already started!");

            this._listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(localEndPoint);
            // 连接需要三次握手的过程, 需要时间, 所以可能会产生积压, 当排队连接的客户端超出足最大积压数后, 后续再尝试连接的客户端, 在此时会被服务端直接拒绝
            this._listener.Listen(backlog);

            // 这个eventArg是用来监听客户端连接的
            this._listenerEventArgs = new SocketAsyncEventArgs();
            this._listenerEventArgs.Completed += this.AcceptEventArg_Completed;
            this.StartAccept(this._listenerEventArgs);

            Log.Info($"IOCP_Tcp服务器启动成功...{localEndPoint}");
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
            try {
                // 处理这个连接请求
                this.ProcessAccept(e);

                // 重新开启 StartAccept
                this.StartAccept(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        // 尝试接受客户端连接的结果下来了
        private void ProcessAccept(SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Accept)
                return;

            // Interlocked.Increment比使用lock的方式要快, 特别在线程争用较少的情况下
            Interlocked.Increment(ref this._numConnectedSockets);
            Log.Info("接受客户端的连接! 现在有{0}个客户端在连接服务器", this._numConnectedSockets);

            // 进来了一个新的客户端, 给他分配一个专门的通道
            var channel = this.channelPool.Rent();
            this._channels.Add(channel.GetHashCode(), channel);
            channel.OnRecvDataIncludeIncomplete += this.OnChannelRecvDataIncludeIncomplete;
            channel.OnRecvData += this.OnChannelRecvData;
            channel.OnError += this.CloseClientSocket;

            channel.Init(this._receiveBufferSize, this._maxinumSendSizeOnce, this._maximumMessageBodySize);
            // 启动通道, 开始接收数据
            channel.Start(e.AcceptSocket);
        }

        private void OnChannelRecvDataIncludeIncomplete(IOCPChannel channel, Memory<byte> data) {
            Interlocked.Add(ref this._totalBytesRead, data.Length);
            Log.Info($"服务器接收到了数据:  {data.Length} b / {this._totalBytesRead} b -- {(data.Length / 1024f):f2} kb / {(this._totalBytesRead / 1024f):f2} kb");
        }

        private void OnChannelRecvData(IOCPChannel channel, Memory<byte> data) {
            Log.Info($"服务器接收到了一个完整的包:  {data.Length} b / {this._totalBytesRead} b -- {(data.Length / 1024f):f2} kb / {(this._totalBytesRead / 1024f):f2} kb");
            try {
                this.OnRecvData?.Invoke(channel, data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private void CloseClientSocket(IOCPChannel channel, SocketAsyncEventArgs e) {
            try {
                channel.Close();
                this.channelPool.Return(channel);
                Interlocked.Decrement(ref this._numConnectedSockets);

                Log.Info("一个客户端从服务器断开! 现在有{0}个客户端在连接服务器", this._numConnectedSockets);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        public void Close() {
            if (this._listener == null)
                return;

            // 服务器的listener只需要close, 不需要shutdown, 因为服务器的listener压根没有I/O操作, 所以谈不上要shutdown, shutdown反而会报错
            this._listenerEventArgs.Completed -= this.AcceptEventArg_Completed;
            this._listenerEventArgs.Dispose();
            this._listener.Close();
            this._listener = null;
            this._receiveBufferSize = 0;
            this._maximumMessageBodySize = 0;
            this._maxinumSendSizeOnce = 0;
            this._totalBytesRead = 0;
            this._numConnectedSockets = 0;

            foreach (var channel in this._channels.Values) {
                channel.Close();
                channel.Dispose();
            }

            this.OnRecvData = null;
        }
    }
}