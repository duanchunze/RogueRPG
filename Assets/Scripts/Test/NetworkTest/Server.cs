using System;
using Hsenl.Network;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public class Server : Component, IUpdate {
        public static Server Instance;

        private IOCPService _service;

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            this._service = new IOCPService() {
                Config = new ServiceConfig {
                    ListenIPHost = "127.0.0.1",
                    Port = 12312,
                    RecvBufferSize = 32,
                    SendBufferSize = 32,
                    Backlog = 1000,
                }
            };

            this._service.OnRecvMessage += OnRecvData;
            
            this._service.AddPlug<IAfterMessageReaded, EncryptionPlug>(new DefaultEncryptionPlug());
            this._service.AddPlug<IAfterMessageWrited, EncryptionPlug>(new DefaultEncryptionPlug());
            this._service.AddPlug<IOnChannelStarted, KeepalivePlug>(new KeepalivePlug());

            this._service.Start();
        }

        protected override void OnDestroy() {
            this._service.OnRecvMessage -= OnRecvData;
            this._service.Close();
        }

        private void OnRecvData(long channelId, ushort opcode, Memory<byte> data) {
            var t = MemoryPackSerializer.Deserialize<C2L_Login>(data.Span);
            Debug.LogWarning($"用户登陆, {t.account} {t.password}");
            string str = "1";
            for (int i = 0; i < RandomHelper.mtRandom.NextInt(1, 50); i++) {
                str += i;
            }
            
            var response = new L2C_Login() { ip = str, token = 31415926 };
            var responseOpcode = OpcodeLookupTable.GetOpcodeOfType(response.GetType());
            this._service.Write(channelId, buffer => {
                MemoryPackSerializer.Serialize(buffer, response);
                return responseOpcode;
            });
            
            this._service.Send(channelId);
        }

        public void Update() { }
    }
}