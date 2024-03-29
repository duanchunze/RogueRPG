﻿using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace Hsenl {
    [Serializable]
    public class Appearance : Unbodied, IUpdate {
        [FormerlySerializedAs("assetName")]
        public string modelName;
        public GameObject model;
        public int modelType = 0; // 0: 3d, 1: 2d
        private SpriteRenderer _spriteRenderer;

        public Entity Holder { get; private set; }

        public Action<GameObject> onModelLoaded;

        private HashSet<Material> _materials = new();
        private static readonly int _outlineColor = Shader.PropertyToID("_OutlineColor");

        protected override void OnStart() {
            this.Holder = Entity.Create("Model Holder");
            this.Holder.SetParent(this.Entity);
        }

        public GameObject LoadModel(string modelName_) {
            this.modelName = modelName_;
            var assetFinder = new OptimalAssetFinder(modelName_, "appear_actor");
            assetFinder.Append("Actor");
            var assetInfo = Resource.GetOptimalBundleName(assetFinder);
            if (YooAssets.LoadAssetSync(assetInfo)?.AssetObject is not GameObject prefab)
                return null;

            this.model = UnityEngine.Object.Instantiate(prefab, this.Holder.GameObject.transform, false);
            try {
                this.onModelLoaded?.Invoke(this.model);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this._spriteRenderer = this.model.GetComponentInChildren<SpriteRenderer>();
            if (this._spriteRenderer) {
                this.modelType = 1;
                this._materials.Add(this._spriteRenderer.material);
            }
            else {
                foreach (var skinnedMeshRenderer in this.model.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                    if (!this._materials.Contains(skinnedMeshRenderer.material)) {
                        this._materials.Add(skinnedMeshRenderer.material);
                    }
                }
            }

            return this.model;
        }

        public void SetModelColor(Color color) {
            if (this.modelType == 0) {
                foreach (var material in this._materials) {
                    material.color = color;
                }
            }
            else {
                this._spriteRenderer.color = color;
            }
        }

        public void SetModelOutlineColor(Color color) {
            if (this.modelType == 0) {
                foreach (var material in this._materials) {
                    material.color = color;
                }
            }
            else {
                foreach (var material in this._materials) {
                    material.SetColor(_outlineColor, color);
                }
            }
        }

        public void Update() {
            if (this.modelType == 0)
                return;

            this.model.transform.rotation = Quaternion.identity;
            var dir = this.transform.Forward;
            if (dir.x > 0) {
                this._spriteRenderer.flipX = false;
            }
            else if (dir.x < 0) {
                this._spriteRenderer.flipX = true;
            }

            this._spriteRenderer.transform.parent.localPosition = new Vector3(0, this.transform.Position.z * 0.0001f, 0);
        }
    }
}