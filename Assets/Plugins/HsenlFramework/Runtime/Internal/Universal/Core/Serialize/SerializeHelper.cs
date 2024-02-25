using System.IO;
using System;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.Serialization;
#endif

namespace Hsenl {
    /*
     * 各种数据处理速度对比: n种序列化、反射、手动
     * 500次, 平均值
     * protobuf - 22ms
     * mongo - 161ms
     * odin - 20ms
     * new一个新类的速度参考 - 1.9ms
     * 手动赋值 - 0ms (太小了, 可以忽略掉)
     * 微软自带的BinaryFormatter序列化 - 330ms
     * 反射 - 36ms
     * unity Instantiate - 8.9ms (unity的测试时, 不仅要序列化, 背后大概还有一系列的操作, 哪怕这种情况下, 依然秒杀 odin 等序列化)
     * 
     * 总结
     * 除了自己手动赋值以外, 最快的就是unity的序列化, 而且是非常快
     * odin 和 protobuf差不多, odin 稍微快一点
     * 微软自带的最慢, 其次是mongo, 再其次是反射
     * json就更不用测了, 肯定也是慢的离谱
     */
    public static class SerializeHelper {
        // proto buf
        public static byte[] Serialize(object message) {
            return ProtobufHelper.Serialize(message);
        }

        public static void Serialize(object message, Stream stream) {
            ProtobufHelper.Serialize(message, stream);
        }

        public static object Deserialize(Type type, byte[] bytes, int index, int count) {
            return ProtobufHelper.Deserialize(type, bytes, index, count);
        }

        public static object Deserialize(Type type, Stream stream) {
            return ProtobufHelper.Deserialize(type, stream);
        }

        // odin - 实际测试下来, odin的速度要比 protobuf dotnet 版的速度要快一点点
        // odin在反序列化时, 不会变量的默认值赋值, 比如 private int a = 1; 反序列化后的新实例, a 依然是默认值 0, 这点很操蛋
#if UNITY_EDITOR
        public static byte[] SerializeOfOdin(object message) {
            return SerializationUtility.SerializeValue(message, DataFormat.Binary);
        }

        public static void SerializeOfOdin(object message, Stream stream) {
            SerializationUtility.SerializeValue(message, stream, DataFormat.Binary);
        }

        public static T DeserializeOfOdin<T>(byte[] bytes) {
            return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
        }

        public static T DeserializeOfOdin<T>(Stream stream) {
            return SerializationUtility.DeserializeValue<T>(stream, DataFormat.Binary);
        }
#endif

        // 这个采用了源码生成技术, 速度比odin还要快, 在预热之后(第一次执行之后), 更是快出8倍左右. 但使用起来比较麻烦
        public static byte[] SerializeOfMemoryPack(in Object message) {
            return MemoryPackSerializer.Serialize(in message);
        }

        public static byte[] SerializeOfMemoryPack<T>(T message) {
            return MemoryPackSerializer.Serialize(message);
        }
        
        public static T DeserializeOfMemoryPack<T>(byte[] bytes) {
            return MemoryPackSerializer.Deserialize<T>(bytes);
        }

        /* 关于MemoryPack
         *
         * 序列化范围: 默认序列化所有共有的字段或属性, 默认不序列化私有、内部、保护类型的字段或属性. 在此基础上, 使用[MemoryPackInclude] [MemoryPackIgnore] 来微操
         *      // 以下默认序列化
         *      public int PublicField;
         *      public readonly int PublicReadOnlyField;
         *      public int PublicProperty { get; set; }
         *      public int PrivateSetPublicProperty { get; private set; }
         *      public int ReadOnlyPublicProperty { get; }
         *      public int InitProperty { get; init; }
         *      public required int RequiredInitProperty { get; init; }
         *      
         *      // 以下默认不序列化
         *      int privateProperty { get; set; }
         *      int privateField;
         *      readonly int privateReadOnlyField;
         *      internal int internalField
         * 
         * 如果成员不是默认支持的类型, 则需要给目标类型也打上[MemoryPackable], 如果不方便添加特性, 可以给成员变量加上[MemoryPackAllowSerialize]以静默报错诊断
         * 
         * 成员顺序很重要，MemoryPack 不会序列化成员名称或其他信息，而是按照声明的顺序序列化字段。如果类型是继承的，则按照父→子的顺序进行序列化。反序列化时成员的顺序不能改变
         * 
         * MemoryPack可以通过构造函数来进行序列化, 所以在有多个构造函数的时候, 需要使用[MemoryPackConstructor]来特别指定使用那个构造函数
         *
         * 事件回调
         *
         * 如果我们的类是一个集合类型的类, 比如 public partial class MyList<T> : List<T>, 我们也需要[MemoryPackable(GenerateType.Collection)]来特别指定, 因为默认使用的GenerateType.Object, 用在一般类上
         *  
         * interface 和 abstract 可以使用[MemoryPackUnion]来支持多态序列化
         * 
         * 
         * 
         * 
         * 
         */
    }
}