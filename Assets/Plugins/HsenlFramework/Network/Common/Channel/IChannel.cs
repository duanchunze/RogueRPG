using System;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 负责数据I\O, 并决定采取何种收发方案
    public interface IChannel {
        public long ChannelId { get; set; }

        public Socket Socket { get; }

        public bool IsSending { get; }

        public int TotalBytesRecv { get; }

        public int TotalBytesSend { get; }

        public Action<long, Memory<byte>> OnRecvPackage { get; set; }

        public Action<long, ushort, Memory<byte>> OnRecvMessage { get; set; }

        public Action<long, Memory<byte>> OnSendPackage { get; set; }

        public Action<long, ushort, Memory<byte>> OnSendMessage { get; set; }

        public Action<long, int> OnError { get; set; }

        public void Init(int recvBufferSize, int sendBufferSize);

        // 启动
        public void Start(Socket s);

        public void Write(Func<PackageBuffer, ushort> func);

        public bool Send();

        public void Disconnect();

        public void Dispose();
    }
}