using Hsenl.procedureline;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    [MemoryPackable()]
    public partial class PlwAdditionalStatusOnAbilityHarm2 : PlwInfo<AdditionalStatusOnAbilityDamageInfo2> {
#if UNITY_EDITOR
        [ShowInInspector]
        [MemoryPackIgnore]
        public string AdditionalStatusName => this.info.StatusAlias;
#endif
    }
}