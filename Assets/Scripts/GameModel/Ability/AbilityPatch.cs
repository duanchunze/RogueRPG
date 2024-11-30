using System;
using Hsenl.ability;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class AbilityPatch : Bodied {
        [MemoryPackOrder(50)]
        [MemoryPackInclude]
        public int configId;

        [MemoryPackIgnore]
        public AbilityPatchConfig Config => Tables.Instance.TbAbilityPatchConfig.GetById(this.configId);

        [MemoryPackIgnore]
        public Ability RealTargetAbility;
    }
}