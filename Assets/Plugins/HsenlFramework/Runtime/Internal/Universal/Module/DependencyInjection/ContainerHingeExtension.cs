using System;

namespace Hsenl {
    // 使用链式调用, 但需要注意前置条件
    public static class ContainerHingeExtension {
        /// <summary>
        /// 该函数总是第一步调用
        /// </summary>
        public static ContainerHinge Register<T>(this Container self) {
            return new ContainerHinge() { Container = self, ImplementorType = typeof(T) };
        }

        /// <summary>
        /// 需要调用过Register
        /// </summary>
        public static ContainerHinge As<T>(this ContainerHinge self, Type specifiedType = null) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            var basicType = typeof(T);
            self.Container.RegisterMapping(basicType, self.ImplementorType, specifiedType);
            self.BasicType = basicType;
            return self;
        }

        /// <summary>
        /// 需要调用过As, 设置自定义的类型实现, 比如接口a, 实现类b, 当需要resolve这条映射的时候, 会用我们提供的实例化方案
        /// </summary>
        /// <param name="self"></param>
        /// <param name="instantiationFunc"></param>
        /// <returns></returns>
        public static ContainerHinge SetInstantiationFunc(this ContainerHinge self, Func<(Type basicType, string memberName), object> instantiationFunc) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            if (self.BasicType == null)
                throw new ArgumentNullException(nameof(self.BasicType) + "You need to specify the basicType!");

            self.Container.SetInstantiationFunc(self.BasicType, self.ImplementorType, instantiationFunc);
            return self;
        }

        /// <summary>
        /// 需要调用过Register
        /// </summary>
        public static ContainerHinge AllowAutoInjection(this ContainerHinge self) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            self.Container.AllowAutoInjection(self.ImplementorType);
            return self;
        }

        /// <summary>
        /// 需要调用过As
        /// </summary>
        public static ContainerHinge ShareInjection(this ContainerHinge self, Type specifiedShareType = null,
            ShareInjectionModel shareInjectionModel = ShareInjectionModel.OnlyInstance) {
            if (self.BasicType == null)
                throw new ArgumentNullException(nameof(self.BasicType) + "You need to specify the basicType!");

            self.Container.RegisterShareInjection(self.BasicType, specifiedShareType, shareInjectionModel);
            return self;
        }
    }
}