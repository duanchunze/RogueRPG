using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class Global : UnitySingleton<Global> {
        public Camera mainCamera;
        public Camera uiCamera;
    }
}