using Hsenl.actor;
using UnityEngine;

namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public partial class ShadowEventOnHeroUnlockUpdate {
        [ShadowFunction]
        private void OnHeroUnlockUpdate() {
            var ui = UISelectHero.instance;
            using var allHeroCandidates = ListComponent<ActorConfig>.Create();
            foreach (var actorConfig in Tables.Instance.TbActorConfig.DataList) {
                if (!actorConfig.Labels.Contains(TagType.Hero))
                    continue;

                allHeroCandidates.Add(actorConfig);
            }

            var unlockHeroCandidates = UnlockCircumstance.Instance.heroUnlock;
            var len = allHeroCandidates.Count;
            ui.holder.NormalizeChildren(ui.template, len);

            for (var i = 0; i < len; i++) {
                var uiSlot = ui.holder.GetChild(i).GetComponent<UISelectHeroSlot>();
                var config = allHeroCandidates[i];
                uiSlot.FillIn(config);
                if (unlockHeroCandidates.Contains(config.Id)) {
                    // unlock
                    uiSlot.image.color = Color.white;
                }
                else {
                    // lock
                    uiSlot.image.color = Color.gray;
                }

                uiSlot.text.text = config.ViewName;
                uiSlot.onButtonClickInvoke = () => {
                    ui.selectSlot = uiSlot;
                    ProcedureManager.Procedure.ChangeState<ProcedureAdventure>(ui.selectSlot.Filler.Id);
                };
            }
        }
    }
}