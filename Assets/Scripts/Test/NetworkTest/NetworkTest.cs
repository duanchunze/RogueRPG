using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Hsenl;
using Hsenl.Network;
using UnityEngine;
using Coroutine = Hsenl.Coroutine;

namespace Test.NetworkTest {
    public class NetworkTest : MonoBehaviour {
        private void Start() {
            Hsenl.SceneManager.GetOrLoadDontDestroyScene();
            
            var entity = Entity.Create("ServerUser");
            entity.AddComponent<Server>();

            entity = Entity.Create("ClientUser");
            entity.AddComponent<ClientUser>();
        }
    }
}