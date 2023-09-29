using System;

namespace Hsenl {
    [Flags]
    public enum NumericNodeLinkModel {
        NoAutoLink = 0,
        AutoLinkToParent = 1,
        AutoLinkToSelf = 2,
    }
}