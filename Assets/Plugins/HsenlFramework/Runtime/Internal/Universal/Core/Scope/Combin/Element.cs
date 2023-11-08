using MemoryPack;

namespace Hsenl {
    [RequireComponent(typeof(Scope))] // element会参与到组合的配对中, 所以必须有前置组件scope
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Element : Component { }
}