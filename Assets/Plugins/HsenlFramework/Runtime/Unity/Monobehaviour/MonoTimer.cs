using System;
using UnityEngine;

namespace Hsenl {
    public class MonoTimer : MonoBehaviour {
        public async void TimeStart(float time, Action callback) {
            await Timer.WaitTime((long)(time * 1000));
            callback.Invoke();
        }
    }
}