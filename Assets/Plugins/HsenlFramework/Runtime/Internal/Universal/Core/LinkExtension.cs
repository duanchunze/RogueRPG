namespace Hsenl {
    public static class LinkExtension {
        public static void Link<T>(this Object self, T target, int key = 0) where T : Object =>
            EventSystemManager.Instance.Link<T>(self.InstanceId, target.InstanceId, key);

        public static void Unlink(this Object self) => EventSystemManager.Instance.Unlink(self.InstanceId);
        public static void Unlink<T>(this Object self, int key = 0) where T : Object => EventSystemManager.Instance.Unlink<T>(self.InstanceId, key);
        public static T GetLinker<T>(this Object self, int key = 0) where T : Object => EventSystemManager.Instance.GetLinker<T>(self.InstanceId, key);
    }
}