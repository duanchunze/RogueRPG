using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Collider : Unbodied {
        [MemoryPackIgnore]
        public Rigidbody Rigidbody => this.GetComponentInParent<Rigidbody>();
    }
}