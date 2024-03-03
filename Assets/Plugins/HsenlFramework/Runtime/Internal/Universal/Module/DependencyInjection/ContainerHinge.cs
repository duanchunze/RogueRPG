using System;

namespace Hsenl {
    // 用于实现链式调用
    public ref struct ContainerHinge {
        public Container Container { get; internal set; }
        public Type BasicType { get; internal set; }
        public Type ImplementorType { get; internal set; }
    }
}