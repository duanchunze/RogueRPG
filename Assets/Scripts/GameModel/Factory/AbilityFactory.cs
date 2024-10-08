using System;
using Hsenl.ability;

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
            var ability = entity.AddComponent<Ability>(initializeInvoke: abi => {
                // 注意这里我们把abi的组合匹配改为手动了, 所以abi里写了很多关于匹配的代码
                // 这么做的目的是为了让abi的组合更有针对性, 且让我们可以更灵活的控制abi的组合与解组
                abi.CombinMatchMode = CombinMatchMode.Manual;

                abi.SetConfigId(config.Id);
                abi.factionTypes.Clear();
                foreach (var factionType in config.TargetTags) {
                    abi.factionTypes.Add(factionType);
                }
            });

            var pl = entity.AddComponent<ProcedureLine>();
            var procedureLineNode = entity.AddComponent<ProcedureLineNode>();

            switch (config.Caster) {
                case cast.ControlCastInfo controlCastInfo: {
                    entity.AddComponent<Caster>();
                    var controlTrigger = entity.AddComponent<ControlTrigger>();
                    controlTrigger.ControlCode = (int)controlCastInfo.Code;
                    controlTrigger.supportContinue = controlCastInfo.SupportContinue;
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
                    var worker = ProcedureLineFactory.CreateWorker<PlwInfo>(config.CasterOfPlw);
                    procedureLineNode.AddWorker(worker);
                    break;
                }
            }

            if (entity.GetComponent<Caster>() != null) {
                var evaluate = entity.AddComponent<CasterEvaluate>();
                var entryNode = BehaviorNodeFactory.CreateNodeLink<CasterEvaluate>(config.CasterEvaluates);
                evaluate.SetEntryNode(entryNode);
            }

            var numerator = entity.AddComponent<Numerator>();
            foreach (var basicValueInfo in config.NumericInfos) {
                switch (basicValueInfo.Sign) {
                    case "f":
                        numerator.SetValue(basicValueInfo.Type, basicValueInfo.Value);
                        break;
                    default:
                        numerator.SetValue(basicValueInfo.Type, (long)basicValueInfo.Value);
                        break;
                }
            }

            entity.AddComponent<PriorityState>(initializeInvoke: state => {
                state.timeScale = config.PriorityState.TimeScale;
                state.duration = config.PriorityState.Duration;
                state.aisles = config.PriorityState.Aisles.ToArray();
                state.enterPriority = config.PriorityState.EnterPri;
                state.resistPriorityAnchor = config.PriorityState.ResistPri;
                state.keepPriority = config.PriorityState.KeepPri;
                state.exclusionPriority = config.PriorityState.ExcluPri;
                state.runPriority = config.PriorityState.RunPri;
                state.disablePriority = config.PriorityState.DisPri;
                ((IPriorityState)state).AddSpecialPassedOfLabels(config.PriorityState.SpPass);
                ((IPriorityState)state).AddSpecialInterceptOfLabels(config.PriorityState.SpInter);
                ((IPriorityState)state).AddSpecialKeepOfLabels(config.PriorityState.SpKeep);
                ((IPriorityState)state).AddSpecialExclusionOfLabels(config.PriorityState.SpExclu);
                ((IPriorityState)state).AddSpecialRunOfLabels(config.PriorityState.SpRun);
                ((IPriorityState)state).AddSpecialDisableOfLabels(config.PriorityState.SpDis);
                state.allowReenter = config.PriorityState.AllowReenter;
            });

            var stageline = entity.AddComponent<StageLine>();
            stageline.SetEntryNode(new SelectorNode<ITimeLine, IStageNode>());
            foreach (var stageInfo in config.Stages) {
                Stage stage = null;
                foreach (var actionInfo in stageInfo.Actions) {
                    stage ??= new Stage {
                        StageType = (int)stageInfo.StageType,
                        Duration = stageInfo.Duration,
                    };

                    var action = BehaviorNodeFactory.CreateNode<ITimeNode>(actionInfo);
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
                var trait = AbilityTraitFactory.Create(traitAlias);
                trait.SetParent(entity);
            }

            return ability;
        }
    }
}