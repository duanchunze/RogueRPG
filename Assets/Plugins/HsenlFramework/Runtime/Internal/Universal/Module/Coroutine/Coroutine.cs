using System;
using System.Collections;

namespace Hsenl {
    [Serializable]
    public static class Coroutine {
        public static int Count => CoroutineManager.Instance.Count;
        public static int Start(IEnumerator enumerator) => CoroutineManager.Instance.Start(enumerator);
        public static int SingleStart(IEnumerator enumerator) => CoroutineManager.Instance.SingleStart(enumerator);
        public static bool Stop(int key) => CoroutineManager.Instance.Stop(key);
        public static void StopAll() => CoroutineManager.Instance.StopAll();
    }
}