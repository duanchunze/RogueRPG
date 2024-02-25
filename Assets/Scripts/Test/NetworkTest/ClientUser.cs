using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Hsenl.Network;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public class ClientUser : Component, IUpdate {
        public static ClientUser Instance;

        public IOCPClient IocpClient { get; private set; }

        private Dictionary<int, object> responses = new();

        private int i;

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            IPAddress.TryParse("127.0.0.1", out var address);
            var endPoint = new IPEndPoint(address, 12312);
            this.IocpClient = new IOCPClient(32, 32);
            this.IocpClient.StartConnecting(endPoint);
            this.IocpClient.OnRecvData += this.OnRecvData;
        }

        protected override void OnDestroy() {
            this.IocpClient.OnRecvData -= this.OnRecvData;
            this.IocpClient.Close();
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
                if (i != 0) {
                    Log.Error(i + "----");
                }
            }
        }

        private void OnRecvData(IOCPChannel channel, Memory<byte> data) {
            Interlocked.Decrement(ref this.i);
            var t = MemoryPackSerializer.Deserialize<G2C_Login>(data.Span.Slice(8));
            var task = (HTask<G2C_Login>)this.responses[1];
            task.SetResult(t);
        }

        private async HTask Login() {
            var t = await this.Request(new C2G_Login() {
                account = "dczedczedczezdzdcezdczdczdczdczdczdc",
                password = "ddddzdczdczdczdczdczdc"
            });

            // Debug.Log(t.ip);
            // Debug.Log(t.verificationCode);
        }

        private async HTask<G2C_Login> Request<T>(T message) {
            var buffer = this.IocpClient.SendBuffer;
            var postion = (int)buffer.Position;
            buffer.Advance(4);
            buffer.Advance(4);
            buffer.RecordWritePoint();
            MemoryPackSerializer.Serialize(buffer, message);
            var writeLen = buffer.EndWriteRecord();
            buffer.AsSpan(postion + 0, 4).WriteTo(writeLen);
            buffer.AsSpan(postion + 4, 8).WriteTo(256);

            // postion = (int)buffer.Position;
            // buffer.Advance(4);
            // buffer.Advance(4);
            // buffer.RecordWritePoint();
            // var msg = new C2G_Login() { account = "123", password = "456" };
            // MemoryPackSerializer.Serialize(buffer, msg);
            // writeLen = buffer.EndWriteRecord();
            // buffer.AsSpan(postion + 0, 4).WriteTo(writeLen);
            // buffer.AsSpan(postion + 4, 8).WriteTo(256);

            var ret = this.IocpClient.Send();
            Interlocked.Increment(ref this.i);

            var task = HTask<G2C_Login>.Create();
            this.responses[1] = task;
            var t = await task;
            return t;
        }

        private void ttt() { }
    }
}