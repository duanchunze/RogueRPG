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

            var rig = entity.GameObject.AddComponent<Rigidbody>();
            var bodyTrigger = entity.AddComponent<PhysicBody>();
            var agent = entity.GameObject.AddComponent<NavMeshAgent>();
            var audioSource = entity.GameObject.AddComponent<AudioSource>();
            var sound = entity.AddComponent<Sound>();
            var actor = entity.AddComponent<Actor>();
            var pl = entity.AddComponent<ProcedureLine>();
            var tl = entity.AddComponent<TaskLine>();
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
            var appear = entity.AddComponent<Appearance>();
            var headMessage = entity.AddComponent<HeadMessage>();
            var followMessage = entity.AddComponent<FollowMessage>();
            var dropable = entity.AddComponent<Dropable>();

            var abilityBar = Entity.Create("AbilityBar", entity).AddComponent<AbilityBar>();
            var statusBar = Entity.Create("StatusBar", entity).AddComponent<StatusBar>();
            var cardBar = Entity.Create("CardBar", entity).AddComponent<CardBar>();
            var cardBackpack = Entity.Create("CardBackpack", entity).AddComponent<CardBackpack>(initializeInvoke: backpack => {
                backpack.capacity = Tables.Instance.TbGameSingletonConfig.CardBackpackCap;
            });

            followMessage.uiStayTime = 0.75f;

            actor.configId = configId;

            audioSource.playOnAwake = false;
            sound.audioSource = audioSource;

            faction.FactionModel = FactionModel.Enemy;

            rig.isKinematic = true;
            agent.angularSpeed = 0;
            agent.acceleration = float.MaxValue;
            agent.stoppingDistance = 0.1f;
            // if (entity.Tags.Contains(TagType.Hero)) {
            //     agent.avoidancePriority = 10;
            // }
            // agent.autoTraverseOffMeshLink = false;
            // agent.autoRepath = true;

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

            appear.LoadModel(config.ModelName);

            var aiConfig = Tables.Instance.TbAIConfig.GetByAlias(config.AiAlias);
            if (aiConfig != null) {
                var entryNode = BehaviorNodeFactory.CreateNodeLink<BehaviorTree>(aiConfig.Nodes);
                tree.SetEntryNode(entryNode);
            }

            foreach (var abilityAlias in config.OrgAbilitys) {
                var ability = AbilityFactory.Create(abilityAlias);
                abilityBar.EquipAbility(ability);
            }

            foreach (var orgCard in config.OrgCards) {
                var card = CardFactory.Create(orgCard);
                cardBar.PutinCard(card);
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