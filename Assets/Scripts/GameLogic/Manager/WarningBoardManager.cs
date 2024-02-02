using System;

namespace Hsenl {
    [Serializable]
    public class WarningBoardManager : SingletonComponent<WarningBoardManager> {
        public Entity Rent(string wbName, bool autoActive = true) {
            var key = PoolKey.Create("WarningBoard", wbName);
            var wb = Pool.Rent<Entity>(key, active: autoActive);
            if (wb == null) {
                wb = WarningBoardFactory.Create(wbName);
            }

            return wb;
        }

        public void Return(Entity wb) {
            var key = PoolKey.Create("WarningBoard", wb.Name);
            Pool.Return(key, wb);
        }
    }
}