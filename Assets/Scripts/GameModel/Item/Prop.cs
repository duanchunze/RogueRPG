using Hsenl.item;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Prop : Bodied {
        [MemoryPackOrder(50)]
        [MemoryPackInclude]
        public int configId;

        [MemoryPackIgnore]
        public PropConfig Config => Tables.Instance.TbPropConfig.GetById(this.configId);
    }
}