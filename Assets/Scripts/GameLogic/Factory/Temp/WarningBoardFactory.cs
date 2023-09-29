using UnityEngine;

namespace Hsenl {
    public static class WarningBoardFactory {
        public static Entity Create(string boardName) {
            var prefab = ResourcesHelper.GetAsset<GameObject>(Constant.WarningBoardBundleName, boardName);
            var go = UnityEngine.Object.Instantiate(prefab);
            go.name = boardName;
            var entity = Entity.Create(go);
            entity.Active = false;
            return entity;
        }
    }
}