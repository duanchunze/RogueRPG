using System.Collections.Generic;

namespace Hsenl {
    // 数值节点
    public interface INumericNode {
        string NodeName { get; }
        int NodeType { get; }
        IEnumerable<uint> Keys { get; }
        NumericNodeLinkModel LinkModel { get; set; }
        bool LinkNumerator(INumerator numerator);
        bool UnlinkNumerator(INumerator numerator);
        Num GetValue(NumericNodeKey key);
        void SetValue(NumericNodeKey key, Num value);
    }
}