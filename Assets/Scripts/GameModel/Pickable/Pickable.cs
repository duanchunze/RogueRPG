using System;
using Hsenl.pickable;
using Test;
using UnityEngine;

namespace Hsenl {
    // 挂载该组件可以被拾取者捡起
    // Pickable和Dropable及Picker之间的关系
    // 
    [Serializable]
    public class Pickable : Substantive {
        public int configId;
        public PickableConfig Config => Tables.Instance.TbPickableConfig.GetById(this.configId);

        public GameObject model;
        private SphereCollider _sphereCollider;

        public int count;

        public void LoadModel(string assetName) {
            var prefab = ResourcesHelper.GetAsset<GameObject>(Constant.PickableModelBundleName, assetName);
            this.model = UnityEngine.Object.Instantiate(prefab, this.UnityTransform, false);
        }

        public async void LoadCollider(float delay) {
            if (this._sphereCollider != null) {
                Object.Destroy(this._sphereCollider.Entity);
            }

            this._sphereCollider =
                ColliderFactory.CreateCollider<SphereCollider>("Pickable Collider", GameColliderPurpose.Pickable, nonEvent: true, enabled: false);
            this._sphereCollider.Radius = this.Config.Radius;
            this._sphereCollider.SetParent(this.Entity);

            // 掉落物刚刚掉落后先不可拾取, 延迟0.3s再可拾取
            await Timer.WaitTime((long)(delay * 1000f));
            this._sphereCollider.Enable = true;
        }
    }
}