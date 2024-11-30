namespace Hsenl.CrossCombiner {
    public class Prop_Actor_Combiner : CrossCombiner<Prop, Actor> {
        protected override void OnCombin(Prop arg1, Actor arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Attach(numeratorNode);
        }

        protected override void OnDecombin(Prop arg1, Actor arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Detach(numeratorNode);
        }
    }
}