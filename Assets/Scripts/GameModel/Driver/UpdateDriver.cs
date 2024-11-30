using System;
using MemoryPack;

namespace Hsenl {
    // 不同于给组件加IUpdate, UpdateDriver可以动态的update
    [MemoryPackable]
    public partial class UpdateDriver : Unbodied, IUpdate {
        [MemoryPackIgnore]
        public Action updateInvoke;
        
        public void Update() {
            this.updateInvoke?.Invoke();
        }
    }
}