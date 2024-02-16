using UnityEngine;
using YooAsset;

namespace Hsenl {
    public class FxManager : MonoBehaviour {
        public static FxManager Instance;

        private readonly MultiQueue<string, UnityEngine.ParticleSystem> _psPool = new();

        private void Awake() {
            Instance = this;
        }

        public void Play(string fxName, Vector3 position) {
            var ps = this._psPool.Dequeue(fxName);

            if (ps == null) {
                var prefab = YooAssets.LoadAssetSync<GameObject>(fxName).AssetObject as GameObject;
                prefab.gameObject.SetActive(false);
                ps = Instantiate(prefab).GetComponent<UnityEngine.ParticleSystem>();
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