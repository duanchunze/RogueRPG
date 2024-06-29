using System;

namespace Hsenl {
    // 使用链式调用, 但需要注意前置条件
    public static class ContainerHingeExtension {
        /*
         * 下面所有的注释I, I2, I3代表我们声明的类型, 也就是BasicType, a,b,c 则代表具体可以实现的类型, 也就是ImplementorType.
         * class代表注入类, 也就是I所属的类
         */

        /// <summary>
        /// 让一个a怎么样, 这是个开头, 你还需要进一步操作, 才能让链式调用生效
        /// </summary>
        public static ContainerHingeImplementor Let<T>(this Container self) {
            return new ContainerHingeImplementor { Container = self, ImplementorType = typeof(T) };
        }

        /// <summary>
        /// 1、把一个a和一个I关联起来, 当遇到I时, 可以用a实现注入.
        /// 2、specifiedType指定了该关联只在某个指定的类里才被允许, 如果不指定, 则代表在任何类里都被允许.
        ///     你可以注册Ia, Ib..., 都没关系, 注入的时候会进行筛选, 比如Ia在类class里不被允许, 而Ib则被允许, 那就会给I注入b, 如果Ia, Ib都被允许, 那就按顺序来, 注入a.
        ///     你也可以给Ia指定多个类, 代表Ia组合在这些类里都被允许注入.
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
        /// 让某个关联Ia使用我们指定的回调进行实例创建.
        /// 旨在需要实例化的时候, 替换原本默认的实例化方式, 所以他的优先级在共享实例之后.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="instantiationFunc"></param>
        /// <returns></returns>
        public static ContainerHinge SetInstantiationFunc(this ContainerHinge self, Func<object> instantiationFunc) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            if (self.BasicType == null)
                throw new ArgumentNullException(nameof(self.BasicType) + "You need to specify the basicType!");

            self.Container.SetInstantiationFunc(self.BasicType, self.ImplementorType, instantiationFunc);
            return self;
        }

        /// <summary>
        /// 允许一个class可以自动注入, 如何一个class没有注册的话, 那么永远不会对其内部的字段进行注入. 这个方法是我们想实现依赖注入所必须调用的.
        /// </summary>
        public static ContainerHinge AllowAutoInjection(this ContainerHinge self) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            self.Container.AllowAutoInjection(self.ImplementorType);
            return self;
        }

        /// <summary>
        /// 同上
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ContainerHingeImplementor AllowAutoInjection(this ContainerHingeImplementor self) {
            if (self.ImplementorType == null)
                throw new ArgumentNullException(nameof(self.ImplementorType) + "You need to specify the implementorType!");

            self.Container.AllowAutoInjection(self.ImplementorType);
            return self;
        }

        /// <summary>
        /// 把某个BasicType注册为共享.
        /// 比如现在有Ia, I2a, I3a, 三个映射关系, 且I, I2, I3都映射a, 那么, 如果我们把I, I2, I3, 都注册为共享, 那么, 当I2, I3再需要a的时候, 就会直接用
        /// I已经创建好的a, 而不是重新创建一个.
        /// 同时也可以指定在哪些类中, 该共享才起效.
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