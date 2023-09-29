using System;
using System.Collections.Generic;

namespace Hsenl {
    // 解锁情况.
    [Serializable]
    public class UnlockCircumstance : SingletonComponent<UnlockCircumstance> {
        public List<int> heroUnlock = new();
        public List<int> cardUnlock = new();
    }
}