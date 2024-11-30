using System;
using Hsenl.ability;
using Hsenl.common;

namespace Hsenl {
    public static class AbilityFactory {
        public static Ability Create(string alias) {
            var config = Tables.Instance.TbAbilityConfig.GetByAlias(alias);
            return Create(config);
        }

        public static Ability Create(AbilityConfig config) {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            // 一个技能的组成:
            // 控制触发器, 优先器, 施法器, 阶段线, 技能本体. 根据情况不同, 可以不要控制器, 比如有些靠条件触发的技能, 比如卢安娜的飓风
            var ability = entity.AddComponent<Ability>();
            ability.SetConfigId(config.Id);
            ability.factionTypes.Clear();
            foreach (var factionType in config.TargetTags) {
                ability.factionTypes.Add(factionType);
            }

            entity.AddComponent<ProcedureLine>();
            var procedureLineNode = entity.AddComponent<ProcedureLineNode>();

            switch (config.Caster) {
                case cast.ControlCastInfo controlCastInfo: {
                    entity.AddComponent<Caster>();
                    var controlTrigger = entity.AddComponent<ControlTrigger>();
                    controlTrigger.ControlCode = (int)controlCastInfo.Code;
                    controlTrigger.supportBurstFire = controlCastInfo.SupportBurstFire;
                    break;
                }

                case cast.AlwaysCastInfo: {
                    entity.AddComponent<Caster>();
                    entity.AddComponent<UpdateDriver>();
                    break;
                }

                default: {
                    if (config.CasterOfPlw is procedureline.CastWorkerNull)
                        break;

                    entity.AddComponent<Caster>();
                    var worker = ProcedureLineFactory.CreateWorker<Plw>(config.CasterOfPlw);
                    procedureLineNode.AddWorker(worker);
                    break;
                }
            }

            entity.AddComponent<Numerator, AbilityConfig>(config, (c, cfg) => {
                c.allowAttacherExpand = true;
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

            var caster = entity.GetComponent<Caster>();
            if (caster != null) {
                var entryNode = BehaviorNodeFactory.CreateNodeLink<Caster>(config.CasterEvaluates);
                caster.SetEntryNode(entryNode);
            }

            if (!string.IsNullOrEmpty(config.EvaluatePriorityState)) {
                var pscfg = Tables.Instance.TbPriorityStateConfig.Get(config.EvaluatePriorityState);
                entity.AddComponent<CastEvaluatePriorityState, PriorityStateConfig>(pscfg, (c, cfg) => {
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
            }

            entity.AddComponent<PriorityState, AbilityConfig>(config, (c, cfg) => {
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

            var stageline = entity.AddComponent<StageLine>();
            stageline.SetEntryNode(new SelectorNode<ITimeLine, IStageNode>());
            foreach (var stageInfo in config.Stages) {
                if (stageInfo.StageType == StageType.None)
                    continue;

                Stage stage = null;
                foreach (var actionInfo in stageInfo.Actions) {
                    stage ??= new Stage {
                        StageType = (int)stageInfo.StageType,
                        Duration = stageInfo.Duration,
                    };

                    ITimeNode action;
                    try {
                        action = BehaviorNodeFactory.CreateNode<ITimeNode>(actionInfo);
                    }
                    catch (Exception e) {
                        Log.Error(e);
                        continue;
                    }

                    stage.AddChild(action);
                }

                stage ??= new Stage {
                    StageType = (int)stageInfo.StageType,
                    Duration = stageInfo.Duration,
                };

                stageline.EntryNode.AddChild(stage);
            }

            stageline.EntryNode.AddChild(new FinishStageAction());

            // 技能的特性, 里面包含数值, worker等元素
            foreach (var traitAlias in config.Traits) {
                try {
                    var trait = AbilityTraitFactory.Create(traitAlias);
                    trait.SetParent(entity);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            return ability;
        }
    }
}