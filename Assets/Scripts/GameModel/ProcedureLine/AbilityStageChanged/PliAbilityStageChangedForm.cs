namespace Hsenl {
    public struct PliAbilityStageChangedForm {
        public Bodied Spellcaster => this.ability.MainBodied;
        private Numerator _spellcasterNumerator;
        public Ability ability;
        private Numerator _abilityNumerator;
        public int currStage;
        public StageLine stageLine;

        public Num manaCost;
        public Num cd;

        public Numerator SpellcasterNumerator => this._spellcasterNumerator ??= this.Spellcaster.GetComponent<Numerator>();
        public Numerator AbilityNumerator => this._abilityNumerator ??= this.ability.GetComponent<Numerator>();
    }
}