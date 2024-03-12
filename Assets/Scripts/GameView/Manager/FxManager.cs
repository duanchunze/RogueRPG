using System;
using UnityEngine;
using YooAsset;

namespace Hsenl.View {
    public class FxManager : MonoBehaviour {
        public static FxManager Instance;

        private readonly MultiQueue<int, UnityEngine.ParticleSystem> _psPool = new();

        private void Awake() {
            Instance = this;
        }

        public void Play(string fxName, Vector3 position, AppearanceMiddleArgStream middleArgStream = null) {
            var hashcode = fxName.GetHashCode();
            if (middleArgStream != null) {
                hashcode = HashCode.Combine(hashcode, middleArgStream.CalcHashCode());
            }
            
            var ps = this._psPool.Dequeue(hashcode);

            if (ps == null) {
                var prefab = AppearanceSystem.LoadFx(fxName, middleArgStream);
                prefab.gameObject.SetActive(false);
                ps = Instantiate(prefab).GetComponent<UnityEngine.ParticleSystem>();
            }

            ps.transform.position = position;
            ps.gameObject.SetActive(false);
            ps.gameObject.SetActive(true);
            
            ps.GetOrAddComponent<MonoTimer>().TimeStart(0.25f, () => {
                ps.transform.SetParent(this.transform);
                ps.gameObject.SetActive(false);
                this._psPool.Enqueue(hashcode, ps);
            });
        }
    }
}