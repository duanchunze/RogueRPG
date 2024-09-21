// using System;
// using System.IO;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
// using Hsenl.Network;
// using MemoryPack;
// using Test.NetworkTest;
// using UnityEngine;
//
// namespace Hsenl {
//     public class KServer : Component, ILateUpdate {
//         private KcpServer _server;
//         private HBuffer _writeBuffer = new();
//
//         protected override void OnAwake() {
//         }
//
//         protected override void OnStart() {
//             this._server = new KcpServer(new KcpServer.Configure() {
//                 LocalIPHost = "127.0.0.1",
//                 Port = 12312,
//                 RecvBufferSize = 512,
//                 SendBufferSize = 512,
//                 Backlog = 1000,
//             });
//
//             this._server.OnRecvMessage += this.OnRecvData;
//             this._server.OnRecvData = (l, memory) => {
//                 NetworkTest._stopwatch.Peek($"服务端收到数据{memory.Length}------------------------------------------");
//             };
//             this._server.OnSendData = (l, memory) => {
//                 NetworkTest._stopwatch.Peek($"服务端已经发出去了{memory.Length}------------");
//             };
//
//             this._server.AddPlug<IAfterMessageReaded, EncryptionPlug>(new DefaultEncryptionPlug());
//             this._server.AddPlug<IAfterMessageWrited, EncryptionPlug>(new DefaultEncryptionPlug());
//             TrafficMonitoringPlug trafficMonitoringPlug = new();
//             this._server.AddPlug<IOnChannelStarted, TrafficMonitoringPlug>(trafficMonitoringPlug);
//             this._server.AddPlug<IOnChannelDisconnect, TrafficMonitoringPlug>(trafficMonitoringPlug);
//             // this._server.AddPlug<IOnChannelStarted, KeepalivePlug>(new KeepalivePlug(5));
//
//             this._server.StartAccept();
//         }
//
//         public void LateUpdate() {
//             this._server?.Update((uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
//         }
//
//         protected override void OnDestroy() {
//             this._server.OnRecvMessage -= this.OnRecvData;
//             this._server.Dispose();
//         }
//
//         private void OnRecvData(long channelId, Memory<byte> data) {
//             var opcode = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(data.Slice(0, 2).Span));
//             var message = data.Slice(2).Span;
//             var t = MemoryPackSerializer.Deserialize<C2R_Login>(message);
//             Debug.LogWarning($"用户登陆, {t.account} {t.password}");
//             NetworkTest._stopwatch.Peek("服务端收到客户端请求登陆--------------------------------------");
//             // return;
//             string str = "1";
//             for (int i = 0; i < RandomHelper.NextInt(1, 50); i++) {
//                 str += i;
//             }
//             
//             var response = new R2C_Login() {
//                 RpcId = t.RpcId,
//                 ip = "1.1.1.1", 
//                 token = 31415926
//             };
//             this.Send(channelId, response);
//             NetworkTest._stopwatch.Peek("服务端回复客户端----------------------------");
//         }
//         
//         private void Send<T>(long channelId, T message) {
//             var opcode = OpcodeLookupTable.GetOpcodeOfType(message.GetType());
//             this._writeBuffer.Seek(0, SeekOrigin.Begin);
//             this._writeBuffer.GetSpan(2).WriteTo(opcode);
//             this._writeBuffer.Advance(2);
//             MemoryPackSerializer.Serialize(this._writeBuffer, message);
//             this._server.Write(channelId, this._writeBuffer.AsSpan(0, this._writeBuffer.Position));
//         }
//     }
// }