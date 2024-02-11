using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl.View {
    [Serializable]
    public class HeadMessage : Unbodied {
        private UIHeadFollow _uiHeadFollow;

        [ShowInInspector]
        private float _height;

        protected override void OnEnable() {
            if (this.Tags.Contains(TagType.Hero)) {
                this._uiHeadFollow = (UIHeadFollow)UIManager.MultiOpen(UIName.HeroHeadMessage, UILayer.Low);
            }
            else {
                this._uiHeadFollow = (UIHeadFollow)UIManager.MultiOpen(UIName.MonsterHeadMessage, UILayer.Low);
            }

            this._uiHeadFollow.followTarget = this.UnityTransform;
            this._uiHeadFollow.followOffset = new Vector3(0, 0, this._height);
        }

        protected override void OnDisable() {
            UIManager.MultiClose(this._uiHeadFollow);
            this._uiHeadFollow = null;
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