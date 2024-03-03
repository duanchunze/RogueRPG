using System;

namespace Hsenl.Network {
    public interface IClient {
        Action OnConnected { get; set; }
        Action OnDisconnected { get; set; }
        public Action<Memory<byte>> OnRecvPackage { get; set; }
        public Action<ushort, Memory<byte>> OnRecvMessage { get; set; }
        public int TotalBytesRecv { get; }
        public int TotalBytesSend { get; }
        public bool IsClosed { get; }
        public bool IsConnecting { get; }

        void Start();
        public HTask StartAsync();
        public void Write(Func<PackageBuffer, ushort> func);
        public bool Send();
        public void Disconnect();
        void Close();
    }
}