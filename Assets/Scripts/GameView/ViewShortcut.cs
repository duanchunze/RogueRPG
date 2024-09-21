using UnityEngine;

namespace Hsenl.View {
    public static class ViewShortcut {
        public static void ShowJumpMessage(string message, UnityEngine.Transform followTarget, Vector3 followOffset, Vector3 jumpOffset, Vector3 localScale,
            Color color = default) {
            var ui = UIManager.MultiOpen<UIJumpMessage>(UILayer.Low);
            ui.text.color = color;
            ui.text.transform.localScale = localScale;
            ui.WriteText(message, followTarget, followOffset, jumpOffset);
        }

        public static void ShowJumpMessage(string message, Vector3 anchor, Vector3 followOffset, Vector3 jumpOffset, Vector3 localScale,
            Color color = default) {
            var ui = UIManager.MultiOpen<UIJumpMessage>(UILayer.Low);
            ui.text.color = color;
            ui.text.transform.localScale = localScale;
            ui.WriteText(message, anchor, followOffset, jumpOffset);
        }
    }
}