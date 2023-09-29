using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public class GameFactoryWindowEditor : OdinEditorWindow {
        [MenuItem("ET/GameFactoryWindow")]
        private static void ShowWindow() {
            var window = GetWindow<GameFactoryWindowEditor>();
            window.titleContent = new GUIContent("GameFactoryWindow");
            window.Show();
        }
    }
}