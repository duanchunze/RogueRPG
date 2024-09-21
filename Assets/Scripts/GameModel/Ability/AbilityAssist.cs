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