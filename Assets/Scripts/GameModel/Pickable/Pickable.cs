﻿using System;
using Hsenl.pickable;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    // 挂载该组件可以被拾取者捡起
    // Pickable和Dropable及Picker之间的关系
    // 
    [Serializable]
    public class Pickable : Bodied, IPoolable {
        public int configId;
        public PickableConfig Config => Tables.Instance.TbPickableConfig.GetById(this.configId);

        public GameObject model;
        private SphereCollider _collider;

        public int count;

        private PoolKey _poolKey;

        public PoolKey PoolKey => this._poolKey;

        void IPoolable.SetPoolKey(PoolKey poolKey) {
            this._poolKey = poolKey;
        }

        public void LoadModel(string assetName) {
            var prefab = YooAssets.LoadAssetSync<GameObject>(assetName).AssetObject as GameObject;
            this.model = UnityEngine.Object.Instantiate(prefab, this.UnityTransform, false);
        }

        public async void LoadCollider(float delay) {
            if (this._collider != null) {
                Object.Destroy(this._collider.Entity);
            }

            this._collider = ColliderManager.Instance.Rent<SphereCollider>("Pickable Collider", active: false);
            this._collider.IsTrigger = true;
            this._collider.SetUsage(GameColliderPurpose.Pickable);
            this._collider.Radius = this.Config.Radius;
            this._collider.SetParent(this.Entity);

            // 掉落物刚刚掉落后先不可拾取, 延迟后再可拾取
            if (delay > 0)
                await Timer.WaitTimeWithScale(delay);

            this._collider.Entity.Active = true;
        }
    }
}