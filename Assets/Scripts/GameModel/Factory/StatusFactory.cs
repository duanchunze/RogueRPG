﻿using System;
using Hsenl.actor;
using Hsenl.status;

namespace Hsenl {
    public static class StatusFactory {
        public static Status CreateActorStatus(StatusConfig config) {
            var entity = Entity.Create(config.Alias);
            entity.Tags.Add(config.Tags);

            var status = entity.AddComponent<Status>(initializeInvoke: s => { s.configId = config.Id; });

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

            var timeline = entity.AddComponent<TimeLine>();
            timeline.SetEntryNode(new ParallelNode<ITimeLine, ActionNode<ITimeLine>>());
            timeline.StageTillTime = config.Duration;
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