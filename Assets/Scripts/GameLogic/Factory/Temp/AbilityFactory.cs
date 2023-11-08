using System;
using System.Collections.Generic;
using Hsenl.ability;
using Hsenl.behavior;
using Unity.VisualScripting;

namespace Hsenl {
    public static class AbilityFactory {
        public static Ability Create(string alias) {
            var config = Tables.Instance.TbAbilityConfig.GetByAlias(alias);
            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            // 一个技能的组成:
            // 控制触发器, 优先级状态机, 施法器, 阶段线, 技能本体. 根据情况不同, 可以不要控制器, 比如有些靠条件触发的技能, 比如卢安娜的飓风
            var ability = entity.AddComponent<Ability>(initializeInvoke: abi => {
                abi.configId = config.Id;
                abi.factionTypes.Clear();
                foreach (var factionType in config.TargetTags) {
                    abi.factionTypes.Add(factionType);
                }

                abi.manaCost = config.ManaCost;
            });

            entity.AddComponent<Caster>();
            var procedureLineNode = entity.AddComponent<ProcedureLineNode>();
            foreach (var affix in config.Affixes) {
                var worker = ProcedureLineFactory.CreateWorker<PlwInfo>(affix);
                procedureLineNode.AddWorker(worker);
            }

            switch (config.Caster) {
                case cast.ControlCastInfo controlCastInfo: {
                    var controlTrigger = entity.AddComponent<ControlTrigger>();
                    controlTrigger.ControlCode = (int)controlCastInfo.Code;
                    controlTrigger.supportContinue = controlCastInfo.SupportContinue;
                    break;
                }

                case cast.AlwaysCastInfo: {
                    entity.AddComponent<UpdateDriver>();
                    break;
                }

                case cast.ConditionCastOfWorkerInfo conditionCastOfWorkerInfo: {
                    var worker = ProcedureLineFactory.CreateWorker<PlwInfo>(conditionCastOfWorkerInfo);
                    procedureLineNode.AddWorker(worker);
                    break;
                }
            }

            var evaluate = entity.AddComponent<CasterEvaluate>();
            var entryNode = BehaviorNodeFactory.CreateNodeLink<CasterEvaluate>(config.CasterEvaluates);
            evaluate.SetEntryNode(entryNode);

            entity.AddComponent<Numerator>();

            entity.AddComponent<NumericNode>(initializeInvoke: node => {
                node.LinkModel = NumericNodeLinkModel.AutoLinkToSelf;
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

            return ability;
        }
    }
}