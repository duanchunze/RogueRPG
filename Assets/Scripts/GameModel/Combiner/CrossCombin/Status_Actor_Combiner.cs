namespace Hsenl.CrossCombiner {
    public class Status_Actor_Combiner : CrossCombiner<Status, Actor> {
        protected override void OnCombin(Status arg1, Actor arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Attach(numeratorNode);

            var pln = arg1.GetComponent<ProcedureLineNode>();
            var pl = arg2.GetComponent<ProcedureLine>();
            if (pl != null && pln != null)
                pl.Attach(pln);
        }

        protected override void OnDecombin(Status arg1, Actor arg2) {
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