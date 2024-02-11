using Hsenl;
using UnityEngine;

namespace Hsenl.CrossCombiner {
    public class ControlTrigger_Control_Combiner : CrossCombiner<ControlTrigger, Control> {
        protected override void OnCombin(ControlTrigger arg1, Control arg2) {
            arg1.SetControl(arg2);
        }

        protected override void OnDecombin(ControlTrigger arg1, Control arg2) {
            arg1.SetControl(null);
        }
    }
}