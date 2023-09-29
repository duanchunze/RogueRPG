using System;
using System.Collections.Generic;

namespace Hsenl.faction {
    public partial class FactionConfig {
        private readonly MultiDictionary<FactionModel, FactionType, Bitlist> _factionRelations = new();
        private readonly List<FactionType> factionTypes = new();

        partial void PostResolve() {
            foreach (var kv in this.FactionRelations) {
                foreach (var kv2 in kv.Value) {
                    var bitlist = new Bitlist();
                    bitlist.Add(kv2.Value);
                    this._factionRelations.Add(kv.Key, kv2.Key, bitlist);
                }
            }

            foreach (var value in Enum.GetValues(typeof(FactionType))) {
                this.factionTypes.Add((FactionType)value);
            }
        }

        public void GetFactionRelations(FactionModel factionModel, FactionType factionType, Bitlist cache) {
            var dict = this._factionRelations[factionModel];
            foreach (var num in this.factionTypes) {
                if ((factionType & num) == num) {
                    dict.TryGetValue(num, out var bitlist);
                    cache.Add(bitlist);
                }
            }
        }
    }
}