using System;
using Hsenl.actor;
using Hsenl.status;

namespace Hsenl {
    public static class StatusFactory {
        public static Status CreateActorStatus(StatusConfig config) {
            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            // var status = entity.AddComponent(new Status { configId = config.Id });
            var status = entity.AddComponent<Status, StatusConfig>(config, (c, statusConfig) => {
                c.configId = statusConfig.Id;
            });

            entity.AddComponent<PriorityState, StatusConfig>(config, (c, cfg) => {
                c.TimeScale = cfg.PriorityState.TimeScale;
                c.Duration = cfg.PriorityState.Duration;
                c.InitAisles(cfg.PriorityState.Aisles);
                c.InitPriorities(
                    cfg.PriorityState.EnterPri,
                    cfg.PriorityState.ObsPri,
                    cfg.PriorityState.KeepPri,
                    cfg.PriorityState.ExcluPri,
                    cfg.PriorityState.RunPri,
                    cfg.PriorityState.DisPri);

                var ips = (IPriorityState)c;
                ips.AddSpecialPassedOfLabels(cfg.PriorityState.SpPass);
                ips.AddSpecialObstructOfLabels(cfg.PriorityState.SpObs);
                ips.AddSpecialKeepOfLabels(cfg.PriorityState.SpKeep);
                ips.AddSpecialExclusionOfLabels(cfg.PriorityState.SpExclu);
                ips.AddSpecialRunOfLabels(cfg.PriorityState.SpRun);
                ips.AddSpecialDisableOfLabels(cfg.PriorityState.SpDis);
                c.allowReenter = cfg.PriorityState.AllowReenter;
            });

            entity.AddComponent<Numerator, StatusConfig>(config, (c, cfg) => {
                foreach (var basicValueInfo in cfg.NumericInfos) {
                    switch (basicValueInfo.Sign) {
                        case "f":
                            c.SetValue(basicValueInfo.Type, basicValueInfo.Value);
                            break;
                        default:
                            c.SetValue(basicValueInfo.Type, (long)basicValueInfo.Value);
                            break;
                    }
                }
            });

            entity.AddComponent<Numeric, StatusConfig>(config, (c, cfg) => {
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

            var procedureLineNode = entity.AddComponent<ProcedureLineNode>();
            foreach (var workerInfo in config.Workers) {
                if (workerInfo is procedureline.CastWorkerNull)
                    continue;

                entity.AddComponent<Caster>();
                var worker = ProcedureLineFactory.CreateWorker<Plw>(workerInfo);
                procedureLineNode.AddWorker(worker);
            }

            var timeline = entity.AddComponent<TimeLine>();
            timeline.SetEntryNode(new ParallelNode<ITimeLine, ActionNode<ITimeLine>>());
            timeline.TillTime = config.Duration;
            foreach (var actionInfo in config.Main.Actions) {
                try {
                    var action = BehaviorNodeFactory.CreateNode<ActionNode<ITimeLine>>(actionInfo);
                    timeline.EntryNode.AddChild(action);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            return status;
        }
    }
}