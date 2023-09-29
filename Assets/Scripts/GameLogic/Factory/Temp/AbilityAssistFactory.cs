using Hsenl.ability_assist;

namespace Hsenl {
    public static class AbilityAssistFactory {
        public static AbilityAssist Create(AbilityAssistConfig config) {
            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            var abilityAssist = entity.AddComponent<AbilityAssist>(initializeInvoke: assist => { assist.configId = config.Id; });

            entity.AddComponent<NumericNode>(initializeInvoke: node => {
                node.LinkModel = NumericNodeLinkModel.AutoLinkToParent;
                foreach (var attachValueInfo in config.NumericNodes) {
                    switch (attachValueInfo.Sign) {
                        case "f":
                            node.SetValue(new NumericNodeKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericNodeModel)(int)attachValueInfo.Model),
                                attachValueInfo.Value);
                            break;

                        default:
                            node.SetValue(new NumericNodeKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericNodeModel)(int)attachValueInfo.Model),
                                (long)attachValueInfo.Value);
                            break;
                    }
                }
            });

            return abilityAssist;
        }
    }
}