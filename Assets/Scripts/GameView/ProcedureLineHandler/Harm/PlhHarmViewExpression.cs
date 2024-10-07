namespace Hsenl.View {
    [ProcedureLineHandlerPriority(PliHarmPriority.ViewExpression)]
    public class PlhHarmViewExpression : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, object userToken) {
            if (item.source is Ability ability) {
                var sound = item.harmable.GetComponent<Sound>();
                if (sound != null && item.hitsound != null) {
                    var clip = AppearanceSystem.LoadSoundClip(item.hitsound);
                    if (clip != null) {
                        sound.Play(clip);
                    }
                }

                if (item.hitfx != null) {
                    FxManager.Instance.Play(item.hitfx, item.hurtable.transform.Position);
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}