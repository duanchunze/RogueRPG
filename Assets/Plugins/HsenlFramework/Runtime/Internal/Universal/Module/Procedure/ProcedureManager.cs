using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class ProcedureManager : Singleton<ProcedureManager> {
        private Procedure _procedure;

        public static Procedure Procedure => Instance._procedure;

        protected override void OnRegister() {
            this._procedure = new Procedure();
        }

        protected override void OnUnregister() {
            this._procedure = null;
        }
    }
}