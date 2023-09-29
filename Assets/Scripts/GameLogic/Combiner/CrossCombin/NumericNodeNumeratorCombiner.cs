using Hsenl;

namespace Hsenl.CrossCombiner {
    public class NumericNodeNumeratorCombiner : CrossCombiner<NumericNode, Numerator> {
        protected override void OnCombin(NumericNode arg1, Numerator arg2) {
            if ((arg1.LinkModel & NumericNodeLinkModel.AutoLinkToParent) == NumericNodeLinkModel.AutoLinkToParent) {
                arg2.Attach(arg1);
                arg1.LinkNumerator(arg2);
            }
        }

        protected override void OnDecombin(NumericNode arg1, Numerator arg2) {
            if (arg2.Detach(arg1)) {
                arg1.UnlinkNumerator(arg2);
            }
        }
    }
}