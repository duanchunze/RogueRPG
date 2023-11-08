using System;
using System.Collections.Generic;
using Hsenl.behavior;
using Hsenl.game;
using Hsenl.View;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    public static class ActorFactory {
        public static Actor Create(int configId, Entity entity = null) {
            var config = Tables.Instance.TbActorConfig.GetById(configId);
            if (entity == null) {
                entity = Entity.Create(config.Alias);
            }

            entity.Tags.Add(config.Labels);

            var actor = entity.AddComponent<Actor>();
            actor.configId = configId;
            var rig = entity.AddComponent<Rigidbody>();
            rig.IsKinematic = true;
            var col = entity.AddComponent<CapsuleCollider>();
            col.IsTrigger = true;
            col.SetUsage(GameColliderPurpose.Receptor);
            var agent = entity.AddComponent<NavMeshAgent>();
            agent.AngularSpeed = 0;
            agent.Acceleration = float.MaxValue;
            agent.StoppingDistance = 0.1f;
            var sound = entity.AddComponent<Sound>();
            sound.PlayOnAwake = false;
            var pl = entity.AddComponent<ProcedureLine>();
            var control = entity.AddComponent<Control>();
            var priorities = entity.AddComponent<Prioritizer>();
            var tree = entity.AddComponent<BehaviorTree>();
            var motion = entity.AddComponent<Motion>();
            var numerator = entity.AddComponent<Numerator>();
            var selections = entity.AddComponent<Selector>();
            var selectTarget = entity.AddComponent<SelectionTarget>();
            var harmable = entity.AddComponent<Harmable>();
            var hurtable = entity.AddComponent<Hurtable>();
            if (entity.Tags.Contains(TagType.Hero))
                entity.AddComponent<Picker>();
            var faction = entity.AddComponent<Faction>();
            faction.FactionModel = FactionModel.Enemy;
            var appear = entity.AddComponent<Appearance>();
            appear.LoadModel(config.ModelName);
            var headMessage = entity.AddComponent<HeadMessage>();
            var followMessage = entity.AddComponent<FollowMessage>();
            followMessage.uiStayTime = 0.75f;
            var dropable = entity.AddComponent<Dropable>();
            
            var numericConfig = Tables.Instance.TbNumericActorConfig.GetByAlias(config.NumericAlias);
            foreach (var basicValueInfo in numericConfig.NumericInfos) {
                switch (basicValueInfo.Sign) {
                    case "f":
                        numerator.SetValue(basicValueInfo.Type, basicValueInfo.Value);
                        break;
                    default:
                        numerator.SetValue(basicValueInfo.Type, (long)basicValueInfo.Value);
                        break;
                }
            }

            var abilityBar = Entity.Create("AbilityBar", entity).AddComponent<AbilityBar>();
            var statusBar = Entity.Create("StatusBar", entity).AddComponent<StatusBar>();
            if (entity.Tags.Contains(TagType.Hero)) {
                var cardBar = Entity.Create("CardBar", entity).AddComponent<CardBar>();
                var cardBackpack = Entity.Create("CardBackpack", entity).AddComponent<CardBackpack>(initializeInvoke: backpack => {
                    backpack.capacity = Tables.Instance.TbGameSingletonConfig.CardBackpackCap;
                });

                foreach (var orgCard in config.OrgCards) {
                    var card = CardFactory.Create(orgCard);
                    cardBar.PutinCard(card);
                }
            }

            foreach (var abilityAlias in config.OrgAbilitys) {
                var ability = AbilityFactory.Create(abilityAlias);
                abilityBar.EquipAbility(ability);
            }
            
            var aiConfig = Tables.Instance.TbAIConfig.GetByAlias(config.AiAlias);
            if (aiConfig != null) {
                var entryNode = BehaviorNodeFactory.CreateNodeLink<BehaviorTree>(aiConfig.Nodes);
                tree.SetEntryNode(entryNode);
            }

            foreach (var info in config.PossibleDropsByProbability) {
                dropable.AddPossibleDrop(new Dropable.DropByProbability() {
                    id = info.PickableId,
                    probability = info.Probability,
                    count = info.CountEach,
                    stackable = info.Satckable,
                });
            }

            foreach (var info in config.PossibleDropsByWeight) {
                dropable.AddPossibleDrop(new Dropable.DropByWeight() {
                    id = info.PickableId,
                    weight = info.Weight,
                    count = info.CountEach,
                    stackable = info.Satckable,
                });
            }

            dropable.SetDropCount(config.PossibleDropNumber.Min, config.PossibleDropNumber.Max, config.PossibleDropNumber.Weights);

            return actor;
        }
    }
}