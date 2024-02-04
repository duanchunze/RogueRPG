using Hsenl.procedureline;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    [MemoryPackable()]
    public partial class PlwAdditionalStatusOnAbilityHarm : PlwInfo<AdditionalStatusOnAbilityDamageInfo> {
#if UNITY_EDITOR
        [ShowInInspector]
        [MemoryPackIgnore]
        public string AdditionalStatusName => this.info.StatusAlias;
#endif
    }
}