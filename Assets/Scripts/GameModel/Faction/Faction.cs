using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class Faction : Unbodied {
        // 阵营模式
        private FactionModel _factionModel;

        public FactionModel FactionModel {
            get => this._factionModel;
            set => this._factionModel = value;
        }

        private Bitlist _cache = new();

        public IReadOnlyBitlist GetTagsOfFactionTypes(IList<FactionType> factionTypes) {
            this._cache.Clear();
            for (int i = 0, len = factionTypes.Count; i < len; i++) {
                this.AppendTagsOfFactionType(factionTypes[i], this._cache);
            }

            return this._cache;
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
            this._cache.Clear();
            this.AppendTagsOfFactionType(factionType, this._cache);
            return this._cache;
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