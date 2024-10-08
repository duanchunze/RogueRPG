using UnityEngine.AI;

namespace Hsenl {
    [ShadowFunction]
    public partial class ActorFactory {
        [ShadowFunction]
        public static Actor Create(int configId, Entity entity = null) {
            var config = Tables.Instance.TbActorConfig.GetById(configId);
            if (entity == null) {
                entity = Entity.Create(config.Alias);
            }

            entity.Tags.Add(config.Labels);

            var actor = entity.AddComponent<Actor>();
            actor.SetConfigId(configId);
            var rig = entity.AddComponent<Rigidbody>();
            rig.IsKinematic = true;
            var col = entity.AddComponent<CapsuleCollider>();
            col.IsTrigger = true;
            col.SetUsage(GameColliderPurpose.Receptor);
            var agent = entity.AddComponent<NavMeshAgent>();
            agent.AngularSpeed = 0;
            agent.StoppingDistance = 0.1f;
            if (entity.Tags.Contains(TagType.Hero)) {
                agent.Acceleration = float.MaxValue;
                agent.ObstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }

            if (entity.Tags.Contains(TagType.Monster)) {
                agent.AvoidancePriority = 0;
            }

            var pl = entity.AddComponent<ProcedureLine>();
            var control = entity.AddComponent<Control>();
            var priorities = entity.AddComponent<Prioritizer>();
            var tree = entity.AddComponent<BehaviorTree>();
            var numerator = entity.AddComponent<Numerator>();
            var selections = entity.AddComponent<Selector>();
            var selectTarget = entity.AddComponent<SelectionTarget>();
            var harmable = entity.AddComponent<Harmable>();
            var hurtable = entity.AddComponent<Hurtable>();
            if (entity.Tags.Contains(TagType.Hero))
                entity.AddComponent<Picker>();
            var faction = entity.AddComponent<Faction>();
            faction.FactionModel = FactionModel.Enemy;
            var dropable = entity.AddComponent<Dropable>();
            CreateShadow(configId, entity);

            var numericConfig = config.NumericActorConfig;
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

            var abilityBar = Entity.Create("AbilityBar", entity).AddComponent<AbilitesBar>();
            abilityBar.ExplicitAbilityCapacity = 6;
            foreach (var abilityAlias in config.OrgAbilitys) {
                var ability = AbilityFactory.Create(abilityAlias);
                abilityBar.EquipAbility(ability);
            }

            Entity.Create("StatusBar", entity).AddComponent<StatusBar>();
            Entity.Create("DrawCard", entity).AddComponent<DrawCard>();
            entity.AddComponent<MinionsBar>();
            // if (entity.Tags.Contains(TagType.Hero)) {
            //     var cardBar = Entity.Create("CardBar", entity).AddComponent<CardBar>();
            //     var cardBackpack = Entity.Create("CardBackpack", entity).AddComponent<CardBackpack>(initializeInvoke: backpack => {
            //         backpack.capacity = Tables.Instance.TbGameSingletonConfig.CardBackpackCap;
            //     });
            //
            //     foreach (var orgCard in config.OrgCards) {
            //         var card = CardFactory.Create(orgCard);
            //         cardBar.PutinCard(card);
            //     }
            // }

            var aiConfig = config.AIConfig;
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

        public static void Reset(Actor actor) {
            var config = actor.Config;
            var numerator = actor.GetComponent<Numerator>();
            numerator.ClearRaw();
            var numericConfig = config.NumericActorConfig;
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

            var navMeshAgent = actor.GetComponent<NavMeshAgent>();
            if (navMeshAgent != null) {
                navMeshAgent.Enable = true;
            }

            var prioritizer = actor.GetComponent<Prioritizer>();
            var state = prioritizer.GetState(StatusAlias.Death);
            if (state != null) {
                state.LeaveState();
            }

            numerator.Recalculate();
        }
    }
}