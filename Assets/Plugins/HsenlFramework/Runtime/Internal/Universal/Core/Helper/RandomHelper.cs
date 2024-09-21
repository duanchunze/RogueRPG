using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Random = System.Random;

namespace Hsenl {
    // 支持多线程
    public static class RandomHelper {
        [ThreadStatic]
        private static Random _random;

        private static int[] _cumulativeWeights = Array.Empty<int>(); // 用于权重随机数

        private static Unity.Mathematics.Random _mtRandom = Unity.Mathematics.Random.CreateFromIndex(721);

        private static readonly object _locker = new();

        private static Random GetRandom() {
            return _random ??= new Random(Guid.NewGuid().GetHashCode());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RandUInt64() {
            var r1 = RandInt32();
            var r2 = RandInt32();

            return ((ulong)r1 << 32) & (ulong)r2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RandInt32() {
            return GetRandom().Next();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RandUInt32() {
            return (uint)GetRandom().Next();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RandInt64() {
            var r1 = RandUInt32();
            var r2 = RandUInt32();
            return (long)(((ulong)r1 << 32) | r2);
        }

        /// <summary>
        /// 获取lower与Upper之间的随机数,包含下限，不包含上限
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextInt(int min, int max) {
            var value = GetRandom().Next(min, max);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextFloat() {
            return _mtRandom.NextFloat();
            // var a = NextInt(0, 1000000);
            // return a / 1000000f;
        }

        public static Vector3 NextFloat3(Vector3 min, Vector3 max) {
            var v = _mtRandom.NextFloat3(new float3(min.x, min.y, min.z), new float3(max.x, max.y, max.z));
            return new Vector3(v.x, v.y, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextFloat(float min, float max) {
            return NextFloat() * (max - min) + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RandomBool() {
            return GetRandom().Next(2) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomArray<T>(T[] array) {
            return array[NextInt(0, array.Length)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomArray<T>(List<T> array) {
            return array[NextInt(0, array.Count)];
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">要打乱的数组</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DerangeRank<T>(List<T> arr) {
            if (arr == null || arr.Count < 2) {
                return;
            }

            for (var i = 0; i < arr.Count; i++) {
                var index = GetRandom().Next(0, arr.Count);
                (arr[index], arr[i]) = (arr[i], arr[index]);
            }
        }

        public static int RandomInt32OfWeight(int min, int max, IList<int> weights) {
            if (min == max) return min;

            var len = weights.Count;
            if (len != max - min + 1) {
                throw new ArgumentException("every value must has it weight.");
            }

            lock (_locker) {
                CalculateCumulativeWeights(weights);

                var totalWeight = _cumulativeWeights[len - 1];
                var randNum = _mtRandom.NextInt(totalWeight);

                var index = GetIndexOfCumulativeWeight(randNum, len);

                return min + index;
            }
        }

        /// <summary>
        /// 从一个数组里, 随机跳出若干个元素, 且可以使用权重对结果进行干扰
        /// </summary>
        /// <param name="array"></param>
        /// <param name="weights"></param>
        /// <param name="count">总共要抽多少个元素</param>
        /// <param name="maxEach">每个元素可以被抽取的最大个数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static unsafe T[] RandomArrayOfWeight<T>(IList<T> array, IList<int> weights, int count, int maxEach = -1) {
            var len = weights.Count;
            if (len != array.Count) {
                throw new ArgumentException("every element must has it weight.");
            }

            if (maxEach > count) {
                throw new ArgumentException("count cant less of maxEach");
            }

            if (maxEach < 1) {
                maxEach = count;
            }

            if (len * maxEach < count) {
                // 比如从5个元素里, 抽6个, 假如每个元素的最大抽取数为1, 则永远抽不够6个出来, 但如果最大抽取数为2, 那么5个元素每个都可能有两次机会, 总数就是10个, 那么就
                // 有可能抽出6个出来.
                throw new ArgumentException("Theoretically, you can't extract enough elements");
            }

            using var list = ListComponent<T>.Rent();
            var weightsCopy = stackalloc int[len];
            for (var i = 0; i < len; i++) {
                weightsCopy[i] = weights[i];
            }

            lock (_locker) {
                while (list.Count < count) {
                    CalculateCumulativeWeights(weightsCopy, len);
                    var totalWeight = _cumulativeWeights[len - 1];

                    for (var i = 0; i < maxEach; i++) {
                        var randNum = _mtRandom.NextInt(totalWeight);
                        var index = GetIndexOfCumulativeWeight(randNum, len);
                        list.Add(array[index]);
                        if (list.Count == count)
                            break;

                        weightsCopy[index] = 0;
                    }
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// 从一个数组里, 随机跳出若干个元素, 且可以使用权重对结果进行干扰 (默认所有元素的权重都为1)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <param name="maxEach"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static unsafe T[] RandomArrayOfWeight<T>(IList<T> array, int count, int maxEach = -1) {
            var len = array.Count;
            if (maxEach > count) {
                throw new ArgumentException("count cant less of maxEach");
            }

            if (maxEach < 1) {
                maxEach = count;
            }

            if (len * maxEach < count) {
                // 比如从5个元素里, 抽6个, 假如每个元素的最大抽取数为1, 则永远抽不够6个出来, 但如果最大抽取数为2, 那么5个元素每个都可能有两次机会, 总数就是10个, 那么就
                // 有可能抽出6个出来.
                throw new ArgumentException("Theoretically, you can't extract enough elements");
            }

            using var list = ListComponent<T>.Rent();
            var weightsCopy = stackalloc int[len];
            for (var i = 0; i < len; i++) {
                weightsCopy[i] = 1;
            }

            lock (_locker) {
                while (list.Count < count) {
                    CalculateCumulativeWeights(weightsCopy, len);
                    var totalWeight = _cumulativeWeights[len - 1];

                    for (var i = 0; i < maxEach; i++) {
                        var randNum = _mtRandom.NextInt(totalWeight);
                        var index = GetIndexOfCumulativeWeight(randNum, len);
                        list.Add(array[index]);
                        if (list.Count == count)
                            break;

                        weightsCopy[index] = 0;
                    }
                }

                return list.ToArray();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CalculateCumulativeWeights(IList<int> weights) {
            var len = weights.Count;
            if (len > _cumulativeWeights.Length) {
                _cumulativeWeights = new int[len];
            }

            var cumSpan = _cumulativeWeights.AsSpan();
            cumSpan[0] = weights[0];

            for (var i = 1; i < len; i++) {
                var weight = weights[i];
                if (weight < 0) {
                    throw new ArgumentException("weight cannot be negative.");
                }

                cumSpan[i] = cumSpan[i - 1] + weight;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void CalculateCumulativeWeights(int* weights, int length) {
            if (length > _cumulativeWeights.Length) {
                _cumulativeWeights = new int[length];
            }

            var cumSpan = _cumulativeWeights.AsSpan();
            cumSpan[0] = weights[0];

            for (var i = 1; i < length; i++) {
                var weight = weights[i];
                if (weight < 0) {
                    throw new ArgumentException("weight cannot be negative.");
                }

                cumSpan[i] = cumSpan[i - 1] + weight;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndexOfCumulativeWeight(int value, int count) {
            for (var i = 0; i < count; i++) {
                var cumulativeWeight = _cumulativeWeights[i];
                if (cumulativeWeight > value) {
                    return i;
                }
            }

            return count - 1;
        }

        public static void ClearCache() {
            lock (_locker) {
                _cumulativeWeights = Array.Empty<int>();
            }
        }
    }

    [Serializable]
    public struct RandomArrayElement<T> {
        public T value;
        public uint weight;

        public RandomArrayElement(T t, float weight = 1) {
            if (weight == 0) throw new ArgumentException($"weight must greater than 0 '{weight}'");
            this.value = t;
            this.weight = (uint)(weight * 1000);
        }
    }
}