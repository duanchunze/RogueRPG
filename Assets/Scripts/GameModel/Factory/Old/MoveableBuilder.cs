using System;
using UnityEngine.AI;

namespace Hsenl {
    public class MoveableBuilder : FactoryBuilder {
        public MoveableBuilder() {
            
        }
        
        public override void Build(Entity entity) {
            var go = entity.GameObject;
            if (go == null) {
                // 移动依赖 unity
                return;
            }

            // var agent = go.GetOrAddComponent<NavMeshAgent>();
            // agent.angularSpeed = 0;
            // agent.acceleration = float.MaxValue;
            // agent.stoppingDistance = 0.1f;
        }
    }
}