using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    [Serializable]
    public class ActorManager : SingletonComponent<ActorManager> {
        private readonly Type _actorType = typeof(Actor);
        
        public Actor Rent(int configId, Vector3 position, Entity entity = null) {
            var key = PoolKey.Create(this._actorType, configId);
            if (PoolManager.Instance.Rent(key, autoActive:false) is not Actor actor) {
                actor = ActorFactory.Create(configId, entity);
            }
            else {
                actor.GetMonoComponent<NavMeshAgent>().enabled = true;
            }

            actor.transform.Position = position;
            actor.Entity.Active = true;
            
            Shortcut.RecoverHealth(actor.GetComponent<Numerator>(), int.MaxValue);
            
            return actor;
        }

        public void Return(Actor actor) {
            var key = PoolKey.Create(typeof(Actor), actor.configId);
            PoolManager.Instance.Return(key, actor);
        }
    }
}