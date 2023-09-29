using UnityEditor;

namespace Hsenl {
    public class BackupsEditor : EditorWindow {
        [MenuItem("ET/Backups Tool")]
        private static void ShowWindow() {
            GetWindow<BackupsEditor>().Show();
        }
    }
}