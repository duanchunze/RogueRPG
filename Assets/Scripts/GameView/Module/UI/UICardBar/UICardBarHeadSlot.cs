using Hsenl.EventType;
using UnityEngine;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICardBarHeadSlot : UICardBarSlot {
        public Image cooldownMask;
        public GameObject autoTrigger;

        public float cooldownTime;
        public float cooldownTillTime;

        public override void FillIn(Card filler) {
            base.FillIn(filler);

            if (filler != null) {
                var ability = (Ability)filler.Source;
                var controlTrigger = ability.GetComponent<ControlTrigger>();
                if (controlTrigger == null || controlTrigger.ControlCode == (int)ControlCode.AutoTrigger) {
                    this.autoTrigger.gameObject.SetActive(true);
                }
                else {
                    this.autoTrigger.gameObject.SetActive(false);
                }
            }
        }

        protected override void OnButtonClick() {
            EventSystem.Publish(new ChangeAbilityAutoTrigger { cardInstanceId = this.FillerInstanceId, cardBarSlotInstanceId = this.SlotInstanceId });
        }

        public void RunCooldown(float cooltime, float cooltilltime) {
            this.cooldownTime = cooltime;
            this.cooldownTillTime = cooltilltime;
        }

        private void Update() {
            if (this.cooldownMask == null) return;
            if (TimeInfo.Time <= this.cooldownTillTime) {
                var diff = this.cooldownTillTime - TimeInfo.Time;
                var pct = diff / this.cooldownTime;
                this.cooldownMask.fillAmount = pct;
            }
        }
    }
}