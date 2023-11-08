namespace Hsenl {
    public struct PliAbilityCastChangedForm {
        public Bodied caster;
        private Numerator _casterNumerator;
        public Ability ability;
        private Numerator _abilityNumerator;
        public int currStage;
        public StageLine stageLine;

        public Numerator CasterNumerator => this._casterNumerator ??= this.caster.GetComponent<Numerator>();
        public Numerator AbilityNumerator => this._abilityNumerator ??= this.ability.GetComponent<Numerator>();
    }
}