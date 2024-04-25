using System;
using System.Collections.Generic;
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
        }

        private void Update() {
        }
    }
}