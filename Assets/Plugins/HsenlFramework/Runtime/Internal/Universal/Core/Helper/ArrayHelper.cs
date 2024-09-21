﻿using System;
using System.Collections.Generic;

namespace Hsenl {
    public static class ArrayHelper {
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

        public static T[] Combin<T>(Span<T> lhs, Span<T> rhs) {
            if (lhs == null || rhs == null) return null;
            var results = new T[lhs.Length + rhs.Length];
            for (var i = 0; i < lhs.Length; i++) {
                results[i] = lhs[i];
            }

            var leftLen = lhs.Length;

            for (var i = 0; i < rhs.Length; i++) {
                results[i + leftLen] = rhs[i];
            }

            return results;
        }

        public static void SwapElements<T>(this IList<T> self, T t1, T t2) {
            var index1 = -1;
            var index2 = -1;
            for (int i = 0, len = self.Count; i < len; i++) {
                var element = self[i];
                if (t1.Equals(element)) {
                    index1 = i;
                    continue;
                }

                if (t2.Equals(element)) {
                    index2 = i;
                }
            }

            if (index1 == -1 || index2 == -1)
                return;

            self[index1] = t2;
            self[index2] = t1;
        }

        /// 归并排序
        /// aux是作为临时数组用, 如果提供的话, 可以避免gc
        public static void MergeSort<T>(IList<T> array, IList<T> aux = null, Comparer<T> comparer = null) {
            comparer ??= Comparer<T>.Default;

            var n = array.Count;
            if (n == 0)
                return;

            if (aux == null || aux.Count != n) {
                aux = new T[n];
            }


            for (var size = 1; size < n; size *= 2) {
                for (var low = 0; low < n - size; low += 2 * size) {
                    var mid = low + size - 1;
                    var high = System.Math.Min(low + 2 * size - 1, n - 1);

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

        /// 归并排序
        /// aux是作为临时数组用, 如果提供的话, 可以避免gc
        public static void MergeSort<T>(Span<T> array, Span<T> aux = default, Comparer<T> comparer = null) {
            comparer ??= Comparer<T>.Default;

            var n = array.Length;
            if (n == 0)
                return;

            if (aux.Length != n) {
                aux = new T[n];
            }


            for (var size = 1; size < n; size *= 2) {
                for (var low = 0; low < n - size; low += 2 * size) {
                    var mid = low + size - 1;
                    var high = System.Math.Min(low + 2 * size - 1, n - 1);

                    Merge(array, aux, low, mid, high, comparer);
                }
            }
        }

        private static void Merge<T>(Span<T> array, Span<T> aux, int low, int mid, int high, Comparer<T> comparer) {
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