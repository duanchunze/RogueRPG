using System;

namespace Hsenl {
    public static class HsenlHelper {
        // 规范子物体
        public static void MakeSureChildrenCount(this Entity self, int count, Func<Entity> instantiateCallback) {
            var childCount = self.ChildCount;
            if (childCount > count) {
                for (var i = 0; i < childCount; i++) {
                    self.GetChild(i).Active = i < count;
                }
            }
            else if (childCount < count) {
                for (var i = 0; i < childCount; i++) {
                    self.GetChild(i).Active = true;
                }

                for (var i = childCount; i < count; i++) {
                    var entity = instantiateCallback.Invoke();
                    entity.SetParent(self);
                    entity.Active = true;
                }
            }
            else {
                for (var i = 0; i < childCount; i++) {
                    self.GetChild(i).Active = true;
                }
            }
        }
        
        public static void MakeSureChildrenCount(this Component self, int count, Func<Entity> instantiateCallback) {
            self.Entity.MakeSureChildrenCount(count, instantiateCallback);
        }
    }
}