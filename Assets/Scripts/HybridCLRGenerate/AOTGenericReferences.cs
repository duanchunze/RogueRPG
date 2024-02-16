using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
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
	// Sirenix.Serialization.Serializer<object>
	// System.Action<Cysharp.Text.Utf16FormatSegment>
	// System.Action<Cysharp.Text.Utf8FormatSegment>
	// System.Action<Hsenl.CasterLeaveDetails>
	// System.Action<Hsenl.StatusFinishDetails>
	// System.Action<MemoryPack.Internal.BufferSegment>
	// System.Action<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Action<System.Threading.CancellationToken>
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
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object,Hsenl.PriorityStateLeaveDetails>
	// System.Action<object,float>
	// System.Action<object,int,Hsenl.Num,Hsenl.Num>
	// System.Action<object,int>
	// System.Action<object,object,object>
	// System.Action<object,object>
	// System.Action<object>
	// System.ArraySegment.Enumerator<System.DateTime>
	// System.ArraySegment.Enumerator<System.DateTimeOffset>
	// System.ArraySegment.Enumerator<System.Decimal>
	// System.ArraySegment.Enumerator<System.Guid>
	// System.ArraySegment.Enumerator<System.IntPtr>
	// System.ArraySegment.Enumerator<System.Numerics.BigInteger>
	// System.ArraySegment.Enumerator<System.Numerics.Complex>
	// System.ArraySegment.Enumerator<System.Numerics.Matrix3x2>
	// System.ArraySegment.Enumerator<System.Numerics.Matrix4x4>
	// System.ArraySegment.Enumerator<System.Numerics.Plane>
	// System.ArraySegment.Enumerator<System.Numerics.Quaternion>
	// System.ArraySegment.Enumerator<System.Numerics.Vector2>
	// System.ArraySegment.Enumerator<System.Numerics.Vector3>
	// System.ArraySegment.Enumerator<System.Numerics.Vector4>
	// System.ArraySegment.Enumerator<System.TimeSpan>
	// System.ArraySegment.Enumerator<System.UIntPtr>
	// System.ArraySegment.Enumerator<UnityEngine.Bounds>
	// System.ArraySegment.Enumerator<UnityEngine.BoundsInt>
	// System.ArraySegment.Enumerator<UnityEngine.Color32>
	// System.ArraySegment.Enumerator<UnityEngine.Color>
	// System.ArraySegment.Enumerator<UnityEngine.GradientAlphaKey>
	// System.ArraySegment.Enumerator<UnityEngine.GradientColorKey>
	// System.ArraySegment.Enumerator<UnityEngine.Keyframe>
	// System.ArraySegment.Enumerator<UnityEngine.LayerMask>
	// System.ArraySegment.Enumerator<UnityEngine.Matrix4x4>
	// System.ArraySegment.Enumerator<UnityEngine.Quaternion>
	// System.ArraySegment.Enumerator<UnityEngine.RangeInt>
	// System.ArraySegment.Enumerator<UnityEngine.Rect>
	// System.ArraySegment.Enumerator<UnityEngine.RectInt>
	// System.ArraySegment.Enumerator<UnityEngine.Vector2>
	// System.ArraySegment.Enumerator<UnityEngine.Vector2Int>
	// System.ArraySegment.Enumerator<UnityEngine.Vector3>
	// System.ArraySegment.Enumerator<UnityEngine.Vector3Int>
	// System.ArraySegment.Enumerator<UnityEngine.Vector4>
	// System.ArraySegment.Enumerator<UnityEngine.jvalue>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment.Enumerator<double>
	// System.ArraySegment.Enumerator<float>
	// System.ArraySegment.Enumerator<int>
	// System.ArraySegment.Enumerator<long>
	// System.ArraySegment.Enumerator<object>
	// System.ArraySegment.Enumerator<sbyte>
	// System.ArraySegment.Enumerator<short>
	// System.ArraySegment.Enumerator<uint>
	// System.ArraySegment.Enumerator<ulong>
	// System.ArraySegment.Enumerator<ushort>
	// System.ArraySegment<System.DateTime>
	// System.ArraySegment<System.DateTimeOffset>
	// System.ArraySegment<System.Decimal>
	// System.ArraySegment<System.Guid>
	// System.ArraySegment<System.IntPtr>
	// System.ArraySegment<System.Numerics.BigInteger>
	// System.ArraySegment<System.Numerics.Complex>
	// System.ArraySegment<System.Numerics.Matrix3x2>
	// System.ArraySegment<System.Numerics.Matrix4x4>
	// System.ArraySegment<System.Numerics.Plane>
	// System.ArraySegment<System.Numerics.Quaternion>
	// System.ArraySegment<System.Numerics.Vector2>
	// System.ArraySegment<System.Numerics.Vector3>
	// System.ArraySegment<System.Numerics.Vector4>
	// System.ArraySegment<System.TimeSpan>
	// System.ArraySegment<System.UIntPtr>
	// System.ArraySegment<UnityEngine.Bounds>
	// System.ArraySegment<UnityEngine.BoundsInt>
	// System.ArraySegment<UnityEngine.Color32>
	// System.ArraySegment<UnityEngine.Color>
	// System.ArraySegment<UnityEngine.GradientAlphaKey>
	// System.ArraySegment<UnityEngine.GradientColorKey>
	// System.ArraySegment<UnityEngine.Keyframe>
	// System.ArraySegment<UnityEngine.LayerMask>
	// System.ArraySegment<UnityEngine.Matrix4x4>
	// System.ArraySegment<UnityEngine.Quaternion>
	// System.ArraySegment<UnityEngine.RangeInt>
	// System.ArraySegment<UnityEngine.Rect>
	// System.ArraySegment<UnityEngine.RectInt>
	// System.ArraySegment<UnityEngine.Vector2>
	// System.ArraySegment<UnityEngine.Vector2Int>
	// System.ArraySegment<UnityEngine.Vector3>
	// System.ArraySegment<UnityEngine.Vector3Int>
	// System.ArraySegment<UnityEngine.Vector4>
	// System.ArraySegment<UnityEngine.jvalue>
	// System.ArraySegment<byte>
	// System.ArraySegment<double>
	// System.ArraySegment<float>
	// System.ArraySegment<int>
	// System.ArraySegment<long>
	// System.ArraySegment<object>
	// System.ArraySegment<sbyte>
	// System.ArraySegment<short>
	// System.ArraySegment<uint>
	// System.ArraySegment<ulong>
	// System.ArraySegment<ushort>
	// System.Buffers.ArrayPool<byte>
	// System.Buffers.ArrayPool<object>
	// System.Buffers.ArrayPool<ushort>
	// System.Buffers.ConfigurableArrayPool.Bucket<byte>
	// System.Buffers.ConfigurableArrayPool.Bucket<object>
	// System.Buffers.ConfigurableArrayPool.Bucket<ushort>
	// System.Buffers.ConfigurableArrayPool<byte>
	// System.Buffers.ConfigurableArrayPool<object>
	// System.Buffers.ConfigurableArrayPool<ushort>
	// System.Buffers.IBufferWriter<byte>
	// System.Buffers.IBufferWriter<object>
	// System.Buffers.IBufferWriter<ushort>
	// System.Buffers.MemoryManager<byte>
	// System.Buffers.MemoryManager<object>
	// System.Buffers.MemoryManager<ushort>
	// System.Buffers.ReadOnlySequence.<>c<byte>
	// System.Buffers.ReadOnlySequence.<>c<object>
	// System.Buffers.ReadOnlySequence.Enumerator<byte>
	// System.Buffers.ReadOnlySequence.Enumerator<object>
	// System.Buffers.ReadOnlySequence<byte>
	// System.Buffers.ReadOnlySequence<object>
	// System.Buffers.ReadOnlySequenceSegment<byte>
	// System.Buffers.ReadOnlySequenceSegment<object>
	// System.Buffers.SpanAction<ushort,System.Buffers.ReadOnlySequence<ushort>>
	// System.Buffers.SpanAction<ushort,System.ValueTuple<System.Buffers.ReadOnlySequence<byte>,long,int,object>>
	// System.Buffers.SpanAction<ushort,System.ValueTuple<System.IntPtr,int,int,object>>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<object>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<ushort>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<object>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<ushort>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<object>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<ushort>
	// System.ByReference<System.DateTime>
	// System.ByReference<System.DateTimeOffset>
	// System.ByReference<System.Decimal>
	// System.ByReference<System.Guid>
	// System.ByReference<System.IntPtr>
	// System.ByReference<System.Numerics.BigInteger>
	// System.ByReference<System.Numerics.Complex>
	// System.ByReference<System.Numerics.Matrix3x2>
	// System.ByReference<System.Numerics.Matrix4x4>
	// System.ByReference<System.Numerics.Plane>
	// System.ByReference<System.Numerics.Quaternion>
	// System.ByReference<System.Numerics.Vector2>
	// System.ByReference<System.Numerics.Vector3>
	// System.ByReference<System.Numerics.Vector4>
	// System.ByReference<System.TimeSpan>
	// System.ByReference<System.UIntPtr>
	// System.ByReference<UnityEngine.Bounds>
	// System.ByReference<UnityEngine.BoundsInt>
	// System.ByReference<UnityEngine.Color32>
	// System.ByReference<UnityEngine.Color>
	// System.ByReference<UnityEngine.GradientAlphaKey>
	// System.ByReference<UnityEngine.GradientColorKey>
	// System.ByReference<UnityEngine.Keyframe>
	// System.ByReference<UnityEngine.LayerMask>
	// System.ByReference<UnityEngine.Matrix4x4>
	// System.ByReference<UnityEngine.Quaternion>
	// System.ByReference<UnityEngine.RangeInt>
	// System.ByReference<UnityEngine.Rect>
	// System.ByReference<UnityEngine.RectInt>
	// System.ByReference<UnityEngine.Vector2>
	// System.ByReference<UnityEngine.Vector2Int>
	// System.ByReference<UnityEngine.Vector3>
	// System.ByReference<UnityEngine.Vector3Int>
	// System.ByReference<UnityEngine.Vector4>
	// System.ByReference<UnityEngine.jvalue>
	// System.ByReference<byte>
	// System.ByReference<double>
	// System.ByReference<float>
	// System.ByReference<int>
	// System.ByReference<long>
	// System.ByReference<object>
	// System.ByReference<sbyte>
	// System.ByReference<short>
	// System.ByReference<uint>
	// System.ByReference<ulong>
	// System.ByReference<ushort>
	// System.Collections.Concurrent.BlockingCollection.<GetConsumingEnumerable>d__68<object>
	// System.Collections.Concurrent.BlockingCollection<object>
	// System.Collections.Concurrent.ConcurrentBag.Enumerator<object>
	// System.Collections.Concurrent.ConcurrentBag.WorkStealingQueue<object>
	// System.Collections.Concurrent.ConcurrentBag<object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Concurrent.ConcurrentStack.<GetEnumerator>d__35<object>
	// System.Collections.Concurrent.ConcurrentStack.Node<object>
	// System.Collections.Concurrent.ConcurrentStack<object>
	// System.Collections.Concurrent.IProducerConsumerCollection<object>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.ArraySortHelper<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ArraySortHelper<System.Threading.CancellationToken>
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
	// System.Collections.Generic.Comparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.Comparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.Comparer<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.Comparer<Hsenl.Num>
	// System.Collections.Generic.Comparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.Comparer<System.Buffers.ReadOnlySequence<byte>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<System.IntPtr>
	// System.Collections.Generic.Comparer<System.Threading.CancellationToken>
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
	// System.Collections.Generic.ComparisonComparer<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.ComparisonComparer<Hsenl.Num>
	// System.Collections.Generic.ComparisonComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ComparisonComparer<System.Buffers.ReadOnlySequence<byte>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ComparisonComparer<System.IntPtr>
	// System.Collections.Generic.ComparisonComparer<System.Threading.CancellationToken>
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
	// System.Collections.Generic.Dictionary.Enumerator<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.Enumerator<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,uint>
	// System.Collections.Generic.Dictionary.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.Enumerator<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,uint>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,uint>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,uint>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,uint>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,object>
	// System.Collections.Generic.Dictionary<int,Hsenl.Num>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,Hsenl.Num>
	// System.Collections.Generic.Dictionary<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<object,uint>
	// System.Collections.Generic.Dictionary<object,ushort>
	// System.Collections.Generic.Dictionary<uint,Hsenl.Num>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.EqualityComparer<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.EqualityComparer<Hsenl.Num>
	// System.Collections.Generic.EqualityComparer<ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.EqualityComparer<System.Buffers.ReadOnlySequence<byte>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.EqualityComparer<System.IntPtr>
	// System.Collections.Generic.EqualityComparer<byte>
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
	// System.Collections.Generic.ICollection<FixedMath.FVector2>
	// System.Collections.Generic.ICollection<FixedMath.FVector3>
	// System.Collections.Generic.ICollection<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.ICollection<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,Hsenl.Num>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Hsenl.Num>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,uint>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.ICollection<System.Threading.CancellationToken>
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
	// System.Collections.Generic.IComparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IComparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<System.Threading.CancellationToken>
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
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IDictionary<uint,object>
	// System.Collections.Generic.IEnumerable<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IEnumerable<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IEnumerable<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.IEnumerable<FixedMath.FVector2>
	// System.Collections.Generic.IEnumerable<FixedMath.FVector3>
	// System.Collections.Generic.IEnumerable<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,Hsenl.Num>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Hsenl.Num>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,uint>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerable<System.Threading.CancellationToken>
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
	// System.Collections.Generic.IEnumerator<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IEnumerator<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IEnumerator<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.IEnumerator<FixedMath.FVector2>
	// System.Collections.Generic.IEnumerator<FixedMath.FVector3>
	// System.Collections.Generic.IEnumerator<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,Hsenl.Num>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Hsenl.Num>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,uint>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerator<System.Memory<byte>>
	// System.Collections.Generic.IEnumerator<System.Threading.CancellationToken>
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
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IEqualityComparer<ushort>
	// System.Collections.Generic.IList<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.IList<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.IList<FixedMath.FVector2>
	// System.Collections.Generic.IList<FixedMath.FVector3>
	// System.Collections.Generic.IList<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.IList<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IList<System.Threading.CancellationToken>
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
	// System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IReadOnlyCollection<object>
	// System.Collections.Generic.IReadOnlyList<object>
	// System.Collections.Generic.ISet<object>
	// System.Collections.Generic.KeyValuePair<System.UIntPtr,object>
	// System.Collections.Generic.KeyValuePair<int,Hsenl.Num>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,Hsenl.Num>
	// System.Collections.Generic.KeyValuePair<object,ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<object,uint>
	// System.Collections.Generic.KeyValuePair<object,ushort>
	// System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.KeyValuePair<ushort,object>
	// System.Collections.Generic.LinkedList.Enumerator<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.LinkedList.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.LinkedList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.LinkedListNode<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.List.Enumerator<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.List.Enumerator<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<System.Threading.CancellationToken>
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
	// System.Collections.Generic.List<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.List<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.List<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List<System.Threading.CancellationToken>
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
	// System.Collections.Generic.ObjectComparer<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.Generic.ObjectComparer<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.Generic.ObjectComparer<Hsenl.MergeSortFloatWrap<object>>
	// System.Collections.Generic.ObjectComparer<Hsenl.Num>
	// System.Collections.Generic.ObjectComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ObjectComparer<System.Buffers.ReadOnlySequence<byte>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<System.IntPtr>
	// System.Collections.Generic.ObjectComparer<System.Threading.CancellationToken>
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
	// System.Collections.Generic.ObjectEqualityComparer<FixedMath.ConcaveHull2.Edge>
	// System.Collections.Generic.ObjectEqualityComparer<Hsenl.Num>
	// System.Collections.Generic.ObjectEqualityComparer<ProtoBuf.Meta.TypeModel.KnownTypeKey>
	// System.Collections.Generic.ObjectEqualityComparer<System.Buffers.ReadOnlySequence<byte>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.IntPtr>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.EventSystemManager.LateUpdateWrap>
	// System.Collections.Generic.Queue.Enumerator<Hsenl.EventSystemManager.UpdateWrap>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<int,long,int>>
	// System.Collections.Generic.Queue.Enumerator<int>
	// System.Collections.Generic.Queue.Enumerator<long>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<Hsenl.EventSystemManager.LateUpdateWrap>
	// System.Collections.Generic.Queue<Hsenl.EventSystemManager.UpdateWrap>
	// System.Collections.Generic.Queue<System.ValueTuple<int,long,int>>
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
	// System.Collections.Generic.SortedList.Enumerator<object,object>
	// System.Collections.Generic.SortedList.KeyList<object,object>
	// System.Collections.Generic.SortedList.SortedListKeyEnumerator<object,object>
	// System.Collections.Generic.SortedList.SortedListValueEnumerator<object,object>
	// System.Collections.Generic.SortedList.ValueList<object,object>
	// System.Collections.Generic.SortedList<object,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<object>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<object>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Enumerator<object>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Node<object>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<object>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<object>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet<object>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<object>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.TreeWalkPredicate<object>
	// System.Collections.ObjectModel.Collection<object>
	// System.Collections.ObjectModel.ObservableCollection.SimpleMonitor<object>
	// System.Collections.ObjectModel.ObservableCollection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Text.Utf16FormatSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Text.Utf8FormatSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<MemoryPack.Internal.BufferSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Threading.CancellationToken>
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
	// System.Collections.ObjectModel.ReadOnlyObservableCollection<object>
	// System.Comparison<Cysharp.Text.Utf16FormatSegment>
	// System.Comparison<Cysharp.Text.Utf8FormatSegment>
	// System.Comparison<Hsenl.MergeSortFloatWrap<object>>
	// System.Comparison<Hsenl.Num>
	// System.Comparison<MemoryPack.Internal.BufferSegment>
	// System.Comparison<System.Buffers.ReadOnlySequence<byte>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Comparison<System.IntPtr>
	// System.Comparison<System.Threading.CancellationToken>
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
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,byte>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<System.ValueTuple<ushort,object>,object>
	// System.Func<System.ValueTuple<ushort,object>,ushort>
	// System.Func<byte>
	// System.Func<float,float>
	// System.Func<float,int>
	// System.Func<int,object,object>
	// System.Func<int>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,byte>
	// System.Func<object,int,int,object>
	// System.Func<object,int>
	// System.Func<object,object,byte>
	// System.Func<object,object,object,object>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IComparable<FixedMath.Fixp>
	// System.IComparable<Hsenl.MergeSortFloatWrap<object>>
	// System.IComparable<object>
	// System.IEquatable<FixedMath.FMatrix3x3>
	// System.IEquatable<FixedMath.FMatrix4x4>
	// System.IEquatable<FixedMath.FVector2>
	// System.IEquatable<FixedMath.FVector3>
	// System.IEquatable<FixedMath.FVector4>
	// System.IEquatable<FixedMath.Fixp>
	// System.IEquatable<object>
	// System.IEquatable<ushort>
	// System.Lazy<object>
	// System.Linq.Buffer<int>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<ConcatIterator>d__59<object>
	// System.Linq.Enumerable.<DistinctIterator>d__68<object>
	// System.Linq.Enumerable.<PrependIterator>d__63<object>
	// System.Linq.Enumerable.<ReverseIterator>d__79<object>
	// System.Linq.Enumerable.<SelectManyIterator>d__17<object,object>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.IGrouping<object,object>
	// System.Linq.ILookup<object,object>
	// System.Linq.Parallel.ParallelEnumerableWrapper<uint>
	// System.Linq.ParallelQuery<uint>
	// System.Linq.Set<object>
	// System.Memory<byte>
	// System.Memory<object>
	// System.Memory<ushort>
	// System.Nullable<System.DateTime>
	// System.Nullable<System.DateTimeOffset>
	// System.Nullable<System.Decimal>
	// System.Nullable<System.Guid>
	// System.Nullable<System.IntPtr>
	// System.Nullable<System.Numerics.Complex>
	// System.Nullable<System.Numerics.Matrix3x2>
	// System.Nullable<System.Numerics.Matrix4x4>
	// System.Nullable<System.Numerics.Plane>
	// System.Nullable<System.Numerics.Quaternion>
	// System.Nullable<System.Numerics.Vector2>
	// System.Nullable<System.Numerics.Vector3>
	// System.Nullable<System.Numerics.Vector4>
	// System.Nullable<System.TimeSpan>
	// System.Nullable<System.UIntPtr>
	// System.Nullable<Unity.Mathematics.float3>
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
	// System.Predicate<MemoryPack.Internal.BufferSegment>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Predicate<System.Threading.CancellationToken>
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
	// System.ReadOnlyMemory<byte>
	// System.ReadOnlyMemory<object>
	// System.ReadOnlyMemory<ushort>
	// System.ReadOnlySpan.Enumerator<System.DateTime>
	// System.ReadOnlySpan.Enumerator<System.DateTimeOffset>
	// System.ReadOnlySpan.Enumerator<System.Decimal>
	// System.ReadOnlySpan.Enumerator<System.Guid>
	// System.ReadOnlySpan.Enumerator<System.IntPtr>
	// System.ReadOnlySpan.Enumerator<System.Numerics.BigInteger>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Complex>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Matrix3x2>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Matrix4x4>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Plane>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Quaternion>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Vector2>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Vector3>
	// System.ReadOnlySpan.Enumerator<System.Numerics.Vector4>
	// System.ReadOnlySpan.Enumerator<System.TimeSpan>
	// System.ReadOnlySpan.Enumerator<System.UIntPtr>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Bounds>
	// System.ReadOnlySpan.Enumerator<UnityEngine.BoundsInt>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Color32>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Color>
	// System.ReadOnlySpan.Enumerator<UnityEngine.GradientAlphaKey>
	// System.ReadOnlySpan.Enumerator<UnityEngine.GradientColorKey>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Keyframe>
	// System.ReadOnlySpan.Enumerator<UnityEngine.LayerMask>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Matrix4x4>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Quaternion>
	// System.ReadOnlySpan.Enumerator<UnityEngine.RangeInt>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Rect>
	// System.ReadOnlySpan.Enumerator<UnityEngine.RectInt>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector2>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector2Int>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector3>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector3Int>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector4>
	// System.ReadOnlySpan.Enumerator<UnityEngine.jvalue>
	// System.ReadOnlySpan.Enumerator<byte>
	// System.ReadOnlySpan.Enumerator<double>
	// System.ReadOnlySpan.Enumerator<float>
	// System.ReadOnlySpan.Enumerator<int>
	// System.ReadOnlySpan.Enumerator<long>
	// System.ReadOnlySpan.Enumerator<object>
	// System.ReadOnlySpan.Enumerator<sbyte>
	// System.ReadOnlySpan.Enumerator<short>
	// System.ReadOnlySpan.Enumerator<uint>
	// System.ReadOnlySpan.Enumerator<ulong>
	// System.ReadOnlySpan.Enumerator<ushort>
	// System.ReadOnlySpan<System.DateTime>
	// System.ReadOnlySpan<System.DateTimeOffset>
	// System.ReadOnlySpan<System.Decimal>
	// System.ReadOnlySpan<System.Guid>
	// System.ReadOnlySpan<System.IntPtr>
	// System.ReadOnlySpan<System.Numerics.BigInteger>
	// System.ReadOnlySpan<System.Numerics.Complex>
	// System.ReadOnlySpan<System.Numerics.Matrix3x2>
	// System.ReadOnlySpan<System.Numerics.Matrix4x4>
	// System.ReadOnlySpan<System.Numerics.Plane>
	// System.ReadOnlySpan<System.Numerics.Quaternion>
	// System.ReadOnlySpan<System.Numerics.Vector2>
	// System.ReadOnlySpan<System.Numerics.Vector3>
	// System.ReadOnlySpan<System.Numerics.Vector4>
	// System.ReadOnlySpan<System.TimeSpan>
	// System.ReadOnlySpan<System.UIntPtr>
	// System.ReadOnlySpan<UnityEngine.Bounds>
	// System.ReadOnlySpan<UnityEngine.BoundsInt>
	// System.ReadOnlySpan<UnityEngine.Color32>
	// System.ReadOnlySpan<UnityEngine.Color>
	// System.ReadOnlySpan<UnityEngine.GradientAlphaKey>
	// System.ReadOnlySpan<UnityEngine.GradientColorKey>
	// System.ReadOnlySpan<UnityEngine.Keyframe>
	// System.ReadOnlySpan<UnityEngine.LayerMask>
	// System.ReadOnlySpan<UnityEngine.Matrix4x4>
	// System.ReadOnlySpan<UnityEngine.Quaternion>
	// System.ReadOnlySpan<UnityEngine.RangeInt>
	// System.ReadOnlySpan<UnityEngine.Rect>
	// System.ReadOnlySpan<UnityEngine.RectInt>
	// System.ReadOnlySpan<UnityEngine.Vector2>
	// System.ReadOnlySpan<UnityEngine.Vector2Int>
	// System.ReadOnlySpan<UnityEngine.Vector3>
	// System.ReadOnlySpan<UnityEngine.Vector3Int>
	// System.ReadOnlySpan<UnityEngine.Vector4>
	// System.ReadOnlySpan<UnityEngine.jvalue>
	// System.ReadOnlySpan<byte>
	// System.ReadOnlySpan<double>
	// System.ReadOnlySpan<float>
	// System.ReadOnlySpan<int>
	// System.ReadOnlySpan<long>
	// System.ReadOnlySpan<object>
	// System.ReadOnlySpan<sbyte>
	// System.ReadOnlySpan<short>
	// System.ReadOnlySpan<uint>
	// System.ReadOnlySpan<ulong>
	// System.ReadOnlySpan<ushort>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<object>
	// System.Runtime.CompilerServices.StrongBox<int>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<int>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<object>
	// System.Span.Enumerator<System.DateTime>
	// System.Span.Enumerator<System.DateTimeOffset>
	// System.Span.Enumerator<System.Decimal>
	// System.Span.Enumerator<System.Guid>
	// System.Span.Enumerator<System.IntPtr>
	// System.Span.Enumerator<System.Numerics.BigInteger>
	// System.Span.Enumerator<System.Numerics.Complex>
	// System.Span.Enumerator<System.Numerics.Matrix3x2>
	// System.Span.Enumerator<System.Numerics.Matrix4x4>
	// System.Span.Enumerator<System.Numerics.Plane>
	// System.Span.Enumerator<System.Numerics.Quaternion>
	// System.Span.Enumerator<System.Numerics.Vector2>
	// System.Span.Enumerator<System.Numerics.Vector3>
	// System.Span.Enumerator<System.Numerics.Vector4>
	// System.Span.Enumerator<System.TimeSpan>
	// System.Span.Enumerator<System.UIntPtr>
	// System.Span.Enumerator<UnityEngine.Bounds>
	// System.Span.Enumerator<UnityEngine.BoundsInt>
	// System.Span.Enumerator<UnityEngine.Color32>
	// System.Span.Enumerator<UnityEngine.Color>
	// System.Span.Enumerator<UnityEngine.GradientAlphaKey>
	// System.Span.Enumerator<UnityEngine.GradientColorKey>
	// System.Span.Enumerator<UnityEngine.Keyframe>
	// System.Span.Enumerator<UnityEngine.LayerMask>
	// System.Span.Enumerator<UnityEngine.Matrix4x4>
	// System.Span.Enumerator<UnityEngine.Quaternion>
	// System.Span.Enumerator<UnityEngine.RangeInt>
	// System.Span.Enumerator<UnityEngine.Rect>
	// System.Span.Enumerator<UnityEngine.RectInt>
	// System.Span.Enumerator<UnityEngine.Vector2>
	// System.Span.Enumerator<UnityEngine.Vector2Int>
	// System.Span.Enumerator<UnityEngine.Vector3>
	// System.Span.Enumerator<UnityEngine.Vector3Int>
	// System.Span.Enumerator<UnityEngine.Vector4>
	// System.Span.Enumerator<UnityEngine.jvalue>
	// System.Span.Enumerator<byte>
	// System.Span.Enumerator<double>
	// System.Span.Enumerator<float>
	// System.Span.Enumerator<int>
	// System.Span.Enumerator<long>
	// System.Span.Enumerator<object>
	// System.Span.Enumerator<sbyte>
	// System.Span.Enumerator<short>
	// System.Span.Enumerator<uint>
	// System.Span.Enumerator<ulong>
	// System.Span.Enumerator<ushort>
	// System.Span<System.DateTime>
	// System.Span<System.DateTimeOffset>
	// System.Span<System.Decimal>
	// System.Span<System.Guid>
	// System.Span<System.IntPtr>
	// System.Span<System.Numerics.BigInteger>
	// System.Span<System.Numerics.Complex>
	// System.Span<System.Numerics.Matrix3x2>
	// System.Span<System.Numerics.Matrix4x4>
	// System.Span<System.Numerics.Plane>
	// System.Span<System.Numerics.Quaternion>
	// System.Span<System.Numerics.Vector2>
	// System.Span<System.Numerics.Vector3>
	// System.Span<System.Numerics.Vector4>
	// System.Span<System.TimeSpan>
	// System.Span<System.UIntPtr>
	// System.Span<UnityEngine.Bounds>
	// System.Span<UnityEngine.BoundsInt>
	// System.Span<UnityEngine.Color32>
	// System.Span<UnityEngine.Color>
	// System.Span<UnityEngine.GradientAlphaKey>
	// System.Span<UnityEngine.GradientColorKey>
	// System.Span<UnityEngine.Keyframe>
	// System.Span<UnityEngine.LayerMask>
	// System.Span<UnityEngine.Matrix4x4>
	// System.Span<UnityEngine.Quaternion>
	// System.Span<UnityEngine.RangeInt>
	// System.Span<UnityEngine.Rect>
	// System.Span<UnityEngine.RectInt>
	// System.Span<UnityEngine.Vector2>
	// System.Span<UnityEngine.Vector2Int>
	// System.Span<UnityEngine.Vector3>
	// System.Span<UnityEngine.Vector3Int>
	// System.Span<UnityEngine.Vector4>
	// System.Span<UnityEngine.jvalue>
	// System.Span<byte>
	// System.Span<double>
	// System.Span<float>
	// System.Span<int>
	// System.Span<long>
	// System.Span<object>
	// System.Span<sbyte>
	// System.Span<short>
	// System.Span<uint>
	// System.Span<ulong>
	// System.Span<ushort>
	// System.Threading.Tasks.Sources.IValueTaskSource<int>
	// System.Threading.Tasks.Sources.IValueTaskSource<object>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<int>
	// System.Threading.Tasks.TaskFactory<object>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<int>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<object>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<int>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<object>
	// System.Threading.Tasks.ValueTask<int>
	// System.Threading.Tasks.ValueTask<object>
	// System.Threading.ThreadLocal.FinalizationHelper<object>
	// System.Threading.ThreadLocal.IdManager<object>
	// System.Threading.ThreadLocal.LinkedSlot<object>
	// System.Threading.ThreadLocal<object>
	// System.Tuple<object,object,object,object,object,object,object,object>
	// System.Tuple<object,object,object,object,object,object,object>
	// System.Tuple<object,object,object,object,object,object>
	// System.Tuple<object,object,object,object,object>
	// System.Tuple<object,object,object,object>
	// System.Tuple<object,object,object>
	// System.Tuple<object,object>
	// System.Tuple<object>
	// System.ValueTuple<Hsenl.Num,Hsenl.Num>
	// System.ValueTuple<System.Buffers.ReadOnlySequence<byte>,long,int,object>
	// System.ValueTuple<System.IntPtr,int,int,object>
	// System.ValueTuple<byte,uint>
	// System.ValueTuple<int,int>
	// System.ValueTuple<int,long,int>
	// System.ValueTuple<object,int>
	// System.ValueTuple<object,object,object,object,object,object,object,object>
	// System.ValueTuple<object,object,object,object,object,object,object>
	// System.ValueTuple<object,object,object,object,object,object>
	// System.ValueTuple<object,object,object,object,object>
	// System.ValueTuple<object,object,object,object>
	// System.ValueTuple<object,object,object>
	// System.ValueTuple<object,object>
	// System.ValueTuple<object>
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
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(Sirenix.Serialization.IDataReader)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(System.IO.Stream,Sirenix.Serialization.DataFormat,Sirenix.Serialization.DeserializationContext)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(byte[],Sirenix.Serialization.DataFormat,Sirenix.Serialization.DeserializationContext)
		// System.Void Sirenix.Serialization.SerializationUtility.SerializeValue<object>(object,Sirenix.Serialization.IDataWriter)
		// System.Void Sirenix.Serialization.SerializationUtility.SerializeValue<object>(object,System.IO.Stream,Sirenix.Serialization.DataFormat,Sirenix.Serialization.SerializationContext)
		// byte[] Sirenix.Serialization.SerializationUtility.SerializeValue<object>(object,Sirenix.Serialization.DataFormat,Sirenix.Serialization.SerializationContext)
		// Sirenix.Serialization.Serializer<object> Sirenix.Serialization.Serializer.Get<object>()
		// object System.Activator.CreateInstance<object>()
		// System.DateTimeOffset[] System.Array.Empty<System.DateTimeOffset>()
		// System.DateTime[] System.Array.Empty<System.DateTime>()
		// System.Decimal[] System.Array.Empty<System.Decimal>()
		// System.Guid[] System.Array.Empty<System.Guid>()
		// System.IntPtr[] System.Array.Empty<System.IntPtr>()
		// System.Numerics.BigInteger[] System.Array.Empty<System.Numerics.BigInteger>()
		// System.Numerics.Complex[] System.Array.Empty<System.Numerics.Complex>()
		// System.Numerics.Matrix3x2[] System.Array.Empty<System.Numerics.Matrix3x2>()
		// System.Numerics.Matrix4x4[] System.Array.Empty<System.Numerics.Matrix4x4>()
		// System.Numerics.Plane[] System.Array.Empty<System.Numerics.Plane>()
		// System.Numerics.Quaternion[] System.Array.Empty<System.Numerics.Quaternion>()
		// System.Numerics.Vector2[] System.Array.Empty<System.Numerics.Vector2>()
		// System.Numerics.Vector3[] System.Array.Empty<System.Numerics.Vector3>()
		// System.Numerics.Vector4[] System.Array.Empty<System.Numerics.Vector4>()
		// System.TimeSpan[] System.Array.Empty<System.TimeSpan>()
		// System.UIntPtr[] System.Array.Empty<System.UIntPtr>()
		// UnityEngine.BoundsInt[] System.Array.Empty<UnityEngine.BoundsInt>()
		// UnityEngine.Bounds[] System.Array.Empty<UnityEngine.Bounds>()
		// UnityEngine.Color32[] System.Array.Empty<UnityEngine.Color32>()
		// UnityEngine.Color[] System.Array.Empty<UnityEngine.Color>()
		// UnityEngine.GradientAlphaKey[] System.Array.Empty<UnityEngine.GradientAlphaKey>()
		// UnityEngine.GradientColorKey[] System.Array.Empty<UnityEngine.GradientColorKey>()
		// UnityEngine.Keyframe[] System.Array.Empty<UnityEngine.Keyframe>()
		// UnityEngine.LayerMask[] System.Array.Empty<UnityEngine.LayerMask>()
		// UnityEngine.Matrix4x4[] System.Array.Empty<UnityEngine.Matrix4x4>()
		// UnityEngine.Quaternion[] System.Array.Empty<UnityEngine.Quaternion>()
		// UnityEngine.RangeInt[] System.Array.Empty<UnityEngine.RangeInt>()
		// UnityEngine.RectInt[] System.Array.Empty<UnityEngine.RectInt>()
		// UnityEngine.Rect[] System.Array.Empty<UnityEngine.Rect>()
		// UnityEngine.Vector2Int[] System.Array.Empty<UnityEngine.Vector2Int>()
		// UnityEngine.Vector2[] System.Array.Empty<UnityEngine.Vector2>()
		// UnityEngine.Vector3Int[] System.Array.Empty<UnityEngine.Vector3Int>()
		// UnityEngine.Vector3[] System.Array.Empty<UnityEngine.Vector3>()
		// UnityEngine.Vector4[] System.Array.Empty<UnityEngine.Vector4>()
		// byte[] System.Array.Empty<byte>()
		// double[] System.Array.Empty<double>()
		// float[] System.Array.Empty<float>()
		// int[] System.Array.Empty<int>()
		// long[] System.Array.Empty<long>()
		// object[] System.Array.Empty<object>()
		// sbyte[] System.Array.Empty<sbyte>()
		// short[] System.Array.Empty<short>()
		// uint[] System.Array.Empty<uint>()
		// ulong[] System.Array.Empty<ulong>()
		// ushort[] System.Array.Empty<ushort>()
		// int System.Array.IndexOf<object>(object[],object)
		// int System.Array.IndexOfImpl<object>(object[],object,int,int)
		// System.Void System.Array.Reverse<object>(object[])
		// System.Void System.Array.Reverse<object>(object[],int,int)
		// System.Void System.Array.Sort<object>(object[])
		// System.Void System.Array.Sort<object>(object[],System.Collections.Generic.IComparer<object>)
		// System.Void System.Array.Sort<object>(object[],System.Comparison<object>)
		// System.Void System.Array.Sort<object>(object[],int,int,System.Collections.Generic.IComparer<object>)
		// System.Void System.Buffers.BuffersExtensions.CopyTo<byte>(System.Buffers.ReadOnlySequence<byte>&,System.Span<byte>)
		// System.Void System.Buffers.BuffersExtensions.CopyToMultiSegment<byte>(System.Buffers.ReadOnlySequence<byte>&,System.Span<byte>)
		// int System.Enum.Parse<int>(string)
		// int System.Enum.Parse<int>(string,bool)
		// bool System.Enum.TryParse<int>(string,bool,int&)
		// bool System.Enum.TryParse<int>(string,int&)
		// int System.HashCode.Combine<int,int,int,int,int>(int,int,int,int,int)
		// int System.HashCode.Combine<int,int,int,int>(int,int,int,int)
		// int System.HashCode.Combine<int,int,int>(int,int,int)
		// int System.HashCode.Combine<int,int>(int,int)
		// int System.HashCode.Combine<long,byte>(long,byte)
		// int System.HashCode.Combine<object,int>(object,int)
		// int System.HashCode.Combine<object,object>(object,object)
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
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Prepend<object>(System.Collections.Generic.IEnumerable<object>,object)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.PrependIterator<object>(System.Collections.Generic.IEnumerable<object>,object)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Reverse<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.ReverseIterator<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectMany<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectManyIterator<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// int[] System.Linq.Enumerable.ToArray<int>(System.Collections.Generic.IEnumerable<int>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.Dictionary<object,ushort> System.Linq.Enumerable.ToDictionary<System.ValueTuple<ushort,object>,object,ushort>(System.Collections.Generic.IEnumerable<System.ValueTuple<ushort,object>>,System.Func<System.ValueTuple<ushort,object>,object>,System.Func<System.ValueTuple<ushort,object>,ushort>)
		// System.Collections.Generic.Dictionary<object,ushort> System.Linq.Enumerable.ToDictionary<System.ValueTuple<ushort,object>,object,ushort>(System.Collections.Generic.IEnumerable<System.ValueTuple<ushort,object>>,System.Func<System.ValueTuple<ushort,object>,object>,System.Func<System.ValueTuple<ushort,object>,ushort>,System.Collections.Generic.IEqualityComparer<object>)
		// System.Collections.Generic.Dictionary<ushort,object> System.Linq.Enumerable.ToDictionary<System.ValueTuple<ushort,object>,ushort,object>(System.Collections.Generic.IEnumerable<System.ValueTuple<ushort,object>>,System.Func<System.ValueTuple<ushort,object>,ushort>,System.Func<System.ValueTuple<ushort,object>,object>)
		// System.Collections.Generic.Dictionary<ushort,object> System.Linq.Enumerable.ToDictionary<System.ValueTuple<ushort,object>,ushort,object>(System.Collections.Generic.IEnumerable<System.ValueTuple<ushort,object>>,System.Func<System.ValueTuple<ushort,object>,ushort>,System.Func<System.ValueTuple<ushort,object>,object>,System.Collections.Generic.IEqualityComparer<ushort>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Linq.ParallelQuery<uint> System.Linq.ParallelEnumerable.AsParallel<uint>(System.Collections.Generic.IEnumerable<uint>)
		// System.Memory<byte> System.MemoryExtensions.AsMemory<byte>(byte[],int)
		// System.Memory<byte> System.MemoryExtensions.AsMemory<byte>(byte[],int,int)
		// System.Memory<object> System.MemoryExtensions.AsMemory<object>(System.ArraySegment<object>)
		// System.Memory<object> System.MemoryExtensions.AsMemory<object>(object[],int,int)
		// System.Memory<ushort> System.MemoryExtensions.AsMemory<ushort>(ushort[],int)
		// System.Memory<ushort> System.MemoryExtensions.AsMemory<ushort>(ushort[],int,int)
		// System.Span<System.DateTime> System.MemoryExtensions.AsSpan<System.DateTime>(System.DateTime[])
		// System.Span<System.DateTimeOffset> System.MemoryExtensions.AsSpan<System.DateTimeOffset>(System.DateTimeOffset[])
		// System.Span<System.Decimal> System.MemoryExtensions.AsSpan<System.Decimal>(System.Decimal[])
		// System.Span<System.Guid> System.MemoryExtensions.AsSpan<System.Guid>(System.Guid[])
		// System.Span<System.IntPtr> System.MemoryExtensions.AsSpan<System.IntPtr>(System.IntPtr[])
		// System.Span<System.Numerics.BigInteger> System.MemoryExtensions.AsSpan<System.Numerics.BigInteger>(System.Numerics.BigInteger[])
		// System.Span<System.Numerics.Complex> System.MemoryExtensions.AsSpan<System.Numerics.Complex>(System.Numerics.Complex[])
		// System.Span<System.Numerics.Matrix3x2> System.MemoryExtensions.AsSpan<System.Numerics.Matrix3x2>(System.Numerics.Matrix3x2[])
		// System.Span<System.Numerics.Matrix4x4> System.MemoryExtensions.AsSpan<System.Numerics.Matrix4x4>(System.Numerics.Matrix4x4[])
		// System.Span<System.Numerics.Plane> System.MemoryExtensions.AsSpan<System.Numerics.Plane>(System.Numerics.Plane[])
		// System.Span<System.Numerics.Quaternion> System.MemoryExtensions.AsSpan<System.Numerics.Quaternion>(System.Numerics.Quaternion[])
		// System.Span<System.Numerics.Vector2> System.MemoryExtensions.AsSpan<System.Numerics.Vector2>(System.Numerics.Vector2[])
		// System.Span<System.Numerics.Vector3> System.MemoryExtensions.AsSpan<System.Numerics.Vector3>(System.Numerics.Vector3[])
		// System.Span<System.Numerics.Vector4> System.MemoryExtensions.AsSpan<System.Numerics.Vector4>(System.Numerics.Vector4[])
		// System.Span<System.TimeSpan> System.MemoryExtensions.AsSpan<System.TimeSpan>(System.TimeSpan[])
		// System.Span<System.UIntPtr> System.MemoryExtensions.AsSpan<System.UIntPtr>(System.UIntPtr[])
		// System.Span<UnityEngine.Bounds> System.MemoryExtensions.AsSpan<UnityEngine.Bounds>(UnityEngine.Bounds[])
		// System.Span<UnityEngine.BoundsInt> System.MemoryExtensions.AsSpan<UnityEngine.BoundsInt>(UnityEngine.BoundsInt[])
		// System.Span<UnityEngine.Color32> System.MemoryExtensions.AsSpan<UnityEngine.Color32>(UnityEngine.Color32[])
		// System.Span<UnityEngine.Color> System.MemoryExtensions.AsSpan<UnityEngine.Color>(UnityEngine.Color[])
		// System.Span<UnityEngine.GradientAlphaKey> System.MemoryExtensions.AsSpan<UnityEngine.GradientAlphaKey>(UnityEngine.GradientAlphaKey[])
		// System.Span<UnityEngine.GradientColorKey> System.MemoryExtensions.AsSpan<UnityEngine.GradientColorKey>(UnityEngine.GradientColorKey[])
		// System.Span<UnityEngine.Keyframe> System.MemoryExtensions.AsSpan<UnityEngine.Keyframe>(UnityEngine.Keyframe[])
		// System.Span<UnityEngine.LayerMask> System.MemoryExtensions.AsSpan<UnityEngine.LayerMask>(UnityEngine.LayerMask[])
		// System.Span<UnityEngine.Matrix4x4> System.MemoryExtensions.AsSpan<UnityEngine.Matrix4x4>(UnityEngine.Matrix4x4[])
		// System.Span<UnityEngine.Quaternion> System.MemoryExtensions.AsSpan<UnityEngine.Quaternion>(UnityEngine.Quaternion[])
		// System.Span<UnityEngine.RangeInt> System.MemoryExtensions.AsSpan<UnityEngine.RangeInt>(UnityEngine.RangeInt[])
		// System.Span<UnityEngine.Rect> System.MemoryExtensions.AsSpan<UnityEngine.Rect>(UnityEngine.Rect[])
		// System.Span<UnityEngine.RectInt> System.MemoryExtensions.AsSpan<UnityEngine.RectInt>(UnityEngine.RectInt[])
		// System.Span<UnityEngine.Vector2> System.MemoryExtensions.AsSpan<UnityEngine.Vector2>(UnityEngine.Vector2[])
		// System.Span<UnityEngine.Vector2Int> System.MemoryExtensions.AsSpan<UnityEngine.Vector2Int>(UnityEngine.Vector2Int[])
		// System.Span<UnityEngine.Vector3> System.MemoryExtensions.AsSpan<UnityEngine.Vector3>(UnityEngine.Vector3[])
		// System.Span<UnityEngine.Vector3Int> System.MemoryExtensions.AsSpan<UnityEngine.Vector3Int>(UnityEngine.Vector3Int[])
		// System.Span<UnityEngine.Vector4> System.MemoryExtensions.AsSpan<UnityEngine.Vector4>(UnityEngine.Vector4[])
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(System.ArraySegment<byte>,int)
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[])
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[],int)
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[],int,int)
		// System.Span<double> System.MemoryExtensions.AsSpan<double>(double[])
		// System.Span<float> System.MemoryExtensions.AsSpan<float>(float[])
		// System.Span<int> System.MemoryExtensions.AsSpan<int>(int[])
		// System.Span<long> System.MemoryExtensions.AsSpan<long>(long[])
		// System.Span<object> System.MemoryExtensions.AsSpan<object>(object[])
		// System.Span<sbyte> System.MemoryExtensions.AsSpan<sbyte>(sbyte[])
		// System.Span<short> System.MemoryExtensions.AsSpan<short>(short[])
		// System.Span<uint> System.MemoryExtensions.AsSpan<uint>(uint[])
		// System.Span<ulong> System.MemoryExtensions.AsSpan<ulong>(ulong[])
		// System.Span<ushort> System.MemoryExtensions.AsSpan<ushort>(ushort[])
		// System.Span<ushort> System.MemoryExtensions.AsSpan<ushort>(ushort[],int)
		// System.Span<ushort> System.MemoryExtensions.AsSpan<ushort>(ushort[],int,int)
		// bool System.MemoryExtensions.IsTypeComparableAsBytes<ushort>(ulong&)
		// bool System.MemoryExtensions.StartsWith<ushort>(System.ReadOnlySpan<ushort>,System.ReadOnlySpan<ushort>)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo,bool)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>&,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>&,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11>(MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19>(MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9>(MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15>(MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>>(MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter&,MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>&,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>>(System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>&,MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.Start<MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11>(MemoryPack.Compression.BrotliCompressor.<CopyToAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.Start<MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19>(MemoryPack.Internal.ReusableLinkedArrayBufferWriter.<WriteToAndResetAsync>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.Start<MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>>(MemoryPack.MemoryPackSerializer.<SerializeAsync>d__21<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder.Start<MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9>(MemoryPack.MemoryPackSerializer.<SerializeAsync>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder<object>.Start<MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15>(MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder<object>.Start<MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>>(MemoryPack.MemoryPackSerializer.<DeserializeAsync>d__5<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Hsenl.ETTaskCompleted,Hsenl.ProcedurePreloadAssets.<OnEnter>d__0>(Hsenl.ETTaskCompleted&,Hsenl.ProcedurePreloadAssets.<OnEnter>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.AdvDefaultCheckpoints.<OnSceneLoaded>d__7>(object&,Hsenl.AdvDefaultCheckpoints.<OnSceneLoaded>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ETTaskHelper.<>c__DisplayClass1_0.<<WaitAny>g__Run|0>d<object>>(object&,Hsenl.ETTaskHelper.<>c__DisplayClass1_0.<<WaitAny>g__Run|0>d<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ETTaskHelper.<>c__DisplayClass2_0.<<WaitAny>g__Run|0>d>(object&,Hsenl.ETTaskHelper.<>c__DisplayClass2_0.<<WaitAny>g__Run|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ETTaskHelper.<>c__DisplayClass3_0.<<WaitAll>g__Run|0>d<object>>(object&,Hsenl.ETTaskHelper.<>c__DisplayClass3_0.<<WaitAll>g__Run|0>d<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ETTaskHelper.<>c__DisplayClass4_0.<<WaitAll>g__Run|0>d<object>>(object&,Hsenl.ETTaskHelper.<>c__DisplayClass4_0.<<WaitAll>g__Run|0>d<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ETTaskHelper.<>c__DisplayClass5_0.<<WaitAll>g__Run|0>d>(object&,Hsenl.ETTaskHelper.<>c__DisplayClass5_0.<<WaitAll>g__Run|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ETTaskHelper.<>c__DisplayClass6_0.<<WaitAll>g__Run|0>d>(object&,Hsenl.ETTaskHelper.<>c__DisplayClass6_0.<<WaitAll>g__Run|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.MonoTimer.<TimeStart>d__0>(object&,Hsenl.MonoTimer.<TimeStart>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.Pickable.<LoadCollider>d__7>(object&,Hsenl.Pickable.<LoadCollider>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ProcedureChangeScene.<OnEnter>d__0>(object&,Hsenl.ProcedureChangeScene.<OnEnter>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.ProcedurePreprocessing.<OnEnter>d__0>(object&,Hsenl.ProcedurePreprocessing.<OnEnter>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d>(object&,Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.TpDie.<OnTimePointTrigger>d__0>(object&,Hsenl.TpDie.<OnTimePointTrigger>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<object,Hsenl.View.UIJumpMessage.<WriteText>d__10>(object&,Hsenl.View.UIJumpMessage.<WriteText>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.AdvDefaultCheckpoints.<OnSceneLoaded>d__7>(Hsenl.AdvDefaultCheckpoints.<OnSceneLoaded>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ETTaskHelper.<>c__DisplayClass1_0.<<WaitAny>g__Run|0>d<object>>(Hsenl.ETTaskHelper.<>c__DisplayClass1_0.<<WaitAny>g__Run|0>d<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ETTaskHelper.<>c__DisplayClass2_0.<<WaitAny>g__Run|0>d>(Hsenl.ETTaskHelper.<>c__DisplayClass2_0.<<WaitAny>g__Run|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ETTaskHelper.<>c__DisplayClass3_0.<<WaitAll>g__Run|0>d<object>>(Hsenl.ETTaskHelper.<>c__DisplayClass3_0.<<WaitAll>g__Run|0>d<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ETTaskHelper.<>c__DisplayClass4_0.<<WaitAll>g__Run|0>d<object>>(Hsenl.ETTaskHelper.<>c__DisplayClass4_0.<<WaitAll>g__Run|0>d<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ETTaskHelper.<>c__DisplayClass5_0.<<WaitAll>g__Run|0>d>(Hsenl.ETTaskHelper.<>c__DisplayClass5_0.<<WaitAll>g__Run|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ETTaskHelper.<>c__DisplayClass6_0.<<WaitAll>g__Run|0>d>(Hsenl.ETTaskHelper.<>c__DisplayClass6_0.<<WaitAll>g__Run|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.MonoTimer.<TimeStart>d__0>(Hsenl.MonoTimer.<TimeStart>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.Pickable.<LoadCollider>d__7>(Hsenl.Pickable.<LoadCollider>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ProcedureChangeScene.<OnEnter>d__0>(Hsenl.ProcedureChangeScene.<OnEnter>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ProcedurePreloadAssets.<OnEnter>d__0>(Hsenl.ProcedurePreloadAssets.<OnEnter>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.ProcedurePreprocessing.<OnEnter>d__0>(Hsenl.ProcedurePreprocessing.<OnEnter>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d>(Hsenl.TaskLockQueue.<>c__DisplayClass7_0.<<Wait>g__CheckTimeOut|0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.TpDie.<OnTimePointTrigger>d__0>(Hsenl.TpDie.<OnTimePointTrigger>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Hsenl.View.UIJumpMessage.<WriteText>d__10>(Hsenl.View.UIJumpMessage.<WriteText>d__10&)
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.Collections.Generic.KeyValuePair<int,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.Collections.Generic.KeyValuePair<object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.Collections.Generic.KeyValuePair<uint,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.Numerics.BigInteger>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object,object,object,object,object,object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object,object,object,object,object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object,object,object,object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object,object,object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object,object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.ValueTuple<object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<int>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<object>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<ulong>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<ushort>()
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// object& System.Runtime.CompilerServices.Unsafe.Add<object>(object&,int)
		// object& System.Runtime.CompilerServices.Unsafe.Add<object>(object&,int)
		// ushort& System.Runtime.CompilerServices.Unsafe.Add<ushort>(ushort&,System.IntPtr)
		// ushort& System.Runtime.CompilerServices.Unsafe.Add<ushort>(ushort&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.AddByteOffset<byte>(byte&,System.IntPtr)
		// ushort& System.Runtime.CompilerServices.Unsafe.AddByteOffset<ushort>(ushort&,System.IntPtr)
		// bool System.Runtime.CompilerServices.Unsafe.AreSame<ushort>(ushort&,ushort&)
		// System.DecimalEx.DecCalc& System.Runtime.CompilerServices.Unsafe.As<System.Decimal,System.DecimalEx.DecCalc>(System.Decimal&)
		// System.DecimalEx.DecimalBits& System.Runtime.CompilerServices.Unsafe.As<System.Decimal,System.DecimalEx.DecimalBits>(System.Decimal&)
		// System.GuidEx& System.Runtime.CompilerServices.Unsafe.As<System.Guid,System.GuidEx>(System.Guid&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.DateTime,byte>(System.DateTime&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.DateTimeOffset,byte>(System.DateTimeOffset&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Decimal,byte>(System.Decimal&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Guid,byte>(System.Guid&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.IntPtr,byte>(System.IntPtr&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.BigInteger,byte>(System.Numerics.BigInteger&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Complex,byte>(System.Numerics.Complex&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Matrix3x2,byte>(System.Numerics.Matrix3x2&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Matrix4x4,byte>(System.Numerics.Matrix4x4&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Plane,byte>(System.Numerics.Plane&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Quaternion,byte>(System.Numerics.Quaternion&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Vector2,byte>(System.Numerics.Vector2&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Vector3,byte>(System.Numerics.Vector3&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.Numerics.Vector4,byte>(System.Numerics.Vector4&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.TimeSpan,byte>(System.TimeSpan&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<System.UIntPtr,byte>(System.UIntPtr&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Bounds,byte>(UnityEngine.Bounds&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.BoundsInt,byte>(UnityEngine.BoundsInt&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Color,byte>(UnityEngine.Color&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Color32,byte>(UnityEngine.Color32&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.GradientAlphaKey,byte>(UnityEngine.GradientAlphaKey&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.GradientColorKey,byte>(UnityEngine.GradientColorKey&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Keyframe,byte>(UnityEngine.Keyframe&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.LayerMask,byte>(UnityEngine.LayerMask&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Matrix4x4,byte>(UnityEngine.Matrix4x4&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Quaternion,byte>(UnityEngine.Quaternion&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.RangeInt,byte>(UnityEngine.RangeInt&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Rect,byte>(UnityEngine.Rect&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.RectInt,byte>(UnityEngine.RectInt&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Vector2,byte>(UnityEngine.Vector2&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Vector2Int,byte>(UnityEngine.Vector2Int&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Vector3,byte>(UnityEngine.Vector3&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Vector3Int,byte>(UnityEngine.Vector3Int&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<UnityEngine.Vector4,byte>(UnityEngine.Vector4&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<double,byte>(double&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<float,byte>(float&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<int,byte>(int&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<long,byte>(long&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<object,byte>(object&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<sbyte,byte>(sbyte&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<short,byte>(short&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<uint,byte>(uint&)
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
		// ushort& System.Runtime.CompilerServices.Unsafe.As<byte,ushort>(byte&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<byte>(byte&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<uint>(uint&)
		// System.DateTime& System.Runtime.CompilerServices.Unsafe.AsRef<System.DateTime>(System.DateTime&)
		// System.DateTimeOffset& System.Runtime.CompilerServices.Unsafe.AsRef<System.DateTimeOffset>(System.DateTimeOffset&)
		// System.Decimal& System.Runtime.CompilerServices.Unsafe.AsRef<System.Decimal>(System.Decimal&)
		// System.Guid& System.Runtime.CompilerServices.Unsafe.AsRef<System.Guid>(System.Guid&)
		// System.IntPtr& System.Runtime.CompilerServices.Unsafe.AsRef<System.IntPtr>(System.IntPtr&)
		// System.Numerics.Complex& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Complex>(System.Numerics.Complex&)
		// System.Numerics.Matrix3x2& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Matrix3x2>(System.Numerics.Matrix3x2&)
		// System.Numerics.Matrix4x4& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Matrix4x4>(System.Numerics.Matrix4x4&)
		// System.Numerics.Plane& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Plane>(System.Numerics.Plane&)
		// System.Numerics.Quaternion& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Quaternion>(System.Numerics.Quaternion&)
		// System.Numerics.Vector2& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Vector2>(System.Numerics.Vector2&)
		// System.Numerics.Vector3& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Vector3>(System.Numerics.Vector3&)
		// System.Numerics.Vector4& System.Runtime.CompilerServices.Unsafe.AsRef<System.Numerics.Vector4>(System.Numerics.Vector4&)
		// System.TimeSpan& System.Runtime.CompilerServices.Unsafe.AsRef<System.TimeSpan>(System.TimeSpan&)
		// System.UIntPtr& System.Runtime.CompilerServices.Unsafe.AsRef<System.UIntPtr>(System.UIntPtr&)
		// UnityEngine.Bounds& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Bounds>(UnityEngine.Bounds&)
		// UnityEngine.BoundsInt& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.BoundsInt>(UnityEngine.BoundsInt&)
		// UnityEngine.Color& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Color>(UnityEngine.Color&)
		// UnityEngine.Color32& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Color32>(UnityEngine.Color32&)
		// UnityEngine.GradientAlphaKey& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.GradientAlphaKey>(UnityEngine.GradientAlphaKey&)
		// UnityEngine.GradientColorKey& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.GradientColorKey>(UnityEngine.GradientColorKey&)
		// UnityEngine.Keyframe& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Keyframe>(UnityEngine.Keyframe&)
		// UnityEngine.LayerMask& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.LayerMask>(UnityEngine.LayerMask&)
		// UnityEngine.Matrix4x4& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Matrix4x4>(UnityEngine.Matrix4x4&)
		// UnityEngine.Quaternion& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Quaternion>(UnityEngine.Quaternion&)
		// UnityEngine.RangeInt& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.RangeInt>(UnityEngine.RangeInt&)
		// UnityEngine.Rect& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Rect>(UnityEngine.Rect&)
		// UnityEngine.RectInt& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.RectInt>(UnityEngine.RectInt&)
		// UnityEngine.Vector2& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// UnityEngine.Vector2Int& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Vector2Int>(UnityEngine.Vector2Int&)
		// UnityEngine.Vector3& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Vector3>(UnityEngine.Vector3&)
		// UnityEngine.Vector3Int& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Vector3Int>(UnityEngine.Vector3Int&)
		// UnityEngine.Vector4& System.Runtime.CompilerServices.Unsafe.AsRef<UnityEngine.Vector4>(UnityEngine.Vector4&)
		// byte& System.Runtime.CompilerServices.Unsafe.AsRef<byte>(System.Void*)
		// byte& System.Runtime.CompilerServices.Unsafe.AsRef<byte>(byte&)
		// double& System.Runtime.CompilerServices.Unsafe.AsRef<double>(double&)
		// float& System.Runtime.CompilerServices.Unsafe.AsRef<float>(float&)
		// int& System.Runtime.CompilerServices.Unsafe.AsRef<int>(int&)
		// long& System.Runtime.CompilerServices.Unsafe.AsRef<long>(long&)
		// object& System.Runtime.CompilerServices.Unsafe.AsRef<object>(object&)
		// sbyte& System.Runtime.CompilerServices.Unsafe.AsRef<sbyte>(sbyte&)
		// short& System.Runtime.CompilerServices.Unsafe.AsRef<short>(short&)
		// uint& System.Runtime.CompilerServices.Unsafe.AsRef<uint>(uint&)
		// ulong& System.Runtime.CompilerServices.Unsafe.AsRef<ulong>(ulong&)
		// ushort& System.Runtime.CompilerServices.Unsafe.AsRef<ushort>(ushort&)
		// bool System.Runtime.CompilerServices.Unsafe.IsAddressLessThan<byte>(byte&,byte&)
		// bool System.Runtime.CompilerServices.Unsafe.IsAddressLessThan<object>(object&,object&)
		// bool System.Runtime.CompilerServices.Unsafe.IsNullRef<object>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.NullRef<object>()
		// System.Collections.Generic.KeyValuePair<int,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Collections.Generic.KeyValuePair<int,object>>(byte&)
		// System.Collections.Generic.KeyValuePair<object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Collections.Generic.KeyValuePair<object,object>>(byte&)
		// System.Collections.Generic.KeyValuePair<uint,Hsenl.Num> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>(byte&)
		// System.Collections.Generic.KeyValuePair<uint,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Collections.Generic.KeyValuePair<uint,object>>(byte&)
		// System.Nullable<System.DateTime> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.DateTime>>(byte&)
		// System.Nullable<System.DateTimeOffset> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.DateTimeOffset>>(byte&)
		// System.Nullable<System.Decimal> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Decimal>>(byte&)
		// System.Nullable<System.Guid> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Guid>>(byte&)
		// System.Nullable<System.IntPtr> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.IntPtr>>(byte&)
		// System.Nullable<System.Numerics.Complex> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Complex>>(byte&)
		// System.Nullable<System.Numerics.Matrix3x2> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Matrix3x2>>(byte&)
		// System.Nullable<System.Numerics.Matrix4x4> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Matrix4x4>>(byte&)
		// System.Nullable<System.Numerics.Plane> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Plane>>(byte&)
		// System.Nullable<System.Numerics.Quaternion> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Quaternion>>(byte&)
		// System.Nullable<System.Numerics.Vector2> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Vector2>>(byte&)
		// System.Nullable<System.Numerics.Vector3> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Vector3>>(byte&)
		// System.Nullable<System.Numerics.Vector4> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.Numerics.Vector4>>(byte&)
		// System.Nullable<System.TimeSpan> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.TimeSpan>>(byte&)
		// System.Nullable<System.UIntPtr> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<System.UIntPtr>>(byte&)
		// System.Nullable<UnityEngine.Bounds> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Bounds>>(byte&)
		// System.Nullable<UnityEngine.BoundsInt> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.BoundsInt>>(byte&)
		// System.Nullable<UnityEngine.Color32> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Color32>>(byte&)
		// System.Nullable<UnityEngine.Color> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Color>>(byte&)
		// System.Nullable<UnityEngine.GradientAlphaKey> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.GradientAlphaKey>>(byte&)
		// System.Nullable<UnityEngine.GradientColorKey> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.GradientColorKey>>(byte&)
		// System.Nullable<UnityEngine.Keyframe> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Keyframe>>(byte&)
		// System.Nullable<UnityEngine.LayerMask> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.LayerMask>>(byte&)
		// System.Nullable<UnityEngine.Matrix4x4> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Matrix4x4>>(byte&)
		// System.Nullable<UnityEngine.Quaternion> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Quaternion>>(byte&)
		// System.Nullable<UnityEngine.RangeInt> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.RangeInt>>(byte&)
		// System.Nullable<UnityEngine.Rect> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Rect>>(byte&)
		// System.Nullable<UnityEngine.RectInt> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.RectInt>>(byte&)
		// System.Nullable<UnityEngine.Vector2> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Vector2>>(byte&)
		// System.Nullable<UnityEngine.Vector2Int> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Vector2Int>>(byte&)
		// System.Nullable<UnityEngine.Vector3> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Vector3>>(byte&)
		// System.Nullable<UnityEngine.Vector3Int> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Vector3Int>>(byte&)
		// System.Nullable<UnityEngine.Vector4> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<UnityEngine.Vector4>>(byte&)
		// System.Nullable<byte> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<byte>>(byte&)
		// System.Nullable<double> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<double>>(byte&)
		// System.Nullable<float> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<float>>(byte&)
		// System.Nullable<int> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<int>>(byte&)
		// System.Nullable<long> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<long>>(byte&)
		// System.Nullable<object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<object>>(byte&)
		// System.Nullable<sbyte> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<sbyte>>(byte&)
		// System.Nullable<short> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<short>>(byte&)
		// System.Nullable<uint> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<uint>>(byte&)
		// System.Nullable<ulong> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<ulong>>(byte&)
		// System.Nullable<ushort> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Nullable<ushort>>(byte&)
		// System.ValueTuple<object,object,object,object,object,object,object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object,object,object,object,object,object,object>>(byte&)
		// System.ValueTuple<object,object,object,object,object,object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object,object,object,object,object,object>>(byte&)
		// System.ValueTuple<object,object,object,object,object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object,object,object,object,object>>(byte&)
		// System.ValueTuple<object,object,object,object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object,object,object,object>>(byte&)
		// System.ValueTuple<object,object,object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object,object,object>>(byte&)
		// System.ValueTuple<object,object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object,object>>(byte&)
		// System.ValueTuple<object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object,object>>(byte&)
		// System.ValueTuple<object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.ValueTuple<object>>(byte&)
		// byte System.Runtime.CompilerServices.Unsafe.ReadUnaligned<byte>(byte&)
		// float System.Runtime.CompilerServices.Unsafe.ReadUnaligned<float>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(byte&)
		// long System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>(byte&)
		// object System.Runtime.CompilerServices.Unsafe.ReadUnaligned<object>(byte&)
		// sbyte System.Runtime.CompilerServices.Unsafe.ReadUnaligned<sbyte>(byte&)
		// short System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(byte&)
		// uint System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>(byte&)
		// ulong System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>(byte&)
		// ushort System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ushort>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Collections.Generic.KeyValuePair<int,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Collections.Generic.KeyValuePair<object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Collections.Generic.KeyValuePair<uint,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.DateTime>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.DateTimeOffset>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Decimal>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Guid>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.IntPtr>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.DateTime>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.DateTimeOffset>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Decimal>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Guid>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.IntPtr>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Complex>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Matrix3x2>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Matrix4x4>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Plane>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Quaternion>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Vector2>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Vector3>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.Numerics.Vector4>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.TimeSpan>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<System.UIntPtr>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Bounds>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.BoundsInt>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Color32>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Color>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.GradientAlphaKey>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.GradientColorKey>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Keyframe>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.LayerMask>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Matrix4x4>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Quaternion>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.RangeInt>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Rect>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.RectInt>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Vector2>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Vector2Int>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Vector3>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Vector3Int>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<UnityEngine.Vector4>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<byte>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<double>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<float>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<int>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<long>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<sbyte>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<short>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<uint>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<ulong>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Nullable<ushort>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.BigInteger>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Complex>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Matrix3x2>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Matrix4x4>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Plane>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Quaternion>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Vector2>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Vector3>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Numerics.Vector4>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.TimeSpan>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.UIntPtr>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object,object,object,object,object,object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object,object,object,object,object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object,object,object,object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object,object,object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object,object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.ValueTuple<object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Bounds>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.BoundsInt>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Color32>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Color>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.GradientAlphaKey>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.GradientColorKey>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Keyframe>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.LayerMask>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Matrix4x4>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Quaternion>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.RangeInt>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Rect>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.RectInt>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Vector2>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Vector2Int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Vector3>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Vector3Int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<UnityEngine.Vector4>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<byte>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<double>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<float>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<long>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<object>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<sbyte>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<short>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<uint>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<ulong>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<ushort>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<ushort>()
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Collections.Generic.KeyValuePair<int,object>>(byte&,System.Collections.Generic.KeyValuePair<int,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Collections.Generic.KeyValuePair<object,object>>(byte&,System.Collections.Generic.KeyValuePair<object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>>(byte&,System.Collections.Generic.KeyValuePair<uint,Hsenl.Num>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Collections.Generic.KeyValuePair<uint,object>>(byte&,System.Collections.Generic.KeyValuePair<uint,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.DateTime>>(byte&,System.Nullable<System.DateTime>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.DateTimeOffset>>(byte&,System.Nullable<System.DateTimeOffset>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Decimal>>(byte&,System.Nullable<System.Decimal>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Guid>>(byte&,System.Nullable<System.Guid>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.IntPtr>>(byte&,System.Nullable<System.IntPtr>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Complex>>(byte&,System.Nullable<System.Numerics.Complex>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Matrix3x2>>(byte&,System.Nullable<System.Numerics.Matrix3x2>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Matrix4x4>>(byte&,System.Nullable<System.Numerics.Matrix4x4>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Plane>>(byte&,System.Nullable<System.Numerics.Plane>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Quaternion>>(byte&,System.Nullable<System.Numerics.Quaternion>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Vector2>>(byte&,System.Nullable<System.Numerics.Vector2>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Vector3>>(byte&,System.Nullable<System.Numerics.Vector3>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.Numerics.Vector4>>(byte&,System.Nullable<System.Numerics.Vector4>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.TimeSpan>>(byte&,System.Nullable<System.TimeSpan>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<System.UIntPtr>>(byte&,System.Nullable<System.UIntPtr>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Bounds>>(byte&,System.Nullable<UnityEngine.Bounds>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.BoundsInt>>(byte&,System.Nullable<UnityEngine.BoundsInt>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Color32>>(byte&,System.Nullable<UnityEngine.Color32>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Color>>(byte&,System.Nullable<UnityEngine.Color>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.GradientAlphaKey>>(byte&,System.Nullable<UnityEngine.GradientAlphaKey>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.GradientColorKey>>(byte&,System.Nullable<UnityEngine.GradientColorKey>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Keyframe>>(byte&,System.Nullable<UnityEngine.Keyframe>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.LayerMask>>(byte&,System.Nullable<UnityEngine.LayerMask>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Matrix4x4>>(byte&,System.Nullable<UnityEngine.Matrix4x4>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Quaternion>>(byte&,System.Nullable<UnityEngine.Quaternion>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.RangeInt>>(byte&,System.Nullable<UnityEngine.RangeInt>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Rect>>(byte&,System.Nullable<UnityEngine.Rect>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.RectInt>>(byte&,System.Nullable<UnityEngine.RectInt>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Vector2>>(byte&,System.Nullable<UnityEngine.Vector2>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Vector2Int>>(byte&,System.Nullable<UnityEngine.Vector2Int>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Vector3>>(byte&,System.Nullable<UnityEngine.Vector3>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Vector3Int>>(byte&,System.Nullable<UnityEngine.Vector3Int>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<UnityEngine.Vector4>>(byte&,System.Nullable<UnityEngine.Vector4>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<byte>>(byte&,System.Nullable<byte>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<double>>(byte&,System.Nullable<double>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<float>>(byte&,System.Nullable<float>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<int>>(byte&,System.Nullable<int>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<long>>(byte&,System.Nullable<long>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<object>>(byte&,System.Nullable<object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<sbyte>>(byte&,System.Nullable<sbyte>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<short>>(byte&,System.Nullable<short>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<uint>>(byte&,System.Nullable<uint>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<ulong>>(byte&,System.Nullable<ulong>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Nullable<ushort>>(byte&,System.Nullable<ushort>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object,object,object,object,object,object,object>>(byte&,System.ValueTuple<object,object,object,object,object,object,object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object,object,object,object,object,object>>(byte&,System.ValueTuple<object,object,object,object,object,object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object,object,object,object,object>>(byte&,System.ValueTuple<object,object,object,object,object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object,object,object,object>>(byte&,System.ValueTuple<object,object,object,object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object,object,object>>(byte&,System.ValueTuple<object,object,object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object,object>>(byte&,System.ValueTuple<object,object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object,object>>(byte&,System.ValueTuple<object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.ValueTuple<object>>(byte&,System.ValueTuple<object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<byte>(byte&,byte)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<float>(byte&,float)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<int>(byte&,int)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<long>(byte&,long)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<object>(byte&,object)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<sbyte>(byte&,sbyte)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<short>(byte&,short)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<uint>(byte&,uint)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<ulong>(byte&,ulong)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<ushort>(byte&,ushort)
		// System.Void System.Runtime.InteropServices.Marshal.StructureToPtr<object>(object,System.IntPtr,bool)
		// System.ReadOnlySpan<byte> System.Runtime.InteropServices.MemoryMarshal.AsBytes<ushort>(System.ReadOnlySpan<ushort>)
		// System.Span<byte> System.Runtime.InteropServices.MemoryMarshal.AsBytes<ushort>(System.Span<ushort>)
		// System.ReadOnlySpan<byte> System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan<byte>(byte&,int)
		// System.ReadOnlySpan<ushort> System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan<ushort>(ushort&,int)
		// System.Span<byte> System.Runtime.InteropServices.MemoryMarshal.CreateSpan<byte>(byte&,int)
		// System.Span<ushort> System.Runtime.InteropServices.MemoryMarshal.CreateSpan<ushort>(ushort&,int)
		// System.DateTime& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.DateTime>(System.Span<System.DateTime>)
		// System.DateTimeOffset& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.DateTimeOffset>(System.Span<System.DateTimeOffset>)
		// System.Decimal& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Decimal>(System.Span<System.Decimal>)
		// System.Guid& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Guid>(System.Span<System.Guid>)
		// System.IntPtr& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.IntPtr>(System.Span<System.IntPtr>)
		// System.Numerics.BigInteger& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.BigInteger>(System.Span<System.Numerics.BigInteger>)
		// System.Numerics.Complex& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Complex>(System.Span<System.Numerics.Complex>)
		// System.Numerics.Matrix3x2& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Matrix3x2>(System.Span<System.Numerics.Matrix3x2>)
		// System.Numerics.Matrix4x4& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Matrix4x4>(System.Span<System.Numerics.Matrix4x4>)
		// System.Numerics.Plane& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Plane>(System.Span<System.Numerics.Plane>)
		// System.Numerics.Quaternion& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Quaternion>(System.Span<System.Numerics.Quaternion>)
		// System.Numerics.Vector2& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Vector2>(System.Span<System.Numerics.Vector2>)
		// System.Numerics.Vector3& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Vector3>(System.Span<System.Numerics.Vector3>)
		// System.Numerics.Vector4& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.Numerics.Vector4>(System.Span<System.Numerics.Vector4>)
		// System.TimeSpan& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.TimeSpan>(System.Span<System.TimeSpan>)
		// System.UIntPtr& System.Runtime.InteropServices.MemoryMarshal.GetReference<System.UIntPtr>(System.Span<System.UIntPtr>)
		// UnityEngine.Bounds& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Bounds>(System.Span<UnityEngine.Bounds>)
		// UnityEngine.BoundsInt& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.BoundsInt>(System.Span<UnityEngine.BoundsInt>)
		// UnityEngine.Color& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Color>(System.Span<UnityEngine.Color>)
		// UnityEngine.Color32& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Color32>(System.Span<UnityEngine.Color32>)
		// UnityEngine.GradientAlphaKey& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.GradientAlphaKey>(System.Span<UnityEngine.GradientAlphaKey>)
		// UnityEngine.GradientColorKey& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.GradientColorKey>(System.Span<UnityEngine.GradientColorKey>)
		// UnityEngine.Keyframe& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Keyframe>(System.Span<UnityEngine.Keyframe>)
		// UnityEngine.LayerMask& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.LayerMask>(System.Span<UnityEngine.LayerMask>)
		// UnityEngine.Matrix4x4& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Matrix4x4>(System.Span<UnityEngine.Matrix4x4>)
		// UnityEngine.Quaternion& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Quaternion>(System.Span<UnityEngine.Quaternion>)
		// UnityEngine.RangeInt& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.RangeInt>(System.Span<UnityEngine.RangeInt>)
		// UnityEngine.Rect& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Rect>(System.Span<UnityEngine.Rect>)
		// UnityEngine.RectInt& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.RectInt>(System.Span<UnityEngine.RectInt>)
		// UnityEngine.Vector2& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Vector2>(System.Span<UnityEngine.Vector2>)
		// UnityEngine.Vector2Int& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Vector2Int>(System.Span<UnityEngine.Vector2Int>)
		// UnityEngine.Vector3& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Vector3>(System.Span<UnityEngine.Vector3>)
		// UnityEngine.Vector3Int& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Vector3Int>(System.Span<UnityEngine.Vector3Int>)
		// UnityEngine.Vector4& System.Runtime.InteropServices.MemoryMarshal.GetReference<UnityEngine.Vector4>(System.Span<UnityEngine.Vector4>)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.ReadOnlySpan<byte>)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.Span<byte>)
		// double& System.Runtime.InteropServices.MemoryMarshal.GetReference<double>(System.Span<double>)
		// float& System.Runtime.InteropServices.MemoryMarshal.GetReference<float>(System.Span<float>)
		// int& System.Runtime.InteropServices.MemoryMarshal.GetReference<int>(System.Span<int>)
		// long& System.Runtime.InteropServices.MemoryMarshal.GetReference<long>(System.Span<long>)
		// object& System.Runtime.InteropServices.MemoryMarshal.GetReference<object>(System.ReadOnlySpan<object>)
		// object& System.Runtime.InteropServices.MemoryMarshal.GetReference<object>(System.Span<object>)
		// sbyte& System.Runtime.InteropServices.MemoryMarshal.GetReference<sbyte>(System.Span<sbyte>)
		// short& System.Runtime.InteropServices.MemoryMarshal.GetReference<short>(System.Span<short>)
		// uint& System.Runtime.InteropServices.MemoryMarshal.GetReference<uint>(System.Span<uint>)
		// ulong& System.Runtime.InteropServices.MemoryMarshal.GetReference<ulong>(System.Span<ulong>)
		// ushort& System.Runtime.InteropServices.MemoryMarshal.GetReference<ushort>(System.ReadOnlySpan<ushort>)
		// ushort& System.Runtime.InteropServices.MemoryMarshal.GetReference<ushort>(System.Span<ushort>)
		// bool System.Runtime.InteropServices.MemoryMarshal.TryGetArray<byte>(System.ReadOnlyMemory<byte>,System.ArraySegment<byte>&)
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
		// object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
		// YooAsset.AllAssetsHandle YooAsset.ResourcePackage.LoadAllAssetsSync<object>(string)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
		// YooAsset.AllAssetsHandle YooAsset.YooAssets.LoadAllAssetsSync<object>(string)
		// YooAsset.AssetHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
		// string string.Create<System.ValueTuple<System.Buffers.ReadOnlySequence<byte>,long,int,object>>(int,System.ValueTuple<System.Buffers.ReadOnlySequence<byte>,long,int,object>,System.Buffers.SpanAction<System.Char,System.ValueTuple<System.Buffers.ReadOnlySequence<byte>,long,int,object>>)
		// string string.Create<System.ValueTuple<System.IntPtr,int,int,object>>(int,System.ValueTuple<System.IntPtr,int,int,object>,System.Buffers.SpanAction<System.Char,System.ValueTuple<System.IntPtr,int,int,object>>)
		// string string.Join<float>(string,System.Collections.Generic.IEnumerable<float>)
		// string string.Join<int>(string,System.Collections.Generic.IEnumerable<int>)
		// string string.Join<object>(string,System.Collections.Generic.IEnumerable<object>)
		// string string.JoinCore<float>(System.Char*,int,System.Collections.Generic.IEnumerable<float>)
		// string string.JoinCore<int>(System.Char*,int,System.Collections.Generic.IEnumerable<int>)
		// string string.JoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
	}
}