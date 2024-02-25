using System;
using System.Net;
using Hsenl;
using Hsenl.Network;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public class Server : Component, IUpdate {
        public static Server Instance;

        public IOCPServer IocpServer { get; private set; }

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            this.IocpServer = new IOCPServer(32, 32);
            IPAddress.TryParse("127.0.0.1", out var address);
            var endPoint = new IPEndPoint(address, 12312);
            this.IocpServer.StartListening(endPoint);
            this.IocpServer.OnRecvData += OnRecvData;
        }

        protected override void OnDestroy() {
            this.IocpServer.OnRecvData -= OnRecvData;
            this.IocpServer.Close();
        }

        private static void OnRecvData(IOCPChannel channel, Memory<byte> data) {
            var t = MemoryPackSerializer.Deserialize<C2G_Login>(data.Span.Slice(8));
            // Debug.Log($"用户登陆, {t.account} {t.password}");
            var response = new G2C_Login() { ip = "127.0.0.1", verificationCode = 31415926 };
            var buffer = channel.SendBuffer;
            buffer.Advance(8);
            buffer.RecordWritePoint();
            MemoryPackSerializer.Serialize(channel.SendBuffer, response);
            var writelen = buffer.EndWriteRecord();
            buffer.AsSpan(0, 4).WriteTo(writelen);
            buffer.AsSpan(4, 8).WriteTo(857);
        }

        public void Update() { }
    }
}