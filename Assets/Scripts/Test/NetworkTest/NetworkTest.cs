using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Hsenl;
using Hsenl.Network.Client;
using Hsenl.Network.Server;
using UnityEngine;
using Coroutine = Hsenl.Coroutine;

namespace Test.NetworkTest {
    public class NetworkTest : MonoBehaviour {
        private IOCPServer _server;
        private List<SocketClient> _clients = new();

        private void Start() {
            this._server = new IOCPServer(3, 1024);
            IPAddress.TryParse("127.0.0.1", out var address);
            var endPoint = new IPEndPoint(address, 0721);
            this._server.Init();
            this._server.StartListening(endPoint);

            var client = new SocketClient();
            this._clients.Add(client);
            client.StartConnect(endPoint);
        }

        private void Update() {
            if (InputController.GetButtonDown(InputCode.Space)) {
                
            }
            
            if (InputController.GetButtonDown(InputCode.C)) {
                
            }
            
            if (InputController.GetButtonDown(InputCode.S)) {
                this._server.SendMessage("你好");
            }
        }
        
        private void OnDestroy() {
            foreach (var client in this._clients) {
                client.Close();
            }

            this._server.Close();
        }
    }
}