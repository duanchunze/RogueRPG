using System;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class Tester : MonoBehaviour {
        private Entity _entity;

        public Tester tester;

        [Button("Click")]
        public void Click() {
            Instantiate(this.tester);
        }

        private void Start() {
            SceneManager.LoadScene("main", LoadSceneMode.Single);
            Numerator.InitNumerator(3);

            var entity = Entity.Create("123");
            entity.transform.Enable = false;
            var abi = entity.AddComponent<Ability>();
            abi.Status = BodiedStatus.Dependent;
            abi.Enable = false;
            var newe = Object.InstantiateWithUnity(entity);
            Debug.LogError(newe.transform.Enable);
            Debug.LogError(newe.GetComponent<Ability>().Status);
            Debug.LogError(newe.GetComponent<Ability>().Enable);
            Debug.LogError(newe.GetComponent<Ability>().IsDeserialized);
        }

        private void Update() { }

        private void OnEnable() {
            // Debug.LogError("OnEnable");
        }

        private void OnDisable() {
            // Debug.LogError("OnDisable");
        }

        private void OnTransformChildrenChanged() {
            // Debug.LogError("OnTransformChildrenChanged");
        }

        private void OnTransformParentChanged() {
            // Debug.LogError("OnTransformParentChanged");
        }

        private void OnBeforeTransformParentChanged() {
            // Debug.LogError("OnBeforeTransformParentChanged");
        }
    }
}