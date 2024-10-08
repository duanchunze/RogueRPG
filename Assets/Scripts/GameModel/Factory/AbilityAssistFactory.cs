using Hsenl.ability_assist;

namespace Hsenl {
    public static class AbilityAssistFactory {
        public static AbilityAssist Create(AbilityAssistConfig config) {
            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            var abilityAssist = entity.AddComponent<AbilityAssist>(initializeInvoke: assist => {
                assist.configId = config.Id;
                
                assist.CombinMatchMode = CombinMatchMode.Manual;
            });

            entity.AddComponent<Numeric>(initializeInvoke: node => {
                foreach (var attachValueInfo in config.NumericNodes) {
                    switch (attachValueInfo.Sign) {
                        case "f":
                            node.SetValue(new NumericKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericMode)(int)attachValueInfo.Model),
                                (float)attachValueInfo.Value);
                            break;

                        default:
                            node.SetValue(new NumericKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericMode)(int)attachValueInfo.Model),
                                (long)attachValueInfo.Value);
                            break;
                    }
                }
            });

            return abilityAssist;
        }
    }
}