using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hsenl.Network;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public class TServer : Component, IUpdate {
        public static TServer Instance;

        private TcpServer _service;
        private PackageBuffer _writeBuffer = new();

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            this._service = new TcpServer(new TcpServer.Configure() {
                ListenIPHost = "127.0.0.1",
                Port = 12312,
                RecvBufferSize = 32,
                SendBufferSize = 32,
                Backlog = 1000,
            });

            this._service.OnRecvMessage += this.OnRecvData;

            this._service.AddPlug<IAfterMessageReaded, EncryptionPlug>(new DefaultEncryptionPlug());
            this._service.AddPlug<IAfterMessageWrited, EncryptionPlug>(new DefaultEncryptionPlug());
            // this._service.AddPlug<IOnChannelStarted, KeepalivePlug>(new KeepalivePlug());

            this._service.StartAccept();
        }

        protected override void OnDestroy() {
            this._service.OnRecvMessage -= this.OnRecvData;
            this._service.Dispose();
        }

        private void OnRecvData(long channelId, Memory<byte> data) {
            var opcode = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(data.Slice(0, 2).Span));
            var message = data.Slice(2).Span;
            var t = MemoryPackSerializer.Deserialize<C2R_Login>(message);
            Debug.LogWarning($"用户登陆, {t.account} {t.password}");
            // string str = "1";
            // for (int i = 0; i < RandomHelper.mtRandom.NextInt(1, 50); i++) {
            //     str += i;
            // }
            // var response = new L2C_Login() { ip = str, token = 31415926 };
            var response = new R2C_Login() { ip = "1.1.1.1", token = 31415926 };
            this.Send(channelId, response);
        }

        private void Send<T>(long channelId, T message) {
            var opcode = OpcodeLookupTable.GetOpcodeOfType(message.GetType());
            this._writeBuffer.Seek(0, SeekOrigin.Begin);
            this._writeBuffer.GetSpan(2).WriteTo(opcode);
            this._writeBuffer.Advance(2);
            MemoryPackSerializer.Serialize(this._writeBuffer, message);
            this._service.Write(channelId, this._writeBuffer.AsSpan(0, this._writeBuffer.Position));
            this._service.StartSend(channelId);
        }

        public void Update() { }
    }
}