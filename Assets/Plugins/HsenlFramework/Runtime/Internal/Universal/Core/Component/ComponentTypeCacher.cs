using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MemoryPack;

namespace Hsenl {
    public interface IReadOnlyComponentTypeCacher {
        bool ContainsAll(ComponentTypeCacher list);
        bool ContainsAny(ComponentTypeCacher list, out int idx);
    }

    [Serializable]
    [MemoryPackable()]
    public sealed unsafe partial class ComponentTypeCacher : Bitlist, IReadOnlyComponentTypeCacher {
        [MemoryPackInclude]
        public readonly int baseIndex;

        public static ComponentTypeCacher Create(int baseIndex) {
            return new ComponentTypeCacher(baseIndex, Entity.CacherCount);
        }

        public static ComponentTypeCacher CreateNull() {
            return Entity.CacherCount == 0 ? new ComponentTypeCacher(-1) : new ComponentTypeCacher(-1, Entity.CacherCount);
        }

        [MemoryPackConstructor]
        private ComponentTypeCacher(int baseIndex) {
            this.baseIndex = baseIndex;
        }
        
        private ComponentTypeCacher(int baseIndex, int capacity) : base(capacity) {
            this.baseIndex = baseIndex;
        }

        public ContainsEnumerable Contains(ComponentTypeCacher cacher) => this.Contains((Bitlist)cacher);

        public bool ContainsAll(ComponentTypeCacher cacher) => this.ContainsAll((Bitlist)cacher);

        public bool ContainsAny(ComponentTypeCacher cacher) => this.ContainsAny((Bitlist)cacher);

        public bool ContainsAny(ComponentTypeCacher cacher, out int idx) => this.ContainsAny((Bitlist)cacher, out idx);

        public override string ToString() {
            var list = this.ToList();
            StringBuilder builder = new();
            if (this.baseIndex != -1) {
                builder.Append(this.baseIndex);
                builder.Append(';');
            }

            for (int i = 0, len = list.Count; i < len; i++) {
                builder.Append(list[i]);
                builder.Append(',');
            }

            return builder.ToString();
        }
    }
}