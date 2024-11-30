using System;
using Hsenl.ability;

namespace Hsenl {
    public static class AbilityTraitFactory {
        public static AbilityTrait Create(string alias) {
            var config = Tables.Instance.TbAbilityTraitConfig.GetByAlias(alias);
            return Create(config);
        }

        public static AbilityTrait Create(AbilityTraitConfig config) {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var entity = Entity.Create(config.Alias);
            var abiTrait = entity.AddComponent<AbilityTrait>();

            if (config.NumericNodes.Count != 0) {
                var numericNode = entity.AddComponent<Numeric>();
                foreach (var attachValueInfo in config.NumericNodes) {
                    switch (attachValueInfo.Sign) {
                        case "f":
                            numericNode.SetValue(new NumericKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericMode)(int)attachValueInfo.Model),
                                (float)attachValueInfo.Value);
                            break;

                        default:
                            numericNode.SetValue(new NumericKey(
                                    (uint)attachValueInfo.Type,
                                    (uint)attachValueInfo.Layer,
                                    (NumericMode)(int)attachValueInfo.Model),
                                (long)attachValueInfo.Value);
                            break;
                    }
                }
            }

            if (config.Workers.Count != 0) {
                var procedureLineNode = entity.AddComponent<ProcedureLineNode>();
                foreach (var workerInfo in config.Workers) {
                    var worker = ProcedureLineFactory.CreateWorker<Plw>(workerInfo);
                    procedureLineNode.AddWorker(worker);
                }
            }

            return abiTrait;
        }
    }
}