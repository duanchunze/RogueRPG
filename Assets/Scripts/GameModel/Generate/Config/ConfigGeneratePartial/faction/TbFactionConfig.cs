namespace Hsenl.faction {
    public partial class TbFactionConfig {
        private readonly Bitlist _factionTags = new();

        public Bitlist AllFactionTags => this._factionTags;

        partial void PostResolve() {
            foreach (var factionConfig in this._dataList) {
                this._factionTags.Add(factionConfig.FactionTag);
            }
        }
    }
}