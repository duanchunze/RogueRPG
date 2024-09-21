// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading.Tasks;
// using Hsenl;
// using Hsenl.Network;
// using UnityEngine;
// using Coroutine = Hsenl.Coroutine;
//
// namespace Test.NetworkTest {
//     public class NetworkTest : MonoBehaviour {
//         public static LogStopwatch _stopwatch;
//         public int testKind;
//
//         private void Start() {
//             _stopwatch = new LogStopwatch("通讯每步耗时");
//             Hsenl.SceneManager.GetOrLoadDontDestroyScene();
//
//             switch (this.testKind) {
//                 case 0: {
//                     var entity = Entity.Create("TServer");
//                     entity.AddComponent<TServer>();
//
//                     entity = Entity.Create("TClient");
//                     entity.AddComponent<TClient>();
//                     break;
//                 }
//
//                 case 1: {
//                     var entity2 = Entity.Create("KServer");
//                     entity2.AddComponent<KServer>();
//
//                     entity2 = Entity.Create("KClient");
//                     entity2.AddComponent<KClient>();
//                     break;
//                 }
//                 
//                 case 2: {
//                     var entity2 = Entity.Create("KServer");
//                     entity2.AddComponent<KServer>();
//
//                     entity2 = Entity.Create("KClient");
//                     entity2.AddComponent<KClient>();
//                     entity2 = Entity.Create("KClient2");
//                     entity2.AddComponent<KClient>();
//                     break;
//                 }
//                 
//                 case 3: {
//                     var entity2 = Entity.Create("KServer");
//                     entity2.AddComponent<KServer>();
//
//                     entity2 = Entity.Create("KClient");
//                     entity2.AddComponent<KClient>();
//                     entity2 = Entity.Create("KClient2");
//                     entity2.AddComponent<KClient>();
//                     entity2 = Entity.Create("KClient3");
//                     entity2.AddComponent<KClient>();
//                     break;
//                 }
//             }
//         }
//     }
// }