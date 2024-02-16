using MemoryPack;
using MemoryPack.Formatters;
using UnityEngine;

namespace Hsenl {
    [FrameworkMember]
    public static class UnityProviderInitializer {
        [OnEventSystemInitialized]
        private static void RegisterInitialFormatters() {
            // struct
            UnityRegister<UnityEngine.Vector2>();
            UnityRegister<UnityEngine.Vector3>();
            UnityRegister<UnityEngine.Vector4>();
            UnityRegister<UnityEngine.Quaternion>();
            UnityRegister<UnityEngine.Color>();
            UnityRegister<UnityEngine.Bounds>();
            UnityRegister<UnityEngine.Rect>();
            UnityRegister<UnityEngine.Keyframe>();
            MemoryPack.MemoryPackFormatterProvider.Register(new UnmanagedFormatter<UnityEngine.WrapMode>());
            UnityRegister<UnityEngine.Matrix4x4>();
            UnityRegister<UnityEngine.GradientColorKey>();
            UnityRegister<UnityEngine.GradientAlphaKey>();
            MemoryPack.MemoryPackFormatterProvider.Register(new UnmanagedFormatter<UnityEngine.GradientMode>());
            UnityRegister<UnityEngine.Color32>();
            UnityRegister<UnityEngine.LayerMask>();
            UnityRegister<UnityEngine.Vector2Int>();
            UnityRegister<UnityEngine.Vector3Int>();
            UnityRegister<UnityEngine.RangeInt>();
            UnityRegister<UnityEngine.RectInt>();
            UnityRegister<UnityEngine.BoundsInt>();

            // class
            if (!MemoryPack.MemoryPackFormatterProvider.IsRegistered<AnimationCurve>()) {
                MemoryPack.MemoryPackFormatterProvider.Register(new AnimationCurveFormatter());
                MemoryPack.MemoryPackFormatterProvider.Register(new ArrayFormatter<AnimationCurve>());
                MemoryPack.MemoryPackFormatterProvider.Register(new ListFormatter<AnimationCurve>());
            }

            if (!MemoryPack.MemoryPackFormatterProvider.IsRegistered<Gradient>()) {
                MemoryPack.MemoryPackFormatterProvider.Register(new GradientFormatter());
                MemoryPack.MemoryPackFormatterProvider.Register(new ArrayFormatter<Gradient>());
                MemoryPack.MemoryPackFormatterProvider.Register(new ListFormatter<Gradient>());
            }

            if (!MemoryPack.MemoryPackFormatterProvider.IsRegistered<RectOffset>()) {
                MemoryPack.MemoryPackFormatterProvider.Register(new RectOffsetFormatter());
                MemoryPack.MemoryPackFormatterProvider.Register(new ArrayFormatter<RectOffset>());
                MemoryPack.MemoryPackFormatterProvider.Register(new ListFormatter<RectOffset>());
            }
        }

        static void UnityRegister<T>()
            where T : unmanaged {
            MemoryPack.MemoryPackFormatterProvider.Register(new UnmanagedFormatter<T>());
            MemoryPack.MemoryPackFormatterProvider.Register(new UnmanagedArrayFormatter<T>());
            MemoryPack.MemoryPackFormatterProvider.Register(new ListFormatter<T>());
            MemoryPack.MemoryPackFormatterProvider.Register(new NullableFormatter<T>());
        }
    }
}