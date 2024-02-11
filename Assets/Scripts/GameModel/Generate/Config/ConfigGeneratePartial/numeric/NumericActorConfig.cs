using System.Collections.Generic;

namespace Hsenl.numeric {
    public partial class NumericActorConfig {
        private readonly Dictionary<NumericType, Num> _nums = new();

        partial void PostInit() {
            foreach (var numericInfo in this.NumericInfos) {
                switch (numericInfo.Sign) {
                    case "f":
                        this._nums.Add(numericInfo.Type, numericInfo.Value);
                        break;
                    default:
                        this._nums.Add(numericInfo.Type, (long)numericInfo.Value);
                        break;
                }
            }
        }

        public bool TryGetNum(NumericType numericType, out Num num) {
            return this._nums.TryGetValue(numericType, out num);
        }
    }
}