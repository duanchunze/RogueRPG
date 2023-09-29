using System.Collections.Generic;
using MemoryPack;

namespace Test {
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class TestObject { }

    [MemoryPackable]
    public partial class TestEntity : TestObject {
        public string name;

        public List<TestComponent> components = new();
        public List<TestEntity> entities = new();
    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class TestComponent : TestObject {
        [MemoryPackOrder(0)]
        public int id;
    }

    [MemoryPackable(GenerateType.CircularReference)]
    public partial class TestSubstantive : TestComponent { }
}