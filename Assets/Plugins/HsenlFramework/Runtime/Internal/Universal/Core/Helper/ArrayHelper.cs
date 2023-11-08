using System;
using System.Collections.Generic;

namespace Hsenl {
    public static class ArrayHelper {
        public static void CopyTo<T>(this List<T> src, ref List<T> target) {
            if (target == null) {
                target = new List<T>(src.Count);
            }
            else {
                target.Clear();
            }

            for (var i = 0; i < src.Count; i++) {
                target.Add(src[i]);
            }
        }

        public static T[] Combin<T>(IList<T> lhs, IList<T> rhs) {
            if (lhs == null || rhs == null) return null;
            var results = new T[lhs.Count + rhs.Count];
            for (var i = 0; i < lhs.Count; i++) {
                results[i] = lhs[i];
            }

            var leftLen = lhs.Count;

            for (var i = 0; i < rhs.Count; i++) {
                results[i + leftLen] = rhs[i];
            }

            return results;
        }

        // 归并排序
        public static void MergeSort<T>(IList<T> array, Comparer<T> comparer = null) {
            comparer ??= Comparer<T>.Default;

            var n = array.Count;
            var aux = new T[n];

            for (var size = 1; size < n; size *= 2) {
                for (var low = 0; low < n - size; low += 2 * size) {
                    var mid = low + size - 1;
                    var high = Math.Min(low + 2 * size - 1, n - 1);

                    Merge(array, aux, low, mid, high, comparer);
                }
            }
        }

        private static void Merge<T>(IList<T> array, IList<T> aux, int low, int mid, int high, Comparer<T> comparer) {
            var i = low;
            var j = mid + 1;

            for (var k = low; k <= high; k++) {
                aux[k] = array[k];
            }

            for (var k = low; k <= high; k++) {
                if (i > mid) {
                    array[k] = aux[j++];
                }
                else if (j > high) {
                    array[k] = aux[i++];
                }
                else if (comparer.Compare(aux[j], aux[i]) < 0) {
                    array[k] = aux[j++];
                }
                else {
                    array[k] = aux[i++];
                }
            }
        }
    }

    public struct MergeSortFloatWrap<T> : IComparable<MergeSortFloatWrap<T>> {
        public float num;
        public T value;

        public static implicit operator T(MergeSortFloatWrap<T> arg) {
            return arg.value;
        }

        public int CompareTo(MergeSortFloatWrap<T> other) {
            return this.num.CompareTo(other.num);
        }
    }
}