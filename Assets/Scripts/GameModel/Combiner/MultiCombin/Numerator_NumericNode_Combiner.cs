namespace Hsenl.MultiCombiner {
    public class Numerator_NumericNode_Combiner : MultiCombiner<Numerator, NumericNode> {
        protected override void OnCombin(Numerator arg1, NumericNode arg2) {
            if ((arg2.LinkModel & NumericNodeLinkModel.AutoLinkToSelf) == NumericNodeLinkModel.AutoLinkToSelf) {
                arg1.Attach(arg2);
                arg2.LinkNumerator(arg1);
            }
        }

        protected override void OnDecombin(Numerator arg1, NumericNode arg2) {
            if (arg1.Detach(arg2)) {
                arg2.UnlinkNumerator(arg1);
            }
        }
    }
}