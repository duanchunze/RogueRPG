using System;

namespace Hsenl {
    public readonly struct NumericKey {
        public readonly uint key;
        public readonly uint layer;

        public static (uint numType, uint layer, NumericMode mode) Split(uint keyValue) {
            return (keyValue >> NumericConst.NumericTypeOffset, 
                (keyValue >> NumericConst.NumericLayerOffset) & NumericConst.NumericMaxLayerNumInTheory, 
                (NumericMode)(keyValue & NumericConst.NumericMaxModeNumlInTheory));
        }

        public static uint GetLayerOfKey(uint keyValue) {
            return (keyValue >> NumericConst.NumericLayerOffset) & NumericConst.NumericMaxLayerNumInTheory;
        }

        public NumericKey(uint numericType, uint numericLayer, NumericMode numericMode) {
            // 因为numericType 和 layer 都需要做 << 运算操作, 所以, 二者皆不能小于1
            if (numericType < 1) throw new ArgumentException($"numeric type must greater than 1. '{numericType}'");
            if (numericLayer < 1) throw new ArgumentException($"numeric layer must greater than 1. '{numericLayer}'");
            if (numericLayer > NumericConst.NumericMaxLayerNumInTheory) throw new ArgumentException($"numeric layer cannot greater than max layer. '{numericLayer}' '{NumericConst.NumericMaxLayerNumInTheory}'");
            this.key = (numericType << NumericConst.NumericTypeOffset) + (numericLayer << NumericConst.NumericLayerOffset) + (uint)numericMode;
            this.layer = numericLayer;
        }
        
        public override string ToString() {
            return this.key.ToString();
        }
    }
}