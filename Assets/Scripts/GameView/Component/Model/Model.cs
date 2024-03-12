using System;
using UnityEngine;

namespace Hsenl.View {
    public class Model : Unbodied {
        public GameObject ModelObj { get; private set; }

        public Action<GameObject> OnModelChanged { get; set; }

        public void SetModel(GameObject modelObj) {
            if (this.ModelObj == modelObj)
                return;

            if (this.ModelObj != null) {
                UnityEngine.Object.Destroy(this.ModelObj);
            }

            this.ModelObj = modelObj;
            if (modelObj != null) {
                modelObj.transform.SetParent(this.UnityTransform, false);
            }

            try {
                this.OnModelChanged?.Invoke(modelObj);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}