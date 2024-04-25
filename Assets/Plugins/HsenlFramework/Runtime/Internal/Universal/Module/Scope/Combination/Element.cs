using System;
using MemoryPack;

namespace Hsenl {
    [RequireComponent(typeof(Scope))] // element会参与到组合的配对中, 所以必须有前置组件scope
    [MemoryPackable(GenerateType.CircularReference)]
    [ComponentOptions(ComponentMode = ComponentMode.Single)]
    public partial class Element : Component {
        [OnEventSystemInitialized]
        private static void CheckOptions() {
            // 因为组合不支持重复组件, 所以需要检查, 所有的element组件, 都必须使用single模式
            var elementType = typeof(Element);
            foreach (var kv in Entity.componentOptions) {
                if (elementType.IsAssignableFrom(kv.Key)) {
                    if (kv.Value.ComponentMode != ComponentMode.Single) {
                        throw new Exception($"All element components muse be use <Single Mode> '{kv.Key}'");
                    }
                }
            }
        }
    }
}