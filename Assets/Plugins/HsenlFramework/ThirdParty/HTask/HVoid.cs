using System.Runtime.CompilerServices;

namespace Hsenl {
    [AsyncMethodBuilder(typeof(AsyncHVoidMethodBuilder))]
    public struct HVoid {
        // 小尾巴, 没实际作用, 让编译器不警告的
        public void Tail() { }
    }
}