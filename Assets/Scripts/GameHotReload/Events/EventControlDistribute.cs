using Hsenl.EventType;
using UnityEngine;

namespace Hsenl {
    public class EventControlDistribute : AEventSync<ControlMessage> {
        protected override void Handle(ControlMessage arg) {
            if (GameManager.Instance == null)
                return;

            foreach (var target in GameManager.Instance.ControlTargets) {
                // 两个特殊的控制指令，需要额外处理
                if (arg.controlCode == (int)ControlCode.Direction) { }

                if (arg.controlCode == (int)ControlCode.MoveOfPoint) { }

                Set(target, arg);
            }

            void Set(Control control, ControlMessage controlMessage) {
                if (controlMessage.isButtonDown) {
                    control.SetStart(controlMessage.controlCode);
                }

                if (controlMessage.isButton) {
                    control.SetSustained(controlMessage.controlCode);
                }

                if (controlMessage.isButtonUp) {
                    control.SetEnd(controlMessage.controlCode);
                }

                if (controlMessage.hasValue) {
                    control.SetValue(controlMessage.controlCode, controlMessage.value);
                }
            }
        }

        private static void HandleDirection(ref EventType.ControlMessage inputMessage) {
            // switch (InputGroup.current.inputGroupType) {
            //     case InputGroupType.Keycode:
            //         if (Global.MainControl == null) {
            //             break;
            //         }
            //
            //         var ray = Camera.main.ScreenPointToRay(inputMessage.value);
            //         if (Physics.Raycast(ray, out var hit, 1000, 1 << 6)) {
            //             var position = Global.MainControl.UnityTransform.position;
            //             var point = hit.point;
            //             var dir = point - position;
            //             inputMessage.value = new float3(dir.x, 0, dir.z);
            //         }
            //
            //         break;
            //
            //     case InputGroupType.Joystick:
            //         break;
            // }
        }

        private static void HandleMouseClick(ref EventType.ControlMessage inputMessage) {
            switch (InputGroup.current.inputGroupType) {
                case InputGroupType.Keycode:
                    var ray = Camera.main.ScreenPointToRay(InputController.GetMousePosition());
                    if (Physics.Raycast(ray, out var hit, 1000, 1 << 6)) {
                        inputMessage.value = new Vector3(hit.point.x, 0, hit.point.z);
                    }

                    break;

                case InputGroupType.Joystick:
                    break;
            }
        }
    }
}