using Hsenl.EventType;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    public class EventNumericChanged : AEventSync<OnNumericChanged> {
        protected override void Handle(OnNumericChanged arg) {
            // var procedureLine = arg.numerator.GetComponent<ProcedureLine>();
            // if (procedureLine != null) {
            //     var numericChangedForm = new NumericChangedForm() {
            //         hub = arg.hub,
            //         numericType = arg.numKey,
            //         old = arg.old,
            //         now = arg.now,
            //     };
            //     procedureLine.StartLine(ref numericChangedForm, false);
            // }

            var numerator = arg.numerator;
            var nt = (NumericType)arg.numType;
            switch (nt) {
                case NumericType.Height: {
                    var body = numerator.GetComponent<PhysicBody>();
                    body.Height = arg.now;
                    body.Radius = body.Height * 0.1444f;
                    body.Center = new Vector3(0, body.Height * 0.5f, 0);

                    var agent = numerator.GetMonoComponent<NavMeshAgent>();
                    agent.height = arg.now;
                    agent.radius = agent.height * 0.15f;
                    break;
                }

                case NumericType.Mspd: {
                    var agent = numerator.GetMonoComponent<NavMeshAgent>();
                    agent.speed = arg.now;
                    break;
                }

                case NumericType.Prange: {
                    var picker = numerator.GetComponent<Picker>();
                    if (picker != null) {
                        picker.Radius = arg.now;
                    }

                    break;
                }
            }
        }
    }
}