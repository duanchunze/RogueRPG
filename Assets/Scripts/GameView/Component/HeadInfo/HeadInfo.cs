using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl.View {
    // 头部显示信息, 比如血量, 蓝量等
    [Serializable]
    public class HeadInfo : Unbodied {
        private UIHeadFollow _uiHeadFollow;

        public Action refreshInvoke;

        [ShowInInspector]
        private float _height;

        protected override void OnEnable() {
            if (this.Tags.Contains(TagType.Hero)) {
                this._uiHeadFollow = (UIHeadFollow)UIManager.MultiOpen(UIName.HeroHeadFollow, UILayer.Low);
            }
            else {
                this._uiHeadFollow = (UIHeadFollow)UIManager.MultiOpen(UIName.MonsterHeadFollow, UILayer.Low);
            }

            this._uiHeadFollow.followTarget = this.UnityTransform;
            this._uiHeadFollow.followOffset = new Vector3(0, 0, this._height);

            this.Refresh();
        }

        protected override void OnDisable() {
            UIManager.MultiClose(this._uiHeadFollow);
            this._uiHeadFollow = null;
        }

        public void Refresh() {
            try {
                this.refreshInvoke?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void UpdateHp(float value) {
            if (this._uiHeadFollow == null)
                return;

            this._uiHeadFollow.healthSlider.UpdateSlider(value);
        }

        public void UpdateEnergy(float value) {
            if (this._uiHeadFollow == null)
                return;

            this._uiHeadFollow.energySlider.UpdateSlider(value);
        }

        public void UpdateMana(float value) {
            if (this._uiHeadFollow == null)
                return;

            this._uiHeadFollow.manaSlider.UpdateSlider(value);
        }

        public void UpdateFollowHeight(float height) {
            if (this._uiHeadFollow == null)
                return;

            this._height = height;
            this._uiHeadFollow.followOffset = new Vector3(0, 0, this._height);
        }
    }
}