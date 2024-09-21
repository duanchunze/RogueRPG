using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    public class ActorManager : SingletonComponent<ActorManager> {
        private readonly Type _actorType = typeof(Actor);

        public Actor Rent(string alias, Vector3 position = default, Entity entity = null) {
            var config = Tables.Instance.TbActorConfig.GetByAlias(alias);
            if (config == null) {
                Log.Error($"Actor '{alias} config is not find'");
                return null;
            }

            var actor = this.Rent(config.Id, position, entity);
            return actor;
        }

        public Actor Rent(int configId, Vector3 position = default, Entity entity = null) {
            var key = PoolKey.Create(this._actorType, configId);
            var actor = Pool.Rent<Actor>(key, active: false);
            if (actor == null) {
                actor = ActorFactory.Create(configId, entity);
            }
            else {
                ActorFactory.Reset(actor);
            }

            ((IPoolable)actor).SetPoolKey(key);

            actor.transform.Position = position;
            actor.Entity.Active = true;

            return actor;
        }

        public void Return(Actor actor) {
            Pool.Return(actor);
        }
    }
}