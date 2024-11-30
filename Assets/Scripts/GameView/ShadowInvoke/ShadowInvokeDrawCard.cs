using System;

namespace Hsenl.View {
    [ShadowFunction(typeof(InvokeStation))]
    public static partial class ShadowInvokeDrawCard {
        private static HTask _task;

        [ShadowFunction]
        private static async Hsenl.HTask<bool> InvokeDrawCard(Hsenl.DrawCard drawCard) {
            TimeInfo.TimeScale = 0;
            {
                var ui = UIManager.SingleOpen<UIDrawCard>(UILayer.High);
                _task = HTask.Create();
                ui.onSelected = () => {
                    _task.SetResult(); //
                };
                try {
                    ui.FillIn(drawCard);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            await _task;

            {
                UIManager.SingleClose<UIDrawCard>();
            }

            TimeInfo.TimeScale = 1;
            return true;
        }
    }
}