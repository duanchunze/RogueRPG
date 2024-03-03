using System;

namespace Hsenl.Network {
    // 负责管理客户端连接, 管理channel, 以及一些服务器功能
    public interface IService {
        Action<long> OnConnected { get; set; }
        Action<long> OnDisconnected { get; set; }
        public Action<long, Memory<byte>> OnRecvPackage { get; set; }
        public Action<long, ushort, Memory<byte>> OnRecvMessage { get; set; }
        public int TotalBytesRecv { get; }
        public int TotalBytesSend { get; }
        public int GetTotalBytesRecvOfChannel(long channelId);
        public int GetTotalBytesSendOfChannel(long channelId);
        public bool IsClosed { get; }

        void Start();
        public void Write(long channelId, Func<PackageBuffer, ushort> func);
        public bool Send(long channelId);
        void DisconnectChannel(long channelId);
        void Close();
    }
}