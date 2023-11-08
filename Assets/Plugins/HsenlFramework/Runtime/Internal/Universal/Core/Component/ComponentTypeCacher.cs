using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MemoryPack;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Hsenl {
    public interface IReadOnlyComponentTypeCacher {
        bool ContainsAll(ComponentTypeCacher list);
        bool ContainsAny(ComponentTypeCacher list, out int idx);
    }

    [Serializable]
    [MemoryPackable()]
    public sealed unsafe partial class ComponentTypeCacher : Bitlist, IReadOnlyComponentTypeCacher, IEquatable<ComponentTypeCacher> {
        [MemoryPackInclude]
        public readonly int originalIndex;

        public static ComponentTypeCacher Create(int originalIndex) {
            return new ComponentTypeCacher(originalIndex, Entity.CacherCount);
        }

        public static ComponentTypeCacher CreateNull() {
            return Entity.CacherCount == 0 ? new ComponentTypeCacher(-1) : new ComponentTypeCacher(-1, Entity.CacherCount);
        }

        [MemoryPackConstructor]
        private ComponentTypeCacher(int originalIndex) {
            this.originalIndex = originalIndex;
        }

        private ComponentTypeCacher(int originalIndex, int capacity) : base(capacity) {
            this.originalIndex = originalIndex;
        }

        public ContainsEnumerable Contains(ComponentTypeCacher cacher) => this.Contains((Bitlist)cacher);

        public bool ContainsAll(ComponentTypeCacher cacher) => this.ContainsAll((Bitlist)cacher);

        public bool ContainsAny(ComponentTypeCacher cacher) => this.ContainsAny((Bitlist)cacher);

        public bool ContainsAny(ComponentTypeCacher cacher, out int idx) => this.ContainsAny((Bitlist)cacher, out idx);

        public int ContainsCount(ComponentTypeCacher cacher) => this.ContainsCount((Bitlist)cacher);

        public override string ToString() {
            var list = this.ToList();
            StringBuilder builder = new();
            if (this.originalIndex != -1) {
                builder.Append(this.originalIndex);
                builder.Append(';');
            }

            for (int i = 0, len = list.Count; i < len; i++) {
                builder.Append(list[i]);
                builder.Append(',');
            }

            return builder.ToString();
        }

        public bool Equals(ComponentTypeCacher other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.originalIndex != other.originalIndex) return false;
            if (this.BucketLength != other.BucketLength) return false;
            for (int i = 0, len = this.BucketLength; i < len; i++) {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj) {
            if (obj is not ComponentTypeCacher cacher) return false;
            return this.Equals(cacher);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), this.originalIndex);
        }
    }
}