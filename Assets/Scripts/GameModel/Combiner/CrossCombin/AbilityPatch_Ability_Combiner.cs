namespace Hsenl.CrossCombiner {
    public class AbilityPatch_Ability_Combiner : CrossCombiner<AbilityPatch, Ability> {
        protected override void OnCombin(AbilityPatch arg1, Ability arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Attach(numeratorNode);

            var pln = arg1.GetComponent<ProcedureLineNode>();
            var pl = arg2.GetComponent<ProcedureLine>();
            if (pl != null && pln != null)
                pl.Attach(pln);
        }

        protected override void OnDecombin(AbilityPatch arg1, Ability arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Detach(numeratorNode);

            var pln = arg1.GetComponent<ProcedureLineNode>();
            var pl = arg2.GetComponent<ProcedureLine>();
            if (pl != null && pln != null)
                pl.Detach(pln);
        }
    }
}