// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Threading;
// using Hsenl.Network;
// using MemoryPack;
// using UnityEngine;
//
// namespace Hsenl {
//     public class ClientUser : Hsenl.Component, IUpdate {
//         public static ClientUser Instance;
//
//         private IOCPClient _client;
//
//         private Dictionary<int, object> responses = new();
//
//         private int i;
//
//         protected override void OnAwake() {
//             Instance = this;
//         }
//
//         protected override void OnStart() {
//             this._client = new IOCPClient {
//                 Config = new ClientConfig {
//                     RemoteIPHost = "127.0.0.1",
//                     Port = 12312,
//                     RecvBufferSize = 32,
//                     SendBufferSize = 32
//                 }
//             };
//
//             this._client.OnRecvMessage += this.OnRecvMessage;
//
//             this._client.AddPlug<IAfterMessageReaded, EncryptionPlug>(new DefaultEncryptionPlug());
//             this._client.AddPlug<IAfterMessageWrited, EncryptionPlug>(new DefaultEncryptionPlug());
//             this._client.AddPlug<IOnChannelDisconnected, Reconnect>(new Reconnect());
//
//             this._client.Start();
//         }
//
//         protected override void OnDestroy() {
//             this._client.OnRecvMessage -= this.OnRecvMessage;
//             this._client.Close();
//         }
//
//         public void Update() {
//             var b = false;
//             if (InputController.GetButtonDown(InputCode.L)) {
//                 b = true;
//                 this.Login().Tail();
//             }
//
//             if (InputController.GetButton(InputCode.K)) {
//                 if (b) { }
//
//                 this.Login().Tail();
//             }
//
//             if (InputController.GetButtonDown(InputCode.Space)) {
//                 if (this.i != 0) {
//                     Log.Error(this.i + "----");
//                 }
//             }
//
//             if (InputController.GetButtonDown(InputCode.C)) {
//                 this._client?.Close();
//             }
//             
//             if (InputController.GetButtonDown(InputCode.D)) {
//                 this._client?.Disconnect();
//             }
//         }
//
//         private void OnRecvMessage(ushort messageId, Memory<byte> data) {
//             var t = MemoryPackSerializer.Deserialize<L2C_Login>(data.Span);
//             var task = (HTask<L2C_Login>)this.responses[1];
//             // task.SetResult(t);
//             Framework.Instance.ThreadSynchronizationContext.Post(() => { task.SetResult(t); });
//         }
//
//         private async HTask Login() {
//             Interlocked.Increment(ref this.i);
//             var date = DateTime.Now;
//             var t = await this.Request(new C2L_Login() {
//                 account = "dczedczedczezdzdcezdczdczdczdczdczdc",
//                 password = "ddddzdczdczdczdczdczdc"
//             });
//
//             Debug.Log((DateTime.Now - date).Ticks / 10000f);
//
//             Interlocked.Decrement(ref this.i);
//             Debug.LogWarning($"用户登陆回复, {t.ip} {t.token}");
//         }
//
//         private async HTask<L2C_Login> Request<T>(T message) {
//             var opcode = OpcodeLookupTable.GetOpcodeOfType(message.GetType());
//             this._client.Write(writeBuffer => {
//                 MemoryPackSerializer.Serialize(writeBuffer, message);
//                 return opcode;
//             });
//
//             var ret = this._client.Send();
//
//             var task = HTask<L2C_Login>.Create();
//             this.responses[1] = task;
//             var t = await task;
//             return t;
//         }
//
//         private void ttt() { }
//     }
// }