using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Hsenl.Network;
using MemoryPack;
using Test.NetworkTest;
using UnityEngine;

namespace Hsenl {
    public class KClient : Component, IUpdate, ILateUpdate {
        private KcpClient _client;
        private PackageBuffer _writeBuffer = new();
        private static readonly DateTimeOffset utc1970 = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private int _rpcId;
        private Dictionary<int, RpcInfo> _rpcInfos = new(); // key: rpcId

        private int i;

        protected override void OnAwake() { }

        protected override void OnStart() {
            this._client = new KcpClient(new KcpClient.Configure() {
                RemoteIPHost = "127.0.0.1",
                Port = 12312,
                RecvBufferSize = 512,
                SendBufferSize = 512,
            });

            this._client.OnRecvMessage += this.OnRecvMessage;
            this._client.OnSendData = (_, memory) => {
                NetworkTest._stopwatch.Peek($"客户端已经发出去了{memory.Length}---------------------------------------------------");
            };

            this._client.AddPlug<IAfterMessageReaded, EncryptionPlug>(new DefaultEncryptionPlug());
            this._client.AddPlug<IAfterMessageWrited, EncryptionPlug>(new DefaultEncryptionPlug());
            this._client.AddPlug<IOnChannelDisconnect, ReconnectPlug>(new ReconnectPlug());

            this._client.ConnectAsync().Tail();
        }

        protected override void OnDestroy() {
            this._client.OnRecvMessage -= this.OnRecvMessage;
            this._client.Dispose();
        }

        public void Update() { }

        public void LateUpdate() {
            var b = false;
            if (InputController.GetButtonDown(InputCode.L)) {
                b = true;
                this.Login().Tail();
            }

            if (InputController.GetButton(InputCode.K)) {
                if (b) { }

                this.Login().Tail();
            }

            if (InputController.GetButtonDown(InputCode.Space)) {
                if (this.i != 0) {
                    Log.Error(this.i + "----");
                }
            }

            if (InputController.GetButtonDown(InputCode.C)) {
                this._client?.Disconnect();
            }

            this._client?.Update((uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        private void OnRecvMessage(long _, Memory<byte> data) {
            var opcode = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(data.Slice(0, 2).Span));
            var message = data.Slice(2);
            // 判断是否是rpc消息
            // 如果是则提取data1-4位的rpcId
            var rpcId = BitConverter.ToInt32(message.Slice(1, 4).Span);
            if (this._rpcInfos.TryGetValue(rpcId, out var rpcInfo)) {
                rpcInfo.Response(message);
            }
        }

        private async HTask Login() {
            NetworkTest._stopwatch.Peek("客户端发送登陆请求-----------------------------------------------------------------------");
            Interlocked.Increment(ref this.i);
            var date = DateTime.Now;
            var t = await this.Request(new C2R_Login() {
                RpcId = 3,
                account = "dczedczedczezdzdcezdczdczdczdczdczdc",
                password = "ddddzdczdczdczdczdczdc1"
            }).As<R2C_Login>();

            Interlocked.Decrement(ref this.i);
            Debug.LogWarning($"用户登陆回复, {t.ip} {t.token}");
            NetworkTest._stopwatch.Peek("客户端收到登陆回复(实际)");
            Debug.LogWarning((DateTime.Now - date).Ticks / 10000f);
        }

        private RpcInfo Request<T>(T message) {
            var rpcId = ++this._rpcId;
            var opcode = OpcodeLookupTable.GetOpcodeOfType(message.GetType());
            this._writeBuffer.Seek(0, SeekOrigin.Begin);
            this._writeBuffer.GetSpan(2).WriteTo(opcode);
            this._writeBuffer.Advance(2);
            MemoryPackSerializer.Serialize(this._writeBuffer, message);
            this._writeBuffer.AsSpan(3, 4).WriteTo(rpcId);
            this._client.Write(0, this._writeBuffer.AsSpan(0, this._writeBuffer.Position));
            var rpcInfo = new RpcInfo() {
                task = HTask<Memory<byte>>.Create(),
            };
            
            this._rpcInfos.Add(rpcId, rpcInfo);
            return rpcInfo;
        }

        private struct RpcInfo {
            public HTask<Memory<byte>> task;

            public async HTask<T> As<T>() {
                var memory = await this.task;
                var t = MemoryPackSerializer.Deserialize<T>(memory.Span);
                return t;
            }

            public void Response(Memory<byte> data) {
                this.task.SetResult(data);
            }
        }
    }
}