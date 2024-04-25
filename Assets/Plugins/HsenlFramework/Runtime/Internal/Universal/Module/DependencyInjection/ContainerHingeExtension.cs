using System;

namespace Hsenl {
    // 使用链式调用, 但需要注意前置条件
    public static class ContainerHingeExtension {
        /// <summary>
        /// 注册一个实现类型, 该方法是起点
        /// </summary>
        public static ContainerHingeImplementor Register<T>(this Container self) {
            return new ContainerHingeImplementor { Container = self, ImplementorType = typeof(T) };
        }

        /// <summary>
        /// 把一个实现类型和一个基类关联起来
        /// </summary>
        public static ContainerHinge As<T>(this ContainerHingeImplementor self, Type specifiedType = null) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            var basicType = typeof(T);
            self.Container.RegisterMapping(basicType, self.ImplementorType, specifiedType);
            var hinge = new ContainerHinge() { Container = self.Container, BasicType = basicType, ImplementorType = self.ImplementorType };
            return hinge;
        }

        /// <summary>
        /// 设置自定义的类型实现, 比如接口a, 实现类b, 当需要resolve这条映射的时候, 会用我们提供的实例化方案
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
        /// 允许自动注入, 如果为true, 当Resolve一个类型的时候, 会自动对其内部进行依赖注入, 包括其子类型
        /// </summary>
        public static ContainerHinge AllowAutoInjection(this ContainerHinge self) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            self.Container.AllowAutoInjection(self.ImplementorType);
            return self;
        }
        
        public static ContainerHingeImplementor AllowAutoInjection(this ContainerHingeImplementor self) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            self.Container.AllowAutoInjection(self.ImplementorType);
            return self;
        }

        /// <summary>
        /// 把一个映射关系注册为共享
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