using Hsenl.ability;

namespace Hsenl {
    public static class AbilityPatchFactory {
        public static AbilityPatch Create(AbilityPatchConfig config) {
            var entity = Entity.Create(config.Alias);

            var patch = entity.AddComponent<AbilityPatch>();
            patch.configId = config.Id;

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

            var procedureLineNode = entity.AddComponent<ProcedureLineNode>();
            foreach (var workerInfo in config.Workers) {
                var worker = ProcedureLineFactory.CreateWorker<Plw>(workerInfo);
                procedureLineNode.AddWorker(worker);
            }

            return patch;
        }
    }
}