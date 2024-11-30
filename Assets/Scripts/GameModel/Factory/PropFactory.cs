using Hsenl.item;

namespace Hsenl {
    public static class PropFactory {
        public static Prop Create(PropConfig config) {
            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            var abilityAssist = entity.AddComponent<Prop>();
            abilityAssist.configId = config.Id;

            entity.AddComponent<Numeric, PropConfig>(config, (c, cfg) => {
                foreach (var attachValueInfo in cfg.NumericNodes) {
                    switch (attachValueInfo.Sign) {
                        case "f":
                            c.SetValue(new NumericKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericMode)(int)attachValueInfo.Model),
                                (float)attachValueInfo.Value);
                            break;

                        default:
                            c.SetValue(new NumericKey(
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