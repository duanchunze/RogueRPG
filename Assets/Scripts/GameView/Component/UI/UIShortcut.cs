using Unity.Mathematics;
using UnityEngine;

namespace Hsenl.View {
    public static class UIShortcut {
        public static void ShowJumpMessage(string message, UnityEngine.Transform followTarget, float3 followOffset, float3 jumpOffset, float3 localScale,
            Color color = default) {
            var ui = UIManager.MultiOpen<UIJumpMessage>(UILayer.Low);
            ui.text.color = color;
            ui.text.transform.localScale = localScale;
            ui.WriteText(message, followTarget, followOffset, jumpOffset);
        }

        public static void ShowJumpMessage(string message, float3 anchor, float3 followOffset, float3 jumpOffset, float3 localScale,
            Color color = default) {
            var ui = UIManager.MultiOpen<UIJumpMessage>(UILayer.Low);
            ui.text.color = color;
            ui.text.transform.localScale = localScale;
            ui.WriteText(message, anchor, followOffset, jumpOffset);
        }
    }
}