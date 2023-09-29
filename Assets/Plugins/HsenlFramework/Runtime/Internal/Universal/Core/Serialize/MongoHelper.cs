// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Reflection;
// using MongoDB.Bson;
// using MongoDB.Bson.IO;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Conventions;
// using MongoDB.Bson.Serialization.Serializers;
// using Unity.Mathematics;
//
// namespace Hsenl {
//     /* 注册泛型类示例
//      * MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<SerializeFlag<Unit>>(cm =>
//      * {
//      * 	cm.AutoMap();
//      * 	cm.SetDiscriminator("SerializeFlag`ET.Unit");
//      * });
//      * MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<SerializeFlag<Skill>>(cm =>
//      * {
//      * 	cm.AutoMap();
//      * 	cm.SetDiscriminator("SerializeFlag`ET.Skill");
//      * });
//      * 
//      * 字典字段需要添加特殊属性
//      * [MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
//      * 
//      */
//
//     public static class MongoHelper {
//         private class StructBsonSerialize<TValue> : StructSerializerBase<TValue> where TValue : struct {
//             public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value) {
//                 var nominalType = args.NominalType;
//
//                 var bsonWriter = context.Writer;
//
//                 bsonWriter.WriteStartDocument();
//
//                 var fields = nominalType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//                 foreach (var field in fields) {
//                     bsonWriter.WriteName(field.Name);
//                     BsonSerializer.Serialize(bsonWriter, field.FieldType, field.GetValue(value));
//                 }
//
//                 bsonWriter.WriteEndDocument();
//             }
//
//             public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
//                 //boxing is required for SetValue to work
//                 object obj = new TValue();
//                 var actualType = args.NominalType;
//                 var bsonReader = context.Reader;
//
//                 bsonReader.ReadStartDocument();
//
//                 while (bsonReader.State != BsonReaderState.EndOfDocument) {
//                     switch (bsonReader.State) {
//                         case BsonReaderState.Name: {
//                             var name = bsonReader.ReadName(Utf8NameDecoder.Instance);
//                             var field = actualType.GetField(name);
//                             if (field != null) {
//                                 var value = BsonSerializer.Deserialize(bsonReader, field.FieldType);
//                                 field.SetValue(obj, value);
//                             }
//
//                             break;
//                         }
//                         case BsonReaderState.Type: {
//                             bsonReader.ReadBsonType();
//                             break;
//                         }
//                         case BsonReaderState.Value: {
//                             bsonReader.SkipValue();
//                             break;
//                         }
//                     }
//                 }
//
//                 bsonReader.ReadEndDocument();
//
//                 return (TValue)obj;
//             }
//         }
//
//         private static readonly JsonWriterSettings _defaultSettings = new() { OutputMode = JsonOutputMode.RelaxedExtendedJson };
//
//         static MongoHelper() {
//             // 自动注册IgnoreExtraElements
//
//             var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
//
//             ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
//
//             RegisterStruct<float2>();
//             RegisterStruct<float3>();
//             RegisterStruct<float4>();
//             RegisterStruct<quaternion>();
//
//             var types = EventSystem.GetAllTypes();
//             foreach (var type in types) {
//                 if (!type.IsSubclassOf(typeof(Object))) {
//                     continue;
//                 }
//
//                 if (type.IsGenericType) {
//                     continue;
//                 }
//
//                 BsonClassMap.LookupClassMap(type);
//             }
//         }
//
//         public static void Init() { }
//
//         public static void RegisterStruct<T>() where T : struct {
//             BsonSerializer.RegisterSerializer(typeof(T), new StructBsonSerialize<T>());
//         }
//
//         public static string ToJson(object obj) {
//             return obj.ToJson(_defaultSettings);
//         }
//
//         public static string ToJson(object obj, JsonWriterSettings settings) {
//             return obj.ToJson(settings);
//         }
//
//         public static T FromJson<T>(string str) {
//             try {
//                 return BsonSerializer.Deserialize<T>(str);
//             }
//             catch (Exception e) {
//                 throw new Exception($"{str}\n{e}");
//             }
//         }
//
//         public static object FromJson(Type type, string str) {
//             return BsonSerializer.Deserialize(str, type);
//         }
//
//         public static byte[] Serialize(object obj) {
//             return obj.ToBson();
//         }
//
//         public static void Serialize(object message, MemoryStream stream) {
//             using (var bsonWriter = new BsonBinaryWriter(stream, BsonBinaryWriterSettings.Defaults)) {
//                 var context = BsonSerializationContext.CreateRoot(bsonWriter);
//                 BsonSerializationArgs args = default;
//                 args.NominalType = typeof(object);
//                 var serializer = BsonSerializer.LookupSerializer(args.NominalType);
//                 serializer.Serialize(context, args, message);
//             }
//         }
//
//         public static object Deserialize(Type type, byte[] bytes) {
//             try {
//                 return BsonSerializer.Deserialize(bytes, type);
//             }
//             catch (Exception e) {
//                 throw new Exception($"from bson error: {type.Name}", e);
//             }
//         }
//
//         public static object Deserialize(Type type, byte[] bytes, int index, int count) {
//             try {
//                 using (var memoryStream = new MemoryStream(bytes, index, count)) {
//                     return BsonSerializer.Deserialize(memoryStream, type);
//                 }
//             }
//             catch (Exception e) {
//                 throw new Exception($"from bson error: {type.Name}", e);
//             }
//         }
//
//         public static object Deserialize(Type type, Stream stream) {
//             try {
//                 return BsonSerializer.Deserialize(stream, type);
//             }
//             catch (Exception e) {
//                 throw new Exception($"from bson error: {type.Name}", e);
//             }
//         }
//
//         public static T Deserialize<T>(byte[] bytes) {
//             try {
//                 using (var memoryStream = new MemoryStream(bytes)) {
//                     return (T)BsonSerializer.Deserialize(memoryStream, typeof(T));
//                 }
//             }
//             catch (Exception e) {
//                 throw new Exception($"from bson error: {typeof(T).Name}", e);
//             }
//         }
//
//         public static T Deserialize<T>(byte[] bytes, int index, int count) {
//             return (T)Deserialize(typeof(T), bytes, index, count);
//         }
//
//         public static T Clone<T>(T t) {
//             return Deserialize<T>(Serialize(t));
//         }
//     }
// }