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

        protected override void OnAwake() {
            this.MultiCombin(null);
            this.CrossCombinForOther(this.FindScopeInParent<Ability>(), null);
        }

        protected override void OnDestroy() {
            this.DecombinAll();
        }

        protected override void OnComponentAdd(Component component) {
            if (component is not Element element)
                return;

            this.MultiCombin(element);
            this.CrossCombinForOther(this.FindScopeInParent<Ability>(), element);
        }

        protected override void OnComponentRemove(Component component) {
            if (component is not Element element)
                return;

            this.MultiDecombin(element);
            this.CrossDecombinByComponent(element);
        }

        protected override void OnParentChanged(Entity previousParent) {
            this.CrossDecombinForParents(previousParent?.GetComponentInParent<Scope>(true, true));
            this.CrossCombinForOther(this.FindScopeInParent<Ability>(), null);
        }
    }
}