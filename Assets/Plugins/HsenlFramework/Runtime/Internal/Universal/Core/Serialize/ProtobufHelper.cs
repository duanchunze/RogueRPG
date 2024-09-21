using System;
using System.ComponentModel;
using System.IO;
using ProtoBuf.Meta;

namespace Hsenl {
    public static class ProtobufHelper {
        public static void Init() { }

        static ProtobufHelper() {
            RuntimeTypeModel.Default.Add(typeof(Vector2), false).Add("x", "y");
            RuntimeTypeModel.Default.Add(typeof(Vector3), false).Add("x", "y", "z");
            RuntimeTypeModel.Default.Add(typeof(Vector4), false).Add("x", "y", "z", "w");
            RuntimeTypeModel.Default.Add(typeof(Quaternion), false).Add("x", "y", "z", "w");
        }

        public static object Deserialize(Type type, byte[] bytes, int index, int count) {
            using var stream = new MemoryStream(bytes, index, count);
            var o = ProtoBuf.Serializer.Deserialize(type, stream);
            if (o is ISupportInitialize supportInitialize) {
                supportInitialize.EndInit();
            }

            return o;
        }

        public static byte[] Serialize(object message) {
            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, message);
            return stream.ToArray();
        }

        public static void Serialize(object message, Stream stream) {
            ProtoBuf.Serializer.Serialize(stream, message);
        }

        public static object Deserialize(Type type, Stream stream) {
            var o = ProtoBuf.Serializer.Deserialize(type, stream);
            if (o is ISupportInitialize supportInitialize) {
                supportInitialize.EndInit();
            }

            return o;
        }
    }
}