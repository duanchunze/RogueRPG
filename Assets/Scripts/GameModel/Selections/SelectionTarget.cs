using System;

namespace Hsenl {
    [Serializable]
    public class SelectionTarget : Unbodied {
        public bool Contains(TagType tagType) {
            return this.Tags.Contains(tagType);
        }

        public bool ContainsAll(Bitlist tags) {
            return this.Tags.ContainsAll(tags);
        }

        public bool ContainsAny(Bitlist tags) {
            return this.Tags.ContainsAny(tags);
        }

        public bool ContainsAll(IReadOnlyBitlist tags) {
            return this.Tags.ContainsAll(tags);
        }

        public bool ContainsAny(IReadOnlyBitlist tags) {
            return this.Tags.ContainsAny(tags);
        }
    }
}