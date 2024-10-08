using Hsenl.pickable;
using Unity.VisualScripting;
using UnityEngine;

namespace Hsenl {
    public static class PickableFactory {
        public static Pickable Create(string alias, Vector3 position, int count = 1) {
            var config = Tables.Instance.TbPickableConfig.GetByAlias(alias);
            return Create(config, position, count);
        }

        public static Pickable Create(int configId, Vector3 position, int count = 1) {
            var config = Tables.Instance.TbPickableConfig.GetById(configId);
            return Create(config, position, count);
        }

        public static Pickable Create(PickableConfig config, Vector3 position, int count = 1) {
            var entity = Entity.Create(config.Alias + "(Pickable)");
            var pickable = entity.AddComponent<Pickable>(initializeInvoke: pik => {
                pik.configId = config.Id;
                pik.count = count;
            });
            pickable.transform.Position = position;
            pickable.LoadModel(config.ModelName);
            pickable.LoadCollider(0.3f);
            return pickable;
        }
    }
}