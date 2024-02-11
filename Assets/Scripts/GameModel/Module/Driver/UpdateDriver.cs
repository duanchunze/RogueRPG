using System;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class UpdateDriver : Unbodied, IUpdate {
        [MemoryPackIgnore]
        public Action updateInvoke;
        
        public void Update() {
            this.updateInvoke?.Invoke();
        }
    }
}