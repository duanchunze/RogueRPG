using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Hsenl {
    public class MatTextureMatcherEditor : OdinEditorWindow {
        [ShowInInspector]
        private string _matMatchContext;
        [ShowInInspector]
        private string _textureMatchContext;

        [ShowInInspector]
        private string materialsPath;
        [ShowInInspector]
        private string texturesPath;

        private string basemap;
        private string normalmap;
        private string heightmap;
        
        [MenuItem("ET/材质贴图匹配器")]
        private static void ShowWindow() {
            GetWindow<MatTextureMatcherEditor>().Show();
        }

        protected override void OnGUI() {
            base.OnGUI();
            
        }
    }
}