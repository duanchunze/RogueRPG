using System;
using System.Collections.Generic;

namespace Hsenl {
    [ShadowFunction]
    public static partial class InvokeStation {
        [ShadowFunction]
        public static async HTask<bool> InvokeDrawCard(DrawCard drawCard) {
            return await InvokeDrawCardShadow(drawCard);
        }
    }
}