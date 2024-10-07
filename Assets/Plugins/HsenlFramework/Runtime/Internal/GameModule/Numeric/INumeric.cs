using System.Collections.Generic;

namespace Hsenl {
    // 数值节点
    public interface INumeric {
        uint MaxLayer { get; }
        IEnumerable<uint> Keys { get; }
        internal bool LinkNumerator(INumerator numerator);
        internal bool UnlinkNumerator(INumerator numerator);
        Num GetValue(NumericKey key);
        void SetValue(NumericKey key, Num value);
    }
}