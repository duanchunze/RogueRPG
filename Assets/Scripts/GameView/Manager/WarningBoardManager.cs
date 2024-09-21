using System;

namespace Hsenl.View {
    [Serializable]
    public class WarningBoardManager : SingletonComponent<WarningBoardManager> {
        public Entity Rent(string wbName, bool active = true, AppearanceMiddleArgStream middleArgStream = null) {
            var hashcode = wbName.GetHashCode();
            if (middleArgStream != null) {
                hashcode = HashCode.Combine(hashcode, middleArgStream.CalcHashCode());
            }

            var key = PoolKey.Create("WarningBoard", hashcode);
            if (Pool.Rent(key, active: active) is not Entity wb) {
                wb = WarningBoardFactory.Create(wbName, middleArgStream);
                wb.Name = key.key.ToString();
            }

            return wb;
        }

        public void Return(Entity wb) {
            if (wb.IsDisposed)
                return;

            PoolKey key = new("WarningBoard", int.Parse(wb.Name));
            Pool.Return(key, wb);
        }
    }
}