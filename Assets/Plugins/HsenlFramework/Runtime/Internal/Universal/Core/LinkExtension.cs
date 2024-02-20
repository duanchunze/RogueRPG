namespace Hsenl {
    // todo 1、不合适, 考虑删掉, 找替代方案. 该方案是为了解决某些我们不想明文写出来但实际运行过程中又需要的引用关系,
    // todo 2、比如, abi可以被包裹在card里, 而card里可以声明一个obj, 用来保存当前包裹的是谁, 但我却不想在abi里也声明一个变量用来保存是谁包裹了abi. 因为card包裹东西是他的核心功能,
    // todo 3、声明个变量很正常, 但abi被包裹可不是他的核心功能, 声明个变量就怪怪的
    public static class LinkExtension {
        public static void Link<T>(this Object self, T target, int key = 0) where T : Object =>
            EventSystemManager.Instance.Link<T>(self.InstanceId, target.InstanceId, key);

        public static void Unlink(this Object self) => EventSystemManager.Instance.Unlink(self.InstanceId);
        public static void Unlink<T>(this Object self, int key = 0) where T : Object => EventSystemManager.Instance.Unlink<T>(self.InstanceId, key);
        public static T GetLinker<T>(this Object self, int key = 0) where T : Object => EventSystemManager.Instance.GetLinker<T>(self.InstanceId, key);
    }
}