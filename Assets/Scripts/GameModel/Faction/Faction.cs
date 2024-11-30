using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class Faction : Unbodied {
        // 阵营模式
        [MemoryPackInclude]
        private FactionModel _factionModel;

        [MemoryPackIgnore]
        public FactionModel FactionModel {
            get => this._factionModel;
            set => this._factionModel = value;
        }

        private Bitlist _tagCache = new();

        public IReadOnlyBitlist GetTagsOfFactionTypes(IList<FactionType> factionTypes) {
            this._tagCache.Clear();
            for (int i = 0, len = factionTypes.Count; i < len; i++) {
                this.AppendTagsOfFactionType(factionTypes[i], this._tagCache);
            }

            return this._tagCache;
        }

        public void GetTagsOfFactionTypes(IList<FactionType> factionTypes, Bitlist bitlist) {
            bitlist.Clear();
            for (int i = 0, len = factionTypes.Count; i < len; i++) {
                this.AppendTagsOfFactionType(factionTypes[i], bitlist);
            }
        }

        public void AppendTagsOfFactionTypes(IList<FactionType> factionTypes, Bitlist bitlist) {
            for (int i = 0, len = factionTypes.Count; i < len; i++) {
                this.AppendTagsOfFactionType(factionTypes[i], bitlist);
            }
        }

        public IReadOnlyBitlist GetTagsOfFactionType(FactionType factionType) {
            this._tagCache.Clear();
            this.AppendTagsOfFactionType(factionType, this._tagCache);
            return this._tagCache;
        }

        public void GetTagsOfFactionType(FactionType factionType, Bitlist bitlist) {
            bitlist.Clear();
            this.AppendTagsOfFactionType(factionType, bitlist);
        }

        public void AppendTagsOfFactionType(FactionType factionType, Bitlist bitlist) {
            var tb = Tables.Instance.TbFactionConfig;
            foreach (var tag in tb.AllFactionTags.Contains(this.Tags)) {
                var config = tb[(TagType)tag];
                config.GetFactionRelations(this._factionModel, factionType, bitlist);
            }
        }
    }
}