// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Hsenl;
// using UnityEngine;
//
// namespace Test {
//     public class HThreadTest : MonoBehaviour {
//         private void Update() { }
//
//         private void Start() {
//             this.Test1();
//             this.Test2();
//         }
//
//         private void Test1() {
//             HTaskLine taskLine = new();
//             this.Log(taskLine, 1);
//             this.Log(taskLine, 3);
//             this.Log(taskLine, 2);
//             this.Log(taskLine, 0);
//             this.Log(taskLine, 4);
//             this.Log(taskLine, 5);
//             this.Log(taskLine, 6);
//         }
//
//         private async void Log(HTaskLine taskLine, int position) {
//             var pos = position;
//             using (await taskLine.Wait(position)) {
//                 await HTask.Run(() => { Debug.Log(pos); });
//             }
//         }
//
//         private async void Test2() {
//             await HTask.Run(() => { throw new Exception("12312"); });
//         }
//     }
// }