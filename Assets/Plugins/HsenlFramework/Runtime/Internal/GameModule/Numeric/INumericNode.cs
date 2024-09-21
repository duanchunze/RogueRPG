using System.Collections.Generic;

namespace Hsenl {
    // 数值节点
    public interface INumericNode {
        string NodeName { get; }
        int NodeType { get; }
        IEnumerable<uint> Keys { get; }
        internal bool LinkNumerator(INumerator numerator);
        internal bool UnlinkNumerator(INumerator numerator);
        Num GetValue(NumericNodeKey key);
        void SetValue(NumericNodeKey key, Num value);
    }
}