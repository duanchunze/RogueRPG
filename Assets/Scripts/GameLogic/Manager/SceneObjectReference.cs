using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hsenl {
    public class SceneObjectReference : ReferenceCollector { }

    [CustomEditor(typeof(SceneObjectReference))]
    public class SceneObjectReferenceEditor : ReferenceCollectorEditor { }
}