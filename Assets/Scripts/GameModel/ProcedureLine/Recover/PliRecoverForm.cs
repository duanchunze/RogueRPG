namespace Hsenl {
    public struct PliRecoverForm {
        public Bodied target;
        public Bodied inflictor;
        public Component source;
        
        private Numerator _targetNumerator;
        private Numerator _inflictorNumerator;

        public Numerator TargetNumerator => this._targetNumerator ??= this.target.GetComponent<Numerator>();
        public Numerator InflictorNumerator => this._inflictorNumerator ??= this.inflictor.GetComponent<Numerator>();

        public int recoverHp;
    }
}