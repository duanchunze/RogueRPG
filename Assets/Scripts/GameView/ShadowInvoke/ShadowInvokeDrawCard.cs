namespace Hsenl.View {
    [ShadowFunction(typeof(InvokeStation))]
    public static partial class ShadowInvokeDrawCard {
        private static HTask _task;

        [ShadowFunction]
        private static async Hsenl.HTask<bool> InvokeDrawCard(Hsenl.DrawCard drawCard)
        {
            TimeInfo.TimeScale = 0;
            {
                var ui = UIManager.SingleOpen<UIDrawCard>(UILayer.High);
                ui.FillIn(drawCard);
                ui.onSelected = () => {
                    _task.SetResult(); //
                };
            }

            _task = HTask.Create();
            await _task;

            {
                UIManager.SingleClose<UIDrawCard>();
            }

            TimeInfo.TimeScale = 1;
            return true;
        }
    }
}