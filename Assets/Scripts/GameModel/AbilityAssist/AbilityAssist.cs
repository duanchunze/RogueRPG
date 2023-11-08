using Hsenl.ability;
using Hsenl.ability_assist;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class AbilityAssist : Bodied {
        [MemoryPackOrder(50)]
        [MemoryPackInclude]
        public int configId;

        [MemoryPackIgnore]
        public AbilityAssistConfig Config => Tables.Instance.TbAbilityAssistConfig.GetById(this.configId);
    }
}