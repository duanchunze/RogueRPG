using Hsenl;
using UnityEngine;

namespace Hsenl.CrossCombiner {
    public class ControlTriggerControlCombiner : CrossCombiner<ControlTrigger, Control> {
        protected override void OnCombin(ControlTrigger arg1, Control arg2) {
            arg1.SetControl(arg2);
        }

        protected override void OnDecombin(ControlTrigger arg1, Control arg2) {
            arg1.SetControl(null);
        }
    }
}