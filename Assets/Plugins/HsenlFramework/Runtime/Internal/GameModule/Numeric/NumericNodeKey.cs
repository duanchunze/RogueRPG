using System;

namespace Hsenl {
    public readonly struct NumericNodeKey {
        public readonly uint key;

        public static (uint numType, uint layer, NumericNodeModel model) Split(uint keyValue) {
            return (keyValue >> NumericConst.NumericTypeOffset, 
                (keyValue >> NumericConst.NodeLayerOffset) & NumericConst.NodeMaxLayerNumInTheory, 
                (NumericNodeModel)(keyValue & NumericConst.NodeMaxModeNumlInTheory));
        }

        public NumericNodeKey(uint numericType, uint nodeLayer, NumericNodeModel nodeModel) {
            // 因为numericType 和 nodeLayer 都需要做 <<运算操作, 所以, 二者皆不能小于1
            if (numericType < 1) throw new ArgumentException($"numeric type must greater than 1. '{numericType}'");
            if (nodeLayer < 1) throw new ArgumentException($"node layer must greater than 1. '{nodeLayer}'");
            if (nodeLayer > Numerator.MaxLayerNum) throw new ArgumentException($"node layer cannot greater than max node layer. '{nodeLayer}' '{Numerator.MaxLayerNum}'");
            this.key = (numericType << NumericConst.NumericTypeOffset) + (nodeLayer << NumericConst.NodeLayerOffset) + (uint)nodeModel;
        }
        
        public override string ToString() {
            return this.key.ToString();
        }
    }
}