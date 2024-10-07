namespace Hsenl.CrossCombiner {
    public class AbilityAssist_Ability_Combiner : CrossCombiner<AbilityAssist, Ability> {
        protected override void OnCombin(AbilityAssist arg1, Ability arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Attach(numeratorNode);
        }

        protected override void OnDecombin(AbilityAssist arg1, Ability arg2) {
            var numeratorNode = arg1.GetComponent<Numeric>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Detach(numeratorNode);
        }
    }
}