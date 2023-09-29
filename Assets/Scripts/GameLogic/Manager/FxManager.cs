using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Hsenl {
    public class FxManager : MonoBehaviour {
        public static FxManager Instance;

        private readonly MultiQueue<string, ParticleSystem> _psPool = new();

        private void Awake() {
            Instance = this;
        }

        public void Play(string fxName, Vector3 position) {
            var ps = this._psPool.Dequeue(fxName);

            if (ps == null) {
                var prefab = ResourcesHelper.GetAsset<GameObject>(Constant.FxBundleName, fxName);
                prefab.gameObject.SetActive(false);
                ps = Instantiate(prefab).GetComponent<ParticleSystem>();
            }

            ps.transform.position = position;
            ps.gameObject.SetActive(false);
            ps.gameObject.SetActive(true);

            ps.GetOrAddComponent<MonoTimer>().TimeStart(0.25f, () => {
                ps.transform.SetParent(this.transform);
                ps.gameObject.SetActive(false);
                this._psPool.Enqueue(fxName, ps);
            });
        }
    }
}