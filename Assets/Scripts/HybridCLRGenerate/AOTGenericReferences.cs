using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
		"MemoryPack.Core.dll",
		"Sirenix.Serialization.dll",
		"System.Core.dll",
		"System.Runtime.CompilerServices.Unsafe.dll",
		"System.dll",
		"Unity.InputSystem.dll",
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.CoreModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector3>
	// DG.Tweening.Core.DOSetter<UnityEngine.Vector3>
	// MemoryPack.Formatters.ArrayFormatter<Hsenl.C2R_Login>
	// MemoryPack.Formatters.ArrayFormatter<Hsenl.R2C_Login>
	// MemoryPack.Formatters.ArrayFormatter<int>
	// MemoryPack.Formatters.ArrayFormatter<object>
	// MemoryPack.Formatters.ArrayFormatter<ulong>
	// MemoryPack.Formatters.DictionaryFormatter<uint,Hsenl.Num>
	// MemoryPack.Formatters.GenericDictionaryFormatter<object,object,object>
	// MemoryPack.Formatters.GenericDictionaryFormatterBase<object,object,object>
	// MemoryPack.Formatters.InterfaceReadOnlyListFormatter<object>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Bounds>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.BoundsInt>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Color32>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Color>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.GradientAlphaKey>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.GradientColorKey>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Keyframe>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.LayerMask>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Matrix4x4>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Quaternion>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.RangeInt>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Rect>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.RectInt>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Vector2>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Vector2Int>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Vector3>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Vector3Int>
	// MemoryPack.Formatters.ListFormatter<UnityEngine.Vector4>
	// MemoryPack.Formatters.ListFormatter<int>
	// MemoryPack.Formatters.ListFormatter<object>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Bounds>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.BoundsInt>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Color32>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Color>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.GradientAlphaKey>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.GradientColorKey>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Keyframe>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.LayerMask>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Matrix4x4>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Quaternion>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.RangeInt>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Rect>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.RectInt>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Vector2>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Vector2Int>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Vector3>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Vector3Int>
	// MemoryPack.Formatters.NullableFormatter<UnityEngine.Vector4>
	// MemoryPack.Formatters.NullableFormatter<object>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Bounds>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.BoundsInt>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Color32>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Color>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.GradientAlphaKey>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.GradientColorKey>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Keyframe>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.LayerMask>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Matrix4x4>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Quaternion>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.RangeInt>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Rect>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.RectInt>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Vector2>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Vector2Int>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Vector3>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Vector3Int>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<UnityEngine.Vector4>
	// MemoryPack.Formatters.UnmanagedArrayFormatter<object>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Bounds>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.BoundsInt>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Color32>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Color>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.GradientAlphaKey>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.GradientColorKey>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Keyframe>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.LayerMask>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Matrix4x4>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Quaternion>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.RangeInt>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Rect>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.RectInt>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Vector2>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Vector2Int>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Vector3>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Vector3Int>
	// MemoryPack.Formatters.UnmanagedFormatter<UnityEngine.Vector4>
	// MemoryPack.Formatters.UnmanagedFormatter<int>
	// MemoryPack.Formatters.UnmanagedFormatter<object>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Bounds>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.BoundsInt>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Color32>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Color>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.GradientAlphaKey>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.GradientColorKey>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Keyframe>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.LayerMask>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Matrix4x4>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Quaternion>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.RangeInt>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Rect>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.RectInt>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Vector2>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Vector2Int>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Vector3>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Vector3Int>
	// MemoryPack.IMemoryPackFormatter<UnityEngine.Vector4>
	// MemoryPack.IMemoryPackFormatter<int>
	// MemoryPack.IMemoryPackFormatter<object>
	// MemoryPack.IMemoryPackable<Hsenl.C2R_Login>
	// MemoryPack.IMemoryPackable<Hsenl.R2C_Login>
	// MemoryPack.IMemoryPackable<object>
	// MemoryPack.MemoryPackFormatter<Hsenl.C2R_Login>
	// MemoryPack.MemoryPackFormatter<Hsenl.R2C_Login>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Bounds>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.BoundsInt>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Color32>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Color>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.GradientAlphaKey>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.GradientColorKey>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Keyframe>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.LayerMask>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Matrix4x4>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Quaternion>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.RangeInt>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Rect>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.RectInt>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector2>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector2Int>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector3>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector3Int>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector4>>
	// MemoryPack.MemoryPackFormatter<System.Nullable<object>>
	// MemoryPack.MemoryPackFormatter<System.UIntPtr>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Bounds>
	// MemoryPack.MemoryPackFormatter<UnityEngine.BoundsInt>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Color32>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Color>
	// MemoryPack.MemoryPackFormatter<UnityEngine.GradientAlphaKey>
	// MemoryPack.MemoryPackFormatter<UnityEngine.GradientColorKey>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Keyframe>
	// MemoryPack.MemoryPackFormatter<UnityEngine.LayerMask>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Matrix4x4>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Quaternion>
	// MemoryPack.MemoryPackFormatter<UnityEngine.RangeInt>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Rect>
	// MemoryPack.MemoryPackFormatter<UnityEngine.RectInt>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Vector2>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Vector2Int>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Vector3>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Vector3Int>
	// MemoryPack.MemoryPackFormatter<UnityEngine.Vector4>
	// MemoryPack.MemoryPackFormatter<int>
	// MemoryPack.MemoryPackFormatter<object>
	// Sirenix.Serialization.Serializer<object>
	// System.Action<Cysharp.Text.Utf16FormatSegment>
	// System.Action<Cysharp.Text.Utf8FormatSegment>
	// System.Action<Hsenl.CasterEndDetails>
	// System.Action<Hsenl.Container.MappingInfo>
	// System.Action<Hsenl.FixCollisionInfo>
	// System.Action<Hsenl.HTask<int>>
	// System.Action<Hsenl.HTask>
	// System.Action<Hsenl.Network.AckItem>
	// System.Action<Hsenl.Network.SegmentStruct>
	// System.Action<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Action<Hsenl.StatusFinishDetails>
	// System.Action<Hsenl.Vector3>
	// System.Action<MemoryPack.Internal.BufferSegment>
	// System.Action<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Action<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Action<System.Memory<byte>>
	// System.Action<System.ValueTuple<int,object>>
	// System.Action<System.ValueTuple<object,object>>
	// System.Action<System.ValueTuple<ushort,object>>
	// System.Action<UnityEngine.Bounds>
	// System.Action<UnityEngine.BoundsInt>
	// System.Action<UnityEngine.Color32>
	// System.Action<UnityEngine.Color>
	// System.Action<UnityEngine.EventSystems.RaycastResult>
	// System.Action<UnityEngine.GradientAlphaKey>
	// System.Action<UnityEngine.GradientColorKey>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<UnityEngine.Keyframe>
	// System.Action<UnityEngine.LayerMask>
	// System.Action<UnityEngine.Matrix4x4>
	// System.Action<UnityEngine.Quaternion>
	// System.Action<UnityEngine.RangeInt>
	// System.Action<UnityEngine.Rect>
	// System.Action<UnityEngine.RectInt>
	// System.Action<UnityEngine.Vector2>
	// System.Action<UnityEngine.Vector2Int>
	// System.Action<UnityEngine.Vector3>
	// System.Action<UnityEngine.Vector3Int>
	// System.Action<UnityEngine.Vector4>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int,float,float>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<long,System.Memory<byte>>
	// System.Action<long,int>
	// System.Action<long>
	// System.Action<object,Hsenl.PriorityStateLeaveDetails>
	// System.Action<object,System.Memory<byte>>
	// System.Action<object,float,float>
	// System.Action<object,float>
	// System.Action<object,int,Hsenl.Num,Hsenl.Num>
	// System.Action<object,int,System.ValueTuple<object,object>>
	// System.Action<object,int,object>
	// System.Action<object,int>
	// System.Action<object,object,object>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<uint>
	// System.Action<ushort,object>
	// System.Action<ushort,ushort>
	// System.Action<ushort>
	// System.ArraySegment.Enumerator<Hsenl.MergeSortFloatWrap<object>>
	// System.ArraySegment.Enumerator<Hsenl.Vector3>
	// System.ArraySegment.Enumerator<UnityEngine.GradientAlphaKey>
	// System.ArraySegment.Enumerator<UnityEngine.GradientColorKey>
	// System.ArraySegment.Enumerator<UnityEngine.Keyframe>
	// System.ArraySegment.Enumerator<UnityEngine.jvalue>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment.Enumerator<int>
	// System.ArraySegment.Enumerator<object>
	// System.ArraySegment.Enumerator<ulong>
	// System.ArraySegment.Enumerator<ushort>
	// System.ArraySegment<Hsenl.MergeSortFloatWrap<object>>
	// System.ArraySegment<Hsenl.Vector3>
	// System.ArraySegment<UnityEngine.GradientAlphaKey>
	// System.ArraySegment<UnityEngine.GradientColorKey>
	// System.ArraySegment<UnityEngine.Keyframe>
	// System.ArraySegment<UnityEngine.jvalue>
	// System.ArraySegment<byte>
	// System.ArraySegment<int>
	// System.ArraySegment<object>
	// System.ArraySegment<ulong>
	// System.ArraySegment<ushort>
	// System.Buffers.ArrayPool<Hsenl.MergeSortFloatWrap<object>>
	// System.Buffers.ArrayPool<byte>
	// System.Buffers.ArrayPool<ushort>
	// System.Buffers.ConfigurableArrayPool.Bucket<Hsenl.MergeSortFloatWrap<object>>
	// System.Buffers.ConfigurableArrayPool.Bucket<byte>
	// System.Buffers.ConfigurableArrayPool.Bucket<ushort>
	// System.Buffers.ConfigurableArrayPool<Hsenl.MergeSortFloatWrap<object>>
	// System.Buffers.ConfigurableArrayPool<byte>
	// System.Buffers.ConfigurableArrayPool<ushort>
	// System.Buffers.IBufferWriter<byte>
	// System.Buffers.IBufferWriter<object>
	// System.Buffers.IBufferWriter<ushort>
	// System.Buffers.MemoryManager<byte>
	// System.Buffers.MemoryManager<ushort>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<Hsenl.MergeSortFloatWrap<object>>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<ushort>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<Hsenl.MergeSortFloatWrap<object>>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<ushort>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<Hsenl.MergeSortFloatWrap<object>>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<ushort>
	// System.ByReference<Hsenl.MergeSortFloatWrap<object>>
	// System.ByReference<Hsenl.Vector3>
	// System.ByReference<UnityEngine.GradientAlphaKey>
	// System.ByReference<UnityEngine.GradientColorKey>
	// System.ByReference<UnityEngine.Keyframe>
	// System.ByReference<UnityEngine.jvalue>
	// System.ByReference<byte>
	// System.ByReference<int>
	// System.ByReference<object>
	// System.ByReference<ulong>
	// System.ByReference<ushort>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<Hsenl.InjectionReflectionInfo>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<System.ValueTuple<object,int>>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<Hsenl.InjectionReflectionInfo>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<System.ValueTuple<object,int>>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<Hsenl.InjectionReflectionInfo>
	// System.Collections.Concurrent.ConcurrentQueue<System.ValueTuple<object,int>>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.ArraySortHelper<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.ArraySortHelper<Hsenl.HTask<int>>
	// System.Collections.Generic.ArraySortHelper<Hsenl.HTask>
	// System.Collections.Generic.ArraySortHelper<Hsenl.Network.AckItem>
	// System.Collections.Generic.ArraySortHelper<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.ArraySortHelper<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.ArraySortHelper<Hsenl.Vector3>
	// System.Collections.Generic.ArraySortHelper<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<int,object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Bounds>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.BoundsInt>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Color32>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Color>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.GradientColorKey>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Keyframe>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.LayerMask>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Quaternion>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.RangeInt>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Rect>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.RectInt>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2Int>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3Int>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector4>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<float>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.ArraySortHelper<uint>
	// System.Collections.Generic.ArraySortHelper<ushort>
	// System.Collections.Generic.Comparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.Comparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.Comparer<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.Comparer<Hsenl.HTask<int>>
	// System.Collections.Generic.Comparer<Hsenl.HTask>
	// System.Collections.Generic.Comparer<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.Comparer<Hsenl.Network.AckItem>
	// System.Collections.Generic.Comparer<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.Comparer<Hsenl.Num>
	// System.Collections.Generic.Comparer<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.Comparer<Hsenl.Vector3>
	// System.Collections.Generic.Comparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<int,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.Comparer<UnityEngine.Bounds>
	// System.Collections.Generic.Comparer<UnityEngine.BoundsInt>
	// System.Collections.Generic.Comparer<UnityEngine.Color32>
	// System.Collections.Generic.Comparer<UnityEngine.Color>
	// System.Collections.Generic.Comparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.Comparer<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.Comparer<UnityEngine.GradientColorKey>
	// System.Collections.Generic.Comparer<UnityEngine.Keyframe>
	// System.Collections.Generic.Comparer<UnityEngine.LayerMask>
	// System.Collections.Generic.Comparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.Comparer<UnityEngine.Quaternion>
	// System.Collections.Generic.Comparer<UnityEngine.RangeInt>
	// System.Collections.Generic.Comparer<UnityEngine.Rect>
	// System.Collections.Generic.Comparer<UnityEngine.RectInt>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3Int>
	// System.Collections.Generic.Comparer<UnityEngine.Vector4>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<uint>
	// System.Collections.Generic.Comparer<ushort>
	// System.Collections.Generic.ComparisonComparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.ComparisonComparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.ComparisonComparer<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.ComparisonComparer<Hsenl.HTask<int>>
	// System.Collections.Generic.ComparisonComparer<Hsenl.HTask>
	// System.Collections.Generic.ComparisonComparer<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.ComparisonComparer<Hsenl.Network.AckItem>
	// System.Collections.Generic.ComparisonComparer<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.ComparisonComparer<Hsenl.Num>
	// System.Collections.Generic.ComparisonComparer<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.ComparisonComparer<Hsenl.Vector3>
	// System.Collections.Generic.ComparisonComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<int,object>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Bounds>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.BoundsInt>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Color32>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Color>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.GradientColorKey>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Keyframe>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.LayerMask>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Quaternion>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.RangeInt>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Rect>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.RectInt>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector3Int>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector4>
	// System.Collections.Generic.ComparisonComparer<byte>
	// System.Collections.Generic.ComparisonComparer<float>
	// System.Collections.Generic.ComparisonComparer<int>
	// System.Collections.Generic.ComparisonComparer<long>
	// System.Collections.Generic.ComparisonComparer<object>
	// System.Collections.Generic.ComparisonComparer<uint>
	// System.Collections.Generic.ComparisonComparer<ushort>
	// System.Collections.Generic.Dictionary.Enumerator<float,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.Dictionary.Enumerator<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.Dictionary.Enumerator<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.Dictionary.Enumerator<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.Enumerator<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.Enumerator<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,uint>
	// System.Collections.Generic.Dictionary.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.Enumerator<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<float,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,uint>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection<float,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.KeyCollection<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,uint>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<float,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,uint>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection<float,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.ValueCollection<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,uint>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,ushort>
	// System.Collections.Generic.Dictionary<float,object>
	// System.Collections.Generic.Dictionary<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.Dictionary<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.Dictionary<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.Dictionary<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<object,uint>
	// System.Collections.Generic.Dictionary<object,ushort>
	// System.Collections.Generic.Dictionary<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.Dictionary<ushort,ushort>
	// System.Collections.Generic.EqualityComparer<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.EqualityComparer<Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.EqualityComparer<Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.EqualityComparer<Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.EqualityComparer<Hsenl.Num>
	// System.Collections.Generic.EqualityComparer<ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.EqualityComparer<ushort>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.ICollection<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.ICollection<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.ICollection<Hsenl.HTask<int>>
	// System.Collections.Generic.ICollection<Hsenl.HTask<object>>
	// System.Collections.Generic.ICollection<Hsenl.HTask>
	// System.Collections.Generic.ICollection<Hsenl.InjectionReflectionInfo>
	// System.Collections.Generic.ICollection<Hsenl.Network.AckItem>
	// System.Collections.Generic.ICollection<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.ICollection<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.ICollection<Hsenl.Vector2>
	// System.Collections.Generic.ICollection<Hsenl.Vector3>
	// System.Collections.Generic.ICollection<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<float,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,Hsenl.Network.Network.RpcInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,Hsenl.Num>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Hsenl.Container.InjectionInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Hsenl.Container.ResolveInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Hsenl.Num>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,uint>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,ushort>>
	// System.Collections.Generic.ICollection<System.ValueTuple<int,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,int>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.ICollection<UnityEngine.Bounds>
	// System.Collections.Generic.ICollection<UnityEngine.BoundsInt>
	// System.Collections.Generic.ICollection<UnityEngine.Color32>
	// System.Collections.Generic.ICollection<UnityEngine.Color>
	// System.Collections.Generic.ICollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ICollection<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.ICollection<UnityEngine.GradientColorKey>
	// System.Collections.Generic.ICollection<UnityEngine.Keyframe>
	// System.Collections.Generic.ICollection<UnityEngine.LayerMask>
	// System.Collections.Generic.ICollection<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ICollection<UnityEngine.Quaternion>
	// System.Collections.Generic.ICollection<UnityEngine.RangeInt>
	// System.Collections.Generic.ICollection<UnityEngine.Rect>
	// System.Collections.Generic.ICollection<UnityEngine.RectInt>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2Int>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3Int>
	// System.Collections.Generic.ICollection<UnityEngine.Vector4>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<float>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<uint>
	// System.Collections.Generic.ICollection<ushort>
	// System.Collections.Generic.IComparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IComparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IComparer<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.IComparer<Hsenl.HTask<int>>
	// System.Collections.Generic.IComparer<Hsenl.HTask>
	// System.Collections.Generic.IComparer<Hsenl.Network.AckItem>
	// System.Collections.Generic.IComparer<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.IComparer<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.IComparer<Hsenl.Vector3>
	// System.Collections.Generic.IComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<int,object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.IComparer<UnityEngine.Bounds>
	// System.Collections.Generic.IComparer<UnityEngine.BoundsInt>
	// System.Collections.Generic.IComparer<UnityEngine.Color32>
	// System.Collections.Generic.IComparer<UnityEngine.Color>
	// System.Collections.Generic.IComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IComparer<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.IComparer<UnityEngine.GradientColorKey>
	// System.Collections.Generic.IComparer<UnityEngine.Keyframe>
	// System.Collections.Generic.IComparer<UnityEngine.LayerMask>
	// System.Collections.Generic.IComparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IComparer<UnityEngine.Quaternion>
	// System.Collections.Generic.IComparer<UnityEngine.RangeInt>
	// System.Collections.Generic.IComparer<UnityEngine.Rect>
	// System.Collections.Generic.IComparer<UnityEngine.RectInt>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3Int>
	// System.Collections.Generic.IComparer<UnityEngine.Vector4>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<float>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IComparer<uint>
	// System.Collections.Generic.IComparer<ushort>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<long,object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IEnumerable<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IEnumerable<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.IEnumerable<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.IEnumerable<Hsenl.HTask<int>>
	// System.Collections.Generic.IEnumerable<Hsenl.HTask>
	// System.Collections.Generic.IEnumerable<Hsenl.InjectionReflectionInfo>
	// System.Collections.Generic.IEnumerable<Hsenl.Network.AckItem>
	// System.Collections.Generic.IEnumerable<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.IEnumerable<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.IEnumerable<Hsenl.Vector2>
	// System.Collections.Generic.IEnumerable<Hsenl.Vector3>
	// System.Collections.Generic.IEnumerable<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<float,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,Hsenl.Network.Network.RpcInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,Hsenl.Num>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Hsenl.Container.InjectionInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Hsenl.Container.ResolveInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Hsenl.Num>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,uint>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,ushort>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<int,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.Bounds>
	// System.Collections.Generic.IEnumerable<UnityEngine.BoundsInt>
	// System.Collections.Generic.IEnumerable<UnityEngine.Color32>
	// System.Collections.Generic.IEnumerable<UnityEngine.Color>
	// System.Collections.Generic.IEnumerable<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerable<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.IEnumerable<UnityEngine.GradientColorKey>
	// System.Collections.Generic.IEnumerable<UnityEngine.Keyframe>
	// System.Collections.Generic.IEnumerable<UnityEngine.LayerMask>
	// System.Collections.Generic.IEnumerable<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IEnumerable<UnityEngine.Quaternion>
	// System.Collections.Generic.IEnumerable<UnityEngine.RangeInt>
	// System.Collections.Generic.IEnumerable<UnityEngine.Rect>
	// System.Collections.Generic.IEnumerable<UnityEngine.RectInt>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3Int>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector4>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<float>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<uint>
	// System.Collections.Generic.IEnumerable<ushort>
	// System.Collections.Generic.IEnumerator<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IEnumerator<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IEnumerator<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.IEnumerator<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.IEnumerator<Hsenl.HTask<int>>
	// System.Collections.Generic.IEnumerator<Hsenl.HTask>
	// System.Collections.Generic.IEnumerator<Hsenl.InjectionReflectionInfo>
	// System.Collections.Generic.IEnumerator<Hsenl.Network.AckItem>
	// System.Collections.Generic.IEnumerator<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.IEnumerator<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.IEnumerator<Hsenl.Vector2>
	// System.Collections.Generic.IEnumerator<Hsenl.Vector3>
	// System.Collections.Generic.IEnumerator<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<float,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,Hsenl.Network.Network.RpcInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,Hsenl.Num>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Hsenl.Container.InjectionInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Hsenl.Container.ResolveInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Hsenl.Num>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,uint>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,ushort>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<int,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.IEnumerator<UnityEngine.Bounds>
	// System.Collections.Generic.IEnumerator<UnityEngine.BoundsInt>
	// System.Collections.Generic.IEnumerator<UnityEngine.Color32>
	// System.Collections.Generic.IEnumerator<UnityEngine.Color>
	// System.Collections.Generic.IEnumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerator<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.IEnumerator<UnityEngine.GradientColorKey>
	// System.Collections.Generic.IEnumerator<UnityEngine.Keyframe>
	// System.Collections.Generic.IEnumerator<UnityEngine.LayerMask>
	// System.Collections.Generic.IEnumerator<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IEnumerator<UnityEngine.Quaternion>
	// System.Collections.Generic.IEnumerator<UnityEngine.RangeInt>
	// System.Collections.Generic.IEnumerator<UnityEngine.Rect>
	// System.Collections.Generic.IEnumerator<UnityEngine.RectInt>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2Int>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3Int>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector4>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<float>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<uint>
	// System.Collections.Generic.IEnumerator<ushort>
	// System.Collections.Generic.IEqualityComparer<float>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IEqualityComparer<ushort>
	// System.Collections.Generic.IList<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IList<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IList<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.IList<Hsenl.HTask<int>>
	// System.Collections.Generic.IList<Hsenl.HTask<object>>
	// System.Collections.Generic.IList<Hsenl.HTask>
	// System.Collections.Generic.IList<Hsenl.Network.AckItem>
	// System.Collections.Generic.IList<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.IList<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.IList<Hsenl.Vector2>
	// System.Collections.Generic.IList<Hsenl.Vector3>
	// System.Collections.Generic.IList<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IList<System.ValueTuple<int,object>>
	// System.Collections.Generic.IList<System.ValueTuple<object,object>>
	// System.Collections.Generic.IList<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.IList<UnityEngine.Bounds>
	// System.Collections.Generic.IList<UnityEngine.BoundsInt>
	// System.Collections.Generic.IList<UnityEngine.Color32>
	// System.Collections.Generic.IList<UnityEngine.Color>
	// System.Collections.Generic.IList<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IList<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.IList<UnityEngine.GradientColorKey>
	// System.Collections.Generic.IList<UnityEngine.Keyframe>
	// System.Collections.Generic.IList<UnityEngine.LayerMask>
	// System.Collections.Generic.IList<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IList<UnityEngine.Quaternion>
	// System.Collections.Generic.IList<UnityEngine.RangeInt>
	// System.Collections.Generic.IList<UnityEngine.Rect>
	// System.Collections.Generic.IList<UnityEngine.RectInt>
	// System.Collections.Generic.IList<UnityEngine.Vector2>
	// System.Collections.Generic.IList<UnityEngine.Vector2Int>
	// System.Collections.Generic.IList<UnityEngine.Vector3>
	// System.Collections.Generic.IList<UnityEngine.Vector3Int>
	// System.Collections.Generic.IList<UnityEngine.Vector4>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<float>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IList<uint>
	// System.Collections.Generic.IList<ushort>
	// System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IReadOnlyCollection<object>
	// System.Collections.Generic.IReadOnlyDictionary<int,object>
	// System.Collections.Generic.IReadOnlyDictionary<object,object>
	// System.Collections.Generic.IReadOnlyList<object>
	// System.Collections.Generic.KeyValuePair<System.UIntPtr,object>
	// System.Collections.Generic.KeyValuePair<float,object>
	// System.Collections.Generic.KeyValuePair<int,Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.KeyValuePair<int,Hsenl.Num>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.KeyValuePair<object,Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.KeyValuePair<object,Hsenl.Num>
	// System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<object,uint>
	// System.Collections.Generic.KeyValuePair<object,ushort>
	// System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.KeyValuePair<ushort,object>
	// System.Collections.Generic.KeyValuePair<ushort,ushort>
	// System.Collections.Generic.LinkedList.Enumerator<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.LinkedList.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedList<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.LinkedList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedListNode<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.LinkedListNode<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.List.Enumerator<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.List.Enumerator<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.List.Enumerator<Hsenl.HTask<int>>
	// System.Collections.Generic.List.Enumerator<Hsenl.HTask>
	// System.Collections.Generic.List.Enumerator<Hsenl.Network.AckItem>
	// System.Collections.Generic.List.Enumerator<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.List.Enumerator<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.List.Enumerator<Hsenl.Vector3>
	// System.Collections.Generic.List.Enumerator<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<int,object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Bounds>
	// System.Collections.Generic.List.Enumerator<UnityEngine.BoundsInt>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Color32>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Color>
	// System.Collections.Generic.List.Enumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List.Enumerator<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.List.Enumerator<UnityEngine.GradientColorKey>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Keyframe>
	// System.Collections.Generic.List.Enumerator<UnityEngine.LayerMask>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Matrix4x4>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Quaternion>
	// System.Collections.Generic.List.Enumerator<UnityEngine.RangeInt>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Rect>
	// System.Collections.Generic.List.Enumerator<UnityEngine.RectInt>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2Int>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3Int>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector4>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<float>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<uint>
	// System.Collections.Generic.List.Enumerator<ushort>
	// System.Collections.Generic.List<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.List<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.List<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.List<Hsenl.HTask<int>>
	// System.Collections.Generic.List<Hsenl.HTask>
	// System.Collections.Generic.List<Hsenl.Network.AckItem>
	// System.Collections.Generic.List<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.List<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.List<Hsenl.Vector3>
	// System.Collections.Generic.List<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List<System.ValueTuple<int,object>>
	// System.Collections.Generic.List<System.ValueTuple<object,object>>
	// System.Collections.Generic.List<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.List<UnityEngine.Bounds>
	// System.Collections.Generic.List<UnityEngine.BoundsInt>
	// System.Collections.Generic.List<UnityEngine.Color32>
	// System.Collections.Generic.List<UnityEngine.Color>
	// System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.List<UnityEngine.GradientColorKey>
	// System.Collections.Generic.List<UnityEngine.Keyframe>
	// System.Collections.Generic.List<UnityEngine.LayerMask>
	// System.Collections.Generic.List<UnityEngine.Matrix4x4>
	// System.Collections.Generic.List<UnityEngine.Quaternion>
	// System.Collections.Generic.List<UnityEngine.RangeInt>
	// System.Collections.Generic.List<UnityEngine.Rect>
	// System.Collections.Generic.List<UnityEngine.RectInt>
	// System.Collections.Generic.List<UnityEngine.Vector2>
	// System.Collections.Generic.List<UnityEngine.Vector2Int>
	// System.Collections.Generic.List<UnityEngine.Vector3>
	// System.Collections.Generic.List<UnityEngine.Vector3Int>
	// System.Collections.Generic.List<UnityEngine.Vector4>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<float>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<uint>
	// System.Collections.Generic.List<ushort>
	// System.Collections.Generic.ObjectComparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.ObjectComparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.ObjectComparer<Hsenl.Container.MappingInfo>
	// System.Collections.Generic.ObjectComparer<Hsenl.HTask<int>>
	// System.Collections.Generic.ObjectComparer<Hsenl.HTask>
	// System.Collections.Generic.ObjectComparer<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.ObjectComparer<Hsenl.Network.AckItem>
	// System.Collections.Generic.ObjectComparer<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.ObjectComparer<Hsenl.Num>
	// System.Collections.Generic.ObjectComparer<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.Generic.ObjectComparer<Hsenl.Vector3>
	// System.Collections.Generic.ObjectComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<int,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<ushort,object>>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Bounds>
	// System.Collections.Generic.ObjectComparer<UnityEngine.BoundsInt>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Color32>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Color>
	// System.Collections.Generic.ObjectComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ObjectComparer<UnityEngine.GradientAlphaKey>
	// System.Collections.Generic.ObjectComparer<UnityEngine.GradientColorKey>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Keyframe>
	// System.Collections.Generic.ObjectComparer<UnityEngine.LayerMask>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Quaternion>
	// System.Collections.Generic.ObjectComparer<UnityEngine.RangeInt>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Rect>
	// System.Collections.Generic.ObjectComparer<UnityEngine.RectInt>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2Int>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3Int>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector4>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<uint>
	// System.Collections.Generic.ObjectComparer<ushort>
	// System.Collections.Generic.ObjectEqualityComparer<Hsenl.ConcaveHull2.Edge>
	// System.Collections.Generic.ObjectEqualityComparer<Hsenl.Container.InjectionInfo>
	// System.Collections.Generic.ObjectEqualityComparer<Hsenl.Container.ResolveInfo>
	// System.Collections.Generic.ObjectEqualityComparer<Hsenl.Network.Network.RpcInfo>
	// System.Collections.Generic.ObjectEqualityComparer<Hsenl.Num>
	// System.Collections.Generic.ObjectEqualityComparer<ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.EventSystemManager.LateUpdateWrap>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.EventSystemManager.StartWrap>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.EventSystemManager.UpdateWrap>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.HTask>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<int,long,int>>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<long,long,int>>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<object,int,int,object>>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.Queue.Enumerator<int>
	// System.Collections.Generic.Queue.Enumerator<long>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<Hsenl.EventSystemManager.LateUpdateWrap>
	// System.Collections.Generic.Queue<Hsenl.EventSystemManager.StartWrap>
	// System.Collections.Generic.Queue<Hsenl.EventSystemManager.UpdateWrap>
	// System.Collections.Generic.Queue<Hsenl.HTask>
	// System.Collections.Generic.Queue<Hsenl.Network.SegmentStruct>
	// System.Collections.Generic.Queue<System.ValueTuple<int,long,int>>
	// System.Collections.Generic.Queue<System.ValueTuple<long,long,int>>
	// System.Collections.Generic.Queue<System.ValueTuple<object,int,int,object>>
	// System.Collections.Generic.Queue<System.ValueTuple<object,object>>
	// System.Collections.Generic.Queue<int>
	// System.Collections.Generic.Queue<long>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<long,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<object,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<long,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<object,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<object,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<int,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<long,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<object,object>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary<long,object>
	// System.Collections.Generic.SortedDictionary<object,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.Container.MappingInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.HTask<int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.HTask>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.Network.AckItem>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.Network.SegmentStruct>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Collections.ObjectModel.ReadOnlyCollection<Hsenl.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<MemoryPack.Internal.BufferSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<ushort,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Bounds>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.BoundsInt>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Color32>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Color>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.GradientAlphaKey>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.GradientColorKey>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Keyframe>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.LayerMask>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Matrix4x4>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Quaternion>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.RangeInt>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Rect>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.RectInt>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2Int>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3Int>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector4>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<float>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<uint>
	// System.Collections.ObjectModel.ReadOnlyCollection<ushort>
	// System.Comparison<Cysharp.Text.Utf16FormatSegment>
	// System.Comparison<Cysharp.Text.Utf8FormatSegment>
	// System.Comparison<Hsenl.Container.MappingInfo>
	// System.Comparison<Hsenl.HTask<int>>
	// System.Comparison<Hsenl.HTask>
	// System.Comparison<Hsenl.MergeSortFloatWrap<object>>
	// System.Comparison<Hsenl.Network.AckItem>
	// System.Comparison<Hsenl.Network.SegmentStruct>
	// System.Comparison<Hsenl.Num>
	// System.Comparison<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Comparison<Hsenl.Vector3>
	// System.Comparison<MemoryPack.Internal.BufferSegment>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Comparison<System.ValueTuple<int,object>>
	// System.Comparison<System.ValueTuple<object,object>>
	// System.Comparison<System.ValueTuple<ushort,object>>
	// System.Comparison<UnityEngine.Bounds>
	// System.Comparison<UnityEngine.BoundsInt>
	// System.Comparison<UnityEngine.Color32>
	// System.Comparison<UnityEngine.Color>
	// System.Comparison<UnityEngine.EventSystems.RaycastResult>
	// System.Comparison<UnityEngine.GradientAlphaKey>
	// System.Comparison<UnityEngine.GradientColorKey>
	// System.Comparison<UnityEngine.Keyframe>
	// System.Comparison<UnityEngine.LayerMask>
	// System.Comparison<UnityEngine.Matrix4x4>
	// System.Comparison<UnityEngine.Quaternion>
	// System.Comparison<UnityEngine.RangeInt>
	// System.Comparison<UnityEngine.Rect>
	// System.Comparison<UnityEngine.RectInt>
	// System.Comparison<UnityEngine.Vector2>
	// System.Comparison<UnityEngine.Vector2Int>
	// System.Comparison<UnityEngine.Vector3>
	// System.Comparison<UnityEngine.Vector3Int>
	// System.Comparison<UnityEngine.Vector4>
	// System.Comparison<byte>
	// System.Comparison<float>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Comparison<uint>
	// System.Comparison<ushort>
	// System.EventHandler<object>
	// System.Func<Hsenl.HTask<int>>
	// System.Func<Hsenl.HTask>
	// System.Func<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Func<System.ValueTuple<ushort,object>,object>
	// System.Func<System.ValueTuple<ushort,object>,ushort>
	// System.Func<byte>
	// System.Func<float,float>
	// System.Func<float,int>
	// System.Func<int,System.Memory<byte>>
	// System.Func<int,System.ValueTuple<object,int,int,object>>
	// System.Func<int,System.ValueTuple<object,int,int>>
	// System.Func<int,object,object>
	// System.Func<int,object>
	// System.Func<int>
	// System.Func<long,object,object>
	// System.Func<long,object>
	// System.Func<object,Hsenl.HTask<byte>>
	// System.Func<object,Hsenl.HTask<int>>
	// System.Func<object,Hsenl.HTask>
	// System.Func<object,System.ValueTuple<object,object,object>,byte>
	// System.Func<object,byte>
	// System.Func<object,int,System.ValueTuple<object,object,int,object>,byte>
	// System.Func<object,int,int,object>
	// System.Func<object,int,object,byte>
	// System.Func<object,int>
	// System.Func<object,object,Hsenl.HTask>
	// System.Func<object,object,byte>
	// System.Func<object,object,object,Hsenl.HTask>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IComparable<Hsenl.Fixp>
	// System.IComparable<Hsenl.MergeSortFloatWrap<object>>
	// System.IComparable<object>
	// System.IEquatable<Hsenl.Fixp>
	// System.IEquatable<Hsenl.Matrix3x3>
	// System.IEquatable<Hsenl.Matrix4x4>
	// System.IEquatable<Hsenl.Vector2>
	// System.IEquatable<Hsenl.Vector3>
	// System.IEquatable<Hsenl.Vector4>
	// System.IEquatable<object>
	// System.IEquatable<ushort>
	// System.Linq.Buffer<int>
	// System.Linq.Buffer<object>
	// System.Linq.Buffer<uint>
	// System.Linq.Enumerable.<ConcatIterator>d__59<object>
	// System.Linq.Enumerable.<DistinctIterator>d__68<object>
	// System.Linq.Enumerable.<SelectManyIterator>d__17<object,object>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>
	// System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.Set<object>
	// System.Memory<byte>
	// System.Memory<ushort>
	// System.Nullable<Hsenl.HTask<object>>
	// System.Nullable<Hsenl.HTask>
	// System.Nullable<Hsenl.Vector3>
	// System.Nullable<System.DateTime>
	// System.Nullable<System.DateTimeOffset>
	// System.Nullable<System.Decimal>
	// System.Nullable<System.Guid>
	// System.Nullable<System.TimeSpan>
	// System.Nullable<UnityEngine.Bounds>
	// System.Nullable<UnityEngine.BoundsInt>
	// System.Nullable<UnityEngine.Color32>
	// System.Nullable<UnityEngine.Color>
	// System.Nullable<UnityEngine.GradientAlphaKey>
	// System.Nullable<UnityEngine.GradientColorKey>
	// System.Nullable<UnityEngine.Keyframe>
	// System.Nullable<UnityEngine.LayerMask>
	// System.Nullable<UnityEngine.Matrix4x4>
	// System.Nullable<UnityEngine.Quaternion>
	// System.Nullable<UnityEngine.RangeInt>
	// System.Nullable<UnityEngine.Rect>
	// System.Nullable<UnityEngine.RectInt>
	// System.Nullable<UnityEngine.Vector2>
	// System.Nullable<UnityEngine.Vector2Int>
	// System.Nullable<UnityEngine.Vector3>
	// System.Nullable<UnityEngine.Vector3Int>
	// System.Nullable<UnityEngine.Vector4>
	// System.Nullable<byte>
	// System.Nullable<double>
	// System.Nullable<float>
	// System.Nullable<int>
	// System.Nullable<long>
	// System.Nullable<object>
	// System.Nullable<sbyte>
	// System.Nullable<short>
	// System.Nullable<uint>
	// System.Nullable<ulong>
	// System.Nullable<ushort>
	// System.Predicate<Cysharp.Text.Utf16FormatSegment>
	// System.Predicate<Cysharp.Text.Utf8FormatSegment>
	// System.Predicate<Hsenl.Container.MappingInfo>
	// System.Predicate<Hsenl.HTask<int>>
	// System.Predicate<Hsenl.HTask>
	// System.Predicate<Hsenl.Network.AckItem>
	// System.Predicate<Hsenl.Network.SegmentStruct>
	// System.Predicate<Hsenl.ShadowFunctionManager.DelegateWrap>
	// System.Predicate<Hsenl.Vector3>
	// System.Predicate<MemoryPack.Internal.BufferSegment>
	// System.Predicate<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Predicate<System.ValueTuple<int,object>>
	// System.Predicate<System.ValueTuple<object,object>>
	// System.Predicate<System.ValueTuple<ushort,object>>
	// System.Predicate<UnityEngine.Bounds>
	// System.Predicate<UnityEngine.BoundsInt>
	// System.Predicate<UnityEngine.Color32>
	// System.Predicate<UnityEngine.Color>
	// System.Predicate<UnityEngine.EventSystems.RaycastResult>
	// System.Predicate<UnityEngine.GradientAlphaKey>
	// System.Predicate<UnityEngine.GradientColorKey>
	// System.Predicate<UnityEngine.Keyframe>
	// System.Predicate<UnityEngine.LayerMask>
	// System.Predicate<UnityEngine.Matrix4x4>
	// System.Predicate<UnityEngine.Quaternion>
	// System.Predicate<UnityEngine.RangeInt>
	// System.Predicate<UnityEngine.Rect>
	// System.Predicate<UnityEngine.RectInt>
	// System.Predicate<UnityEngine.Vector2>
	// System.Predicate<UnityEngine.Vector2Int>
	// System.Predicate<UnityEngine.Vector3>
	// System.Predicate<UnityEngine.Vector3Int>
	// System.Predicate<UnityEngine.Vector4>
	// System.Predicate<byte>
	// System.Predicate<float>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.Predicate<uint>
	// System.Predicate<ushort>
	// System.ReadOnlyMemory<byte>
	// System.ReadOnlyMemory<ushort>
	// System.ReadOnlySpan.Enumerator<Hsenl.MergeSortFloatWrap<object>>
	// System.ReadOnlySpan.Enumerator<Hsenl.Vector3>
	// System.ReadOnlySpan.Enumerator<UnityEngine.GradientAlphaKey>
	// System.ReadOnlySpan.Enumerator<UnityEngine.GradientColorKey>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Keyframe>
	// System.ReadOnlySpan.Enumerator<UnityEngine.jvalue>
	// System.ReadOnlySpan.Enumerator<byte>
	// System.ReadOnlySpan.Enumerator<int>
	// System.ReadOnlySpan.Enumerator<object>
	// System.ReadOnlySpan.Enumerator<ulong>
	// System.ReadOnlySpan.Enumerator<ushort>
	// System.ReadOnlySpan<Hsenl.MergeSortFloatWrap<object>>
	// System.ReadOnlySpan<Hsenl.Vector3>
	// System.ReadOnlySpan<UnityEngine.GradientAlphaKey>
	// System.ReadOnlySpan<UnityEngine.GradientColorKey>
	// System.ReadOnlySpan<UnityEngine.Keyframe>
	// System.ReadOnlySpan<UnityEngine.jvalue>
	// System.ReadOnlySpan<byte>
	// System.ReadOnlySpan<int>
	// System.ReadOnlySpan<object>
	// System.ReadOnlySpan<ulong>
	// System.ReadOnlySpan<ushort>
	// System.Span.Enumerator<Hsenl.MergeSortFloatWrap<object>>
	// System.Span.Enumerator<Hsenl.Vector3>
	// System.Span.Enumerator<UnityEngine.GradientAlphaKey>
	// System.Span.Enumerator<UnityEngine.GradientColorKey>
	// System.Span.Enumerator<UnityEngine.Keyframe>
	// System.Span.Enumerator<UnityEngine.jvalue>
	// System.Span.Enumerator<byte>
	// System.Span.Enumerator<int>
	// System.Span.Enumerator<object>
	// System.Span.Enumerator<ulong>
	// System.Span.Enumerator<ushort>
	// System.Span<Hsenl.MergeSortFloatWrap<object>>
	// System.Span<Hsenl.Vector3>
	// System.Span<UnityEngine.GradientAlphaKey>
	// System.Span<UnityEngine.GradientColorKey>
	// System.Span<UnityEngine.Keyframe>
	// System.Span<UnityEngine.jvalue>
	// System.Span<byte>
	// System.Span<int>
	// System.Span<object>
	// System.Span<ulong>
	// System.Span<ushort>
	// System.ValueTuple<Hsenl.Num,Hsenl.Num>
	// System.ValueTuple<byte,uint>
	// System.ValueTuple<float,float>
	// System.ValueTuple<int,int>
	// System.ValueTuple<int,long,int>
	// System.ValueTuple<int,object>
	// System.ValueTuple<long,long,int>
	// System.ValueTuple<object,int,int,object>
	// System.ValueTuple<object,int,int>
	// System.ValueTuple<object,int>
	// System.ValueTuple<object,object,int,object>
	// System.ValueTuple<object,object,object>
	// System.ValueTuple<object,object>
	// System.ValueTuple<uint,uint,int>
	// System.ValueTuple<ushort,object>
	// UnityEngine.InputSystem.InputBindingComposite<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputBindingComposite<float>
	// UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputControl<float>
	// UnityEngine.InputSystem.InputProcessor<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputProcessor<float>
	// UnityEngine.InputSystem.Utilities.InlinedArray<object>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray.Enumerator<object>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray<object>
	// }}

	public void RefMethods()
	{
		// object DG.Tweening.TweenSettingsExtensions.From<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.From<object>(object,bool,bool)
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// object DG.Tweening.TweenSettingsExtensions.SetSpeedBased<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.SetSpeedBased<object>(object,bool)
		// System.Collections.Generic.List<object> MemoryPack.Formatters.ListFormatter.DeserializePackable<object>(MemoryPack.MemoryPackReader&)
		// System.Void MemoryPack.Formatters.ListFormatter.DeserializePackable<object>(MemoryPack.MemoryPackReader&,System.Collections.Generic.List<object>&)
		// System.Void MemoryPack.Formatters.ListFormatter.SerializePackable<object,object>(MemoryPack.MemoryPackWriter<object>&,System.Collections.Generic.List<object>)
		// System.Void MemoryPack.IMemoryPackFormatter<object>.Serialize<object>(MemoryPack.MemoryPackWriter<object>&,object&)
		// UnityEngine.GradientAlphaKey[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<UnityEngine.GradientAlphaKey>(int,bool)
		// UnityEngine.GradientColorKey[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<UnityEngine.GradientColorKey>(int,bool)
		// UnityEngine.Keyframe[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<UnityEngine.Keyframe>(int,bool)
		// byte[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<byte>(int,bool)
		// int[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<int>(int,bool)
		// ulong[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<ulong>(int,bool)
		// UnityEngine.GradientAlphaKey& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<UnityEngine.GradientAlphaKey>(UnityEngine.GradientAlphaKey[])
		// UnityEngine.GradientColorKey& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<UnityEngine.GradientColorKey>(UnityEngine.GradientColorKey[])
		// UnityEngine.Keyframe& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<UnityEngine.Keyframe>(UnityEngine.Keyframe[])
		// byte& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<byte>(byte[])
		// int& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<int>(int[])
		// ulong& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<ulong>(ulong[])
		// MemoryPack.MemoryPackFormatter<object> MemoryPack.MemoryPackFormatterProvider.GetFormatter<object>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<Hsenl.C2R_Login>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<Hsenl.R2C_Login>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<int>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<object>()
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<Hsenl.C2R_Login>(MemoryPack.MemoryPackFormatter<Hsenl.C2R_Login>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<Hsenl.R2C_Login>(MemoryPack.MemoryPackFormatter<Hsenl.R2C_Login>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Bounds>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Bounds>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.BoundsInt>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.BoundsInt>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Color32>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Color32>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Color>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Color>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.GradientAlphaKey>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.GradientAlphaKey>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.GradientColorKey>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.GradientColorKey>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Keyframe>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Keyframe>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.LayerMask>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.LayerMask>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Matrix4x4>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Matrix4x4>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Quaternion>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Quaternion>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.RangeInt>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.RangeInt>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Rect>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Rect>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.RectInt>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.RectInt>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Vector2>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector2>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Vector2Int>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector2Int>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Vector3>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector3>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Vector3Int>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector3Int>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<UnityEngine.Vector4>>(MemoryPack.MemoryPackFormatter<System.Nullable<UnityEngine.Vector4>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.Nullable<object>>(MemoryPack.MemoryPackFormatter<System.Nullable<object>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<System.UIntPtr>(MemoryPack.MemoryPackFormatter<System.UIntPtr>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Bounds>(MemoryPack.MemoryPackFormatter<UnityEngine.Bounds>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.BoundsInt>(MemoryPack.MemoryPackFormatter<UnityEngine.BoundsInt>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Color32>(MemoryPack.MemoryPackFormatter<UnityEngine.Color32>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Color>(MemoryPack.MemoryPackFormatter<UnityEngine.Color>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.GradientAlphaKey>(MemoryPack.MemoryPackFormatter<UnityEngine.GradientAlphaKey>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.GradientColorKey>(MemoryPack.MemoryPackFormatter<UnityEngine.GradientColorKey>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Keyframe>(MemoryPack.MemoryPackFormatter<UnityEngine.Keyframe>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.LayerMask>(MemoryPack.MemoryPackFormatter<UnityEngine.LayerMask>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Matrix4x4>(MemoryPack.MemoryPackFormatter<UnityEngine.Matrix4x4>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Quaternion>(MemoryPack.MemoryPackFormatter<UnityEngine.Quaternion>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.RangeInt>(MemoryPack.MemoryPackFormatter<UnityEngine.RangeInt>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Rect>(MemoryPack.MemoryPackFormatter<UnityEngine.Rect>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.RectInt>(MemoryPack.MemoryPackFormatter<UnityEngine.RectInt>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Vector2>(MemoryPack.MemoryPackFormatter<UnityEngine.Vector2>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Vector2Int>(MemoryPack.MemoryPackFormatter<UnityEngine.Vector2Int>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Vector3>(MemoryPack.MemoryPackFormatter<UnityEngine.Vector3>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Vector3Int>(MemoryPack.MemoryPackFormatter<UnityEngine.Vector3Int>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<UnityEngine.Vector4>(MemoryPack.MemoryPackFormatter<UnityEngine.Vector4>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<int>(MemoryPack.MemoryPackFormatter<int>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<object>(MemoryPack.MemoryPackFormatter<object>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.RegisterDictionary<object,object,object>()
		// System.Void MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<int>(int[]&)
		// System.Void MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<ulong>(ulong[]&)
		// UnityEngine.GradientAlphaKey[] MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<UnityEngine.GradientAlphaKey>()
		// UnityEngine.GradientColorKey[] MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<UnityEngine.GradientColorKey>()
		// UnityEngine.Keyframe[] MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<UnityEngine.Keyframe>()
		// int[] MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<int>()
		// ulong[] MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<ulong>()
		// MemoryPack.IMemoryPackFormatter<object> MemoryPack.MemoryPackReader.GetFormatter<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadPackable<object>(object&)
		// object MemoryPack.MemoryPackReader.ReadPackable<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Hsenl.Int2>(Hsenl.Int2&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Hsenl.Vector3>(Hsenl.Vector3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,byte,float,byte,byte>(byte&,byte&,float&,byte&,byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte>(byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<float,float>(float&,float&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<float>(float&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,Hsenl.Vector3>(int&,Hsenl.Vector3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,byte,byte>(int&,byte&,byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,byte,float>(int&,byte&,float&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,byte,int,byte>(int&,byte&,int&,byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,byte,uint>(int&,byte&,uint&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,byte>(int&,byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,float,float,int>(int&,float&,float&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,float,int,byte>(int&,float&,int&,byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,float,int>(int&,float&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,float>(int&,float&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,int,int,byte>(int&,int&,int&,byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,int,int,int,int,int,int,int,int,int,int,int>(int&,int&,int&,int&,int&,int&,int&,int&,int&,int&,int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,int,int,int>(int&,int&,int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,int>(int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<uint>(uint&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanagedArray<int>(int[]&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanagedArray<ulong>(ulong[]&)
		// UnityEngine.GradientAlphaKey[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<UnityEngine.GradientAlphaKey>()
		// UnityEngine.GradientColorKey[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<UnityEngine.GradientColorKey>()
		// UnityEngine.Keyframe[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<UnityEngine.Keyframe>()
		// int[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<int>()
		// ulong[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<ulong>()
		// System.Void MemoryPack.MemoryPackReader.ReadValue<object>(object&)
		// object MemoryPack.MemoryPackReader.ReadValue<object>()
		// int MemoryPack.MemoryPackSerializer.Deserialize<object>(System.ReadOnlySpan<byte>,object&,MemoryPack.MemoryPackSerializerOptions)
		// object MemoryPack.MemoryPackSerializer.Deserialize<object>(System.ReadOnlySpan<byte>,MemoryPack.MemoryPackSerializerOptions)
		// System.Void MemoryPack.MemoryPackSerializer.Serialize<object,object>(MemoryPack.MemoryPackWriter<object>&,object&)
		// System.Void MemoryPack.MemoryPackSerializer.Serialize<object,object>(object&,object&,MemoryPack.MemoryPackSerializerOptions)
		// byte[] MemoryPack.MemoryPackSerializer.Serialize<object>(object&,MemoryPack.MemoryPackSerializerOptions)
		// System.Void MemoryPack.MemoryPackWriter<object>.DangerousWriteUnmanagedArray<UnityEngine.GradientAlphaKey>(UnityEngine.GradientAlphaKey[])
		// System.Void MemoryPack.MemoryPackWriter<object>.DangerousWriteUnmanagedArray<UnityEngine.GradientColorKey>(UnityEngine.GradientColorKey[])
		// System.Void MemoryPack.MemoryPackWriter<object>.DangerousWriteUnmanagedArray<UnityEngine.Keyframe>(UnityEngine.Keyframe[])
		// System.Void MemoryPack.MemoryPackWriter<object>.DangerousWriteUnmanagedArray<int>(int[])
		// System.Void MemoryPack.MemoryPackWriter<object>.DangerousWriteUnmanagedArray<ulong>(ulong[])
		// MemoryPack.IMemoryPackFormatter<object> MemoryPack.MemoryPackWriter<object>.GetFormatter<object>()
		// System.Void MemoryPack.MemoryPackWriter<object>.WritePackable<object>(object&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<Hsenl.Int2>(Hsenl.Int2&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<byte,byte,float,byte,byte>(byte&,byte&,float&,byte&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<byte>(byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<float,float>(float&,float&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,Hsenl.Vector3>(int&,Hsenl.Vector3&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,byte,int,int>(int&,byte&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,byte,int>(int&,byte&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,byte>(int&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,float,int,byte>(int&,float&,int&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,float>(int&,float&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,int,int,byte>(int&,int&,int&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,int,int,int,int,int,int,int,int,int,int,int>(int&,int&,int&,int&,int&,int&,int&,int&,int&,int&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,int,int,int>(int&,int&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int,int>(int&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedArray<UnityEngine.GradientAlphaKey>(UnityEngine.GradientAlphaKey[])
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedArray<UnityEngine.GradientColorKey>(UnityEngine.GradientColorKey[])
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedArray<UnityEngine.Keyframe>(UnityEngine.Keyframe[])
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedArray<int>(int[])
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedArray<ulong>(ulong[])
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,byte,byte>(byte,int&,byte&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,byte,float>(byte,int&,byte&,float&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,byte,int,byte>(byte,int&,byte&,int&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,byte,uint>(byte,int&,byte&,uint&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,byte>(byte,int&,byte&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,float,float,int>(byte,int&,float&,float&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,float,int>(byte,int&,float&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,int,int,int>(byte,int&,int&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int,int>(byte,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteUnmanagedWithObjectHeader<int>(byte,int&)
		// System.Void MemoryPack.MemoryPackWriter<object>.WriteValue<object>(object&)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(Sirenix.Serialization.IDataReader)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(System.IO.Stream,Sirenix.Serialization.DataFormat,Sirenix.Serialization.DeserializationContext)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(byte[],Sirenix.Serialization.DataFormat,Sirenix.Serialization.DeserializationContext)
		// System.Void Sirenix.Serialization.SerializationUtility.SerializeValue<object>(object,Sirenix.Serialization.IDataWriter)
		// System.Void Sirenix.Serialization.SerializationUtility.SerializeValue<object>(object,System.IO.Stream,Sirenix.Serialization.DataFormat,Sirenix.Serialization.SerializationContext)
		// byte[] Sirenix.Serialization.SerializationUtility.SerializeValue<object>(object,Sirenix.Serialization.DataFormat,Sirenix.Serialization.SerializationContext)
		// Sirenix.Serialization.Serializer<object> Sirenix.Serialization.Serializer.Get<object>()
		// object System.Activator.CreateInstance<object>()
		// UnityEngine.GradientAlphaKey[] System.Array.Empty<UnityEngine.GradientAlphaKey>()
		// UnityEngine.GradientColorKey[] System.Array.Empty<UnityEngine.GradientColorKey>()
		// UnityEngine.Keyframe[] System.Array.Empty<UnityEngine.Keyframe>()
		// byte[] System.Array.Empty<byte>()
		// int[] System.Array.Empty<int>()
		// object[] System.Array.Empty<object>()
		// ulong[] System.Array.Empty<ulong>()
		// int System.Array.IndexOf<object>(object[],object)
		// int System.Array.IndexOfImpl<object>(object[],object,int,int)
		// System.Void System.Array.Reverse<object>(object[])
		// System.Void System.Array.Reverse<object>(object[],int,int)
		// System.Void System.Array.Sort<object>(object[])
		// System.Void System.Array.Sort<object>(object[],System.Collections.Generic.IComparer<object>)
		// System.Void System.Array.Sort<object>(object[],System.Comparison<object>)
		// System.Void System.Array.Sort<object>(object[],int,int,System.Collections.Generic.IComparer<object>)
		// int System.Enum.Parse<int>(string)
		// int System.Enum.Parse<int>(string,bool)
		// int System.HashCode.Combine<int,int,int,int,int>(int,int,int,int,int)
		// int System.HashCode.Combine<int,int,int,int>(int,int,int,int)
		// int System.HashCode.Combine<int,int,int>(int,int,int)
		// int System.HashCode.Combine<int,int>(int,int)
		// int System.HashCode.Combine<int,object,object,int>(int,object,object,int)
		// int System.HashCode.Combine<long,byte>(long,byte)
		// int System.HashCode.Combine<object,int>(object,int)
		// int System.HashCode.Combine<object,object>(object,object)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.AsEnumerable<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Concat<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.ConcatIterator<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEnumerable<object>)
		// bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object)
		// bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object,System.Collections.Generic.IEqualityComparer<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Distinct<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEqualityComparer<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.DistinctIterator<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEqualityComparer<object>)
		// System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.ElementAt<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,int)
		// System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>,System.Func<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<object,object>,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectMany<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectManyIterator<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// int[] System.Linq.Enumerable.ToArray<int>(System.Collections.Generic.IEnumerable<int>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// uint[] System.Linq.Enumerable.ToArray<uint>(System.Collections.Generic.IEnumerable<uint>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>,System.Func<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,bool>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>>.Select<object>(System.Func<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<int,int>>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>.Select<object>(System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Memory<byte> System.MemoryExtensions.AsMemory<byte>(byte[])
		// System.Memory<byte> System.MemoryExtensions.AsMemory<byte>(byte[],int)
		// System.Memory<byte> System.MemoryExtensions.AsMemory<byte>(byte[],int,int)
		// System.Memory<ushort> System.MemoryExtensions.AsMemory<ushort>(ushort[],int)
		// System.Memory<ushort> System.MemoryExtensions.AsMemory<ushort>(ushort[],int,int)
		// System.Span<Hsenl.MergeSortFloatWrap<object>> System.MemoryExtensions.AsSpan<Hsenl.MergeSortFloatWrap<object>>(Hsenl.MergeSortFloatWrap<object>[],int,int)
		// System.Span<Hsenl.Vector3> System.MemoryExtensions.AsSpan<Hsenl.Vector3>(Hsenl.Vector3[],int,int)
		// System.Span<UnityEngine.GradientAlphaKey> System.MemoryExtensions.AsSpan<UnityEngine.GradientAlphaKey>(UnityEngine.GradientAlphaKey[])
		// System.Span<UnityEngine.GradientColorKey> System.MemoryExtensions.AsSpan<UnityEngine.GradientColorKey>(UnityEngine.GradientColorKey[])
		// System.Span<UnityEngine.Keyframe> System.MemoryExtensions.AsSpan<UnityEngine.Keyframe>(UnityEngine.Keyframe[])
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[])
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[],int)
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[],int,int)
		// System.Span<int> System.MemoryExtensions.AsSpan<int>(int[])
		// System.Span<object> System.MemoryExtensions.AsSpan<object>(object[])
		// System.Span<ulong> System.MemoryExtensions.AsSpan<ulong>(ulong[])
		// System.Span<ushort> System.MemoryExtensions.AsSpan<ushort>(ushort[])
		// System.Span<ushort> System.MemoryExtensions.AsSpan<ushort>(ushort[],int)
		// System.Span<ushort> System.MemoryExtensions.AsSpan<ushort>(ushort[],int,int)
		// bool System.MemoryExtensions.IsTypeComparableAsBytes<ushort>(ulong&)
		// bool System.MemoryExtensions.StartsWith<ushort>(System.ReadOnlySpan<ushort>,System.ReadOnlySpan<ushort>)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo,bool)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.AdvDefaultCheckpoints.<OnSceneChanged>d__9>(Hsenl.HTask.Awaiter&,Hsenl.AdvDefaultCheckpoints.<OnSceneChanged>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.MonoTimer.<TimeStart>d__0>(Hsenl.HTask.Awaiter&,Hsenl.MonoTimer.<TimeStart>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.Pickable.<LoadCollider>d__11>(Hsenl.HTask.Awaiter&,Hsenl.Pickable.<LoadCollider>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.PlhDie_DestoryDeadBody.<WaitForDestoryBody>d__1>(Hsenl.HTask.Awaiter&,Hsenl.PlhDie_DestoryDeadBody.<WaitForDestoryBody>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.PlhHarm_SplitBolt.<FireAsync>d__3>(Hsenl.HTask.Awaiter&,Hsenl.PlhHarm_SplitBolt.<FireAsync>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.ShadowFunctionSystemExample_Source.<Func22>d__6>(Hsenl.HTask.Awaiter&,Hsenl.ShadowFunctionSystemExample_Source.<Func22>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.ShadowFunctionSystemExample_Source.<Func2>d__1>(Hsenl.HTask.Awaiter&,Hsenl.ShadowFunctionSystemExample_Source.<Func2>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.ShadowFunctionSystemExample_Source2.<Func22>d__6>(Hsenl.HTask.Awaiter&,Hsenl.ShadowFunctionSystemExample_Source2.<Func22>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.ShadowFunctionSystemExample_Source2.<Func2>d__1>(Hsenl.HTask.Awaiter&,Hsenl.ShadowFunctionSystemExample_Source2.<Func2>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.ShadowFunctionSystemExample_Source3.<Func22>d__8<object,object>>(Hsenl.HTask.Awaiter&,Hsenl.ShadowFunctionSystemExample_Source3.<Func22>d__8<object,object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.ShadowFunctionSystemExample_Source3.<Func2>d__3<object,object>>(Hsenl.HTask.Awaiter&,Hsenl.ShadowFunctionSystemExample_Source3.<Func2>d__3<object,object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d>(Hsenl.HTask.Awaiter&,Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.TpHarmOfTargetedBolt.<Fire>d__4>(Hsenl.HTask.Awaiter&,Hsenl.TpHarmOfTargetedBolt.<Fire>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter,Hsenl.View.UIJumpMessage.<WriteText>d__10>(Hsenl.HTask.Awaiter&,Hsenl.View.UIJumpMessage.<WriteText>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter<byte>,Hsenl.AdvDefaultCheckpoints.<NextCheckpoint>d__8>(Hsenl.HTask.Awaiter<byte>&,Hsenl.AdvDefaultCheckpoints.<NextCheckpoint>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.HTask.Awaiter<object>,Hsenl.Network.AMessageHandlerAsync.<Run>d__7<object,object>>(Hsenl.HTask.Awaiter<object>&,Hsenl.Network.AMessageHandlerAsync.<Run>d__7<object,object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Hsenl.Network.KcpClient.<<ConnectAsync>g__Timeout|25_0>d>(System.Runtime.CompilerServices.TaskAwaiter&,Hsenl.Network.KcpClient.<<ConnectAsync>g__Timeout|25_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.AdvDefaultCheckpoints.<NextCheckpoint>d__8>(Hsenl.AdvDefaultCheckpoints.<NextCheckpoint>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.AdvDefaultCheckpoints.<OnSceneChanged>d__9>(Hsenl.AdvDefaultCheckpoints.<OnSceneChanged>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.MonoTimer.<TimeStart>d__0>(Hsenl.MonoTimer.<TimeStart>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.Network.AMessageHandlerAsync.<Run>d__7<object,object>>(Hsenl.Network.AMessageHandlerAsync.<Run>d__7<object,object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.Network.KcpClient.<<ConnectAsync>g__Timeout|25_0>d>(Hsenl.Network.KcpClient.<<ConnectAsync>g__Timeout|25_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.Pickable.<LoadCollider>d__11>(Hsenl.Pickable.<LoadCollider>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.PlhAbilityStageChanged_RepetitionCast.<RepetitionCast>d__1>(Hsenl.PlhAbilityStageChanged_RepetitionCast.<RepetitionCast>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.PlhDie_DestoryDeadBody.<WaitForDestoryBody>d__1>(Hsenl.PlhDie_DestoryDeadBody.<WaitForDestoryBody>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.PlhHarm_SplitBolt.<FireAsync>d__3>(Hsenl.PlhHarm_SplitBolt.<FireAsync>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ShadowFunctionSystemExample_Source.<Func22>d__6>(Hsenl.ShadowFunctionSystemExample_Source.<Func22>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ShadowFunctionSystemExample_Source.<Func2>d__1>(Hsenl.ShadowFunctionSystemExample_Source.<Func2>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ShadowFunctionSystemExample_Source2.<Func22>d__6>(Hsenl.ShadowFunctionSystemExample_Source2.<Func22>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ShadowFunctionSystemExample_Source2.<Func2>d__1>(Hsenl.ShadowFunctionSystemExample_Source2.<Func2>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ShadowFunctionSystemExample_Source3.<Func22>d__8<object,object>>(Hsenl.ShadowFunctionSystemExample_Source3.<Func22>d__8<object,object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ShadowFunctionSystemExample_Source3.<Func2>d__3<object,object>>(Hsenl.ShadowFunctionSystemExample_Source3.<Func2>d__3<object,object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d>(Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.TpHarmOfTargetedBolt.<Fire>d__4>(Hsenl.TpHarmOfTargetedBolt.<Fire>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.View.UIJumpMessage.<WriteText>d__10>(Hsenl.View.UIJumpMessage.<WriteText>d__10&)
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<object>()
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// object& System.Runtime.CompilerServices.Unsafe.Add<object>(object&,int)
		// ushort& System.Runtime.CompilerServices.Unsafe.Add<ushort>(ushort&,System.IntPtr)
		// ushort& System.Runtime.CompilerServices.Unsafe.Add<ushort>(ushort&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.AddByteOffset<byte>(byte&,System.IntPtr)
		// ushort& System.Runtime.CompilerServices.Unsafe.AddByteOffset<ushort>(ushort&,System.IntPtr)
		// bool System.Runtime.CompilerServices.Unsafe.AreSame<ushort>(ushort&,ushort&)
		// System.DecimalEx.DecCalc& System.Runtime.CompilerServices.Unsafe.As<System.Decimal,System.DecimalEx.DecCalc>(System.Decimal&)
		// System.DecimalEx.DecimalBits& System.Runtime.CompilerServices.Unsafe.As<System.Decimal,System.DecimalEx.DecimalBits>(System.Decimal&)
		// System.GuidEx& System.Runtime.CompilerServices.Unsafe.As<System.Guid,System.GuidEx>(System.Guid&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.GradientAlphaKey,byte>(UnityEngine.GradientAlphaKey&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.GradientColorKey,byte>(UnityEngine.GradientColorKey&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Keyframe,byte>(UnityEngine.Keyframe&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<int,byte>(int&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<ulong,byte>(ulong&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<ushort,byte>(ushort&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<ushort,byte>(ushort&)
		// int& System.Runtime.CompilerServices.Unsafe.As<object,int>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<byte,object>(byte&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// uint& System.Runtime.CompilerServices.Unsafe.As<int,uint>(int&)
		// ulong& System.Runtime.CompilerServices.Unsafe.As<long,ulong>(long&)
		// ushort& System.Runtime.CompilerServices.Unsafe.As<byte,ushort>(byte&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<byte>(byte&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<uint>(uint&)
		// object& System.Runtime.CompilerServices.Unsafe.AsRef<object>(object&)
		// bool System.Runtime.CompilerServices.Unsafe.IsAddressLessThan<object>(object&,object&)
		// object& System.Runtime.CompilerServices.Unsafe.NullRef<object>()
		// Hsenl.Int2 System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Hsenl.Int2>(byte&)
		// Hsenl.Network.Kcp.SegmentHead System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Hsenl.Network.Kcp.SegmentHead>(byte&)
		// Hsenl.Vector3 System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Hsenl.Vector3>(byte&)
		// byte System.Runtime.CompilerServices.Unsafe.ReadUnaligned<byte>(byte&)
		// float System.Runtime.CompilerServices.Unsafe.ReadUnaligned<float>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(byte&)
		// object System.Runtime.CompilerServices.Unsafe.ReadUnaligned<object>(byte&)
		// uint System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>(byte&)
		// ulong System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>(byte&)
		// ushort System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ushort>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<Hsenl.Int2>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<Hsenl.Network.Kcp.SegmentHead>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<Hsenl.Vector3>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.GradientAlphaKey>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.GradientColorKey>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Keyframe>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<byte>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<float>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<object>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<uint>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<ulong>()
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<Hsenl.Int2>(byte&,Hsenl.Int2)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<Hsenl.Network.Kcp.SegmentHead>(byte&,Hsenl.Network.Kcp.SegmentHead)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<Hsenl.Vector3>(byte&,Hsenl.Vector3)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<byte>(byte&,byte)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<float>(byte&,float)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<int>(byte&,int)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<object>(byte&,object)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<uint>(byte&,uint)
		// System.Void System.Runtime.InteropServices.Marshal.StructureToPtr<object>(object,System.IntPtr,bool)
		// System.Span<byte> System.Runtime.InteropServices.MemoryMarshal.CreateSpan<byte>(byte&,int)
		// UnityEngine.GradientAlphaKey& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.GradientAlphaKey>(System.Span<UnityEngine.GradientAlphaKey>)
		// UnityEngine.GradientColorKey& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.GradientColorKey>(System.Span<UnityEngine.GradientColorKey>)
		// UnityEngine.Keyframe& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Keyframe>(System.Span<UnityEngine.Keyframe>)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.ReadOnlySpan<byte>)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.Span<byte>)
		// int& System.Runtime.InteropServices.MemoryMarshal.GetReference<int>(System.Span<int>)
		// ulong& System.Runtime.InteropServices.MemoryMarshal.GetReference<ulong>(System.Span<ulong>)
		// ushort& System.Runtime.InteropServices.MemoryMarshal.GetReference<ushort>(System.ReadOnlySpan<ushort>)
		// ushort& System.Runtime.InteropServices.MemoryMarshal.GetReference<ushort>(System.Span<ushort>)
		// bool System.SpanHelpers.SequenceEqual<ushort>(ushort&,ushort&,int)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<float>(float&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<float>()
		// object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// object UnityEngine.AndroidJavaObject.Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject.CallStatic<object>(string,object[])
		// object UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject.GetStatic<object>(string)
		// object UnityEngine.AndroidJavaObject._Call<object>(System.IntPtr,object[])
		// object UnityEngine.AndroidJavaObject._Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(System.IntPtr,object[])
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(string,object[])
		// object UnityEngine.AndroidJavaObject._GetStatic<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject._GetStatic<object>(string)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object UnityEngine.GameObject.GetComponentInParent<object>()
		// object UnityEngine.GameObject.GetComponentInParent<object>(bool)
		// object[] UnityEngine.GameObject.GetComponents<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputAction.ReadValue<UnityEngine.Vector2>()
		// float UnityEngine.InputSystem.InputAction.ReadValue<float>()
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ApplyProcessors<UnityEngine.Vector2>(int,UnityEngine.Vector2,UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>)
		// float UnityEngine.InputSystem.InputActionState.ApplyProcessors<float>(int,float,UnityEngine.InputSystem.InputControl<float>)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ReadValue<UnityEngine.Vector2>(int,int,bool)
		// float UnityEngine.InputSystem.InputActionState.ReadValue<float>(int,int,bool)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Resources.Load<object>(string)
		// object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
		// YooAsset.AssetHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
		// string string.Join<float>(string,System.Collections.Generic.IEnumerable<float>)
		// string string.Join<int>(string,System.Collections.Generic.IEnumerable<int>)
		// string string.Join<object>(string,System.Collections.Generic.IEnumerable<object>)
		// string string.JoinCore<float>(System.Char*,int,System.Collections.Generic.IEnumerable<float>)
		// string string.JoinCore<int>(System.Char*,int,System.Collections.Generic.IEnumerable<int>)
		// string string.JoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
	}
}