using UnityEngine;
using YooAsset;

namespace Hsenl.View {
    public static class WarningBoardFactory {
        public static Entity Create(string boardName, AppearanceMiddleArgStream middleArgStream = null) {
            var prefab = AppearanceSystem.LoadWarningBoard(boardName, middleArgStream);
            var go = UnityEngine.Object.Instantiate(prefab);
            go.name = boardName;
            var entity = Entity.Create(go);
            entity.Active = false;
            return entity;
        }
    }
}