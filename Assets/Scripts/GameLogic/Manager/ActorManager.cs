using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    public class ActorManager : SingletonComponent<ActorManager> {
        private readonly Type _actorType = typeof(Actor);

        public Actor Rent(int configId, Vector3 position, Entity entity = null) {
            var key = PoolKey.Create(this._actorType, configId);
            var actor = Pool.Rent<Actor>(key, active: false) ?? ActorFactory.Create(configId, entity);

            actor.transform.NavMeshAgent.Enable = true;
            actor.transform.Position = position;
            actor.Entity.Active = true;

            Shortcut.RecoverHealth(actor.GetComponent<Numerator>(), int.MaxValue);

            return actor;
        }

        public void Return(Actor actor) {
            var key = PoolKey.Create(this._actorType, actor.configId);
            Pool.Return(key, actor);
        }
    }
}