namespace Hsenl.View {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.ViewExpression)]
    public class PlhHarmViewExpression : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            if (item.source is Ability ability) {
                var sound = item.hurt.GetComponent<Sound>();
                if (sound != null) {
                    var clip = AppearanceSystem.LoadSoundClip(item.hitsound);
                    if (clip != null) {
                        sound.Play(clip);
                    }
                }

                FxManager.Instance.Play(item.hitfx, item.hurt.transform.Position);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}