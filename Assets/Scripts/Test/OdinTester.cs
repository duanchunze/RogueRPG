using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Test {
    public class OdinTester : SerializedMonoBehaviour {
        [OdinSerialize, ShowInInspector]
        private SerializeTest _serializeTest;
    }

    public class SerializeTest {
        public string a;
        public string b;
    }
}