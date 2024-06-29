using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Hsenl.Network;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public class TClient : Component, IUpdate {
        public static TClient Instance;

        private TcpClient _client;

        private PackageBuffer _writeBuffer = new();
        private HTask<R2C_Login> task;

        private int i;

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            this._client = new TcpClient(new TcpClient.Configure() {
                RemoteIPHost = "127.0.0.1",
                Port = 12312,
                RecvBufferSize = 32,
                SendBufferSize = 32,
            });

            this._client.OnRecvMessage += this.OnRecvMessage;

            this._client.AddPlug<IAfterMessageReaded, EncryptionPlug>(new DefaultEncryptionPlug());
            this._client.AddPlug<IAfterMessageWrited, EncryptionPlug>(new DefaultEncryptionPlug());
            this._client.AddPlug<IOnChannelDisconnect, ReconnectPlug>(new ReconnectPlug());

            this._client.ConnectAsync().Tail();
        }

        protected override void OnDestroy() {
            this._client.OnRecvMessage -= this.OnRecvMessage;
            this._client.Dispose();
        }

        public void Update() {
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
        }

        private void OnRecvMessage(long _, Memory<byte> data) {
            var opcode = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(data.Slice(0, 2).Span));
            var message = data.Slice(2).Span;
            var t = MemoryPackSerializer.Deserialize<R2C_Login>(message);
            Framework.Instance.ThreadSynchronizationContext.Post(() => {
                // 由于这里测试rpc没有使用唯一id做标记, 所以可能出现漏回复的问题, 消息本身没有问题
                this.task.SetResult(t);
            });
        }

        private async HTask Login() {
            Interlocked.Increment(ref this.i);
            var date = DateTime.Now;
            var t = await this.Send(new C2R_Login() {
                account = "dczedczedczezdzdcezdczdczdczdczdczdc",
                password = "ddddzdczdczdczdczdczdc"
            });

            Interlocked.Decrement(ref this.i);
            Debug.LogWarning($"用户登陆回复, {t.RpcId} {t.ip} {t.token}");
            Debug.Log((DateTime.Now - date).Ticks / 10000f);
        }

        private async HTask<R2C_Login> Send<T>(T message) {
            var opcode = OpcodeLookupTable.GetOpcodeOfType(message.GetType());
            this._writeBuffer.Seek(0, SeekOrigin.Begin);
            this._writeBuffer.GetSpan(2).WriteTo(opcode);
            this._writeBuffer.Advance(2);
            MemoryPackSerializer.Serialize(this._writeBuffer, message);
            this._client.Write(0, this._writeBuffer.AsSpan(0, this._writeBuffer.Position));
            this._client.StartSend(0);
            var t = HTask<R2C_Login>.Create();
            this.task = t;
            var v = await t;
            return v;
        }
    }
}