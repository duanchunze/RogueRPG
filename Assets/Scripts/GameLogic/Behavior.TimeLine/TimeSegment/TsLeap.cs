using Hsenl.timeline;
using MemoryPack;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TsLeap : TsInfo<LeapInfo> {
        private Bodied _leaper;
        private ControlTrigger _controlTrigger;

        private Tweener _tweener;

        protected override void OnNodeReset() {
            // 这里把逻辑写在Reset里是一种特殊情况,
            // 因为Reset是当CasterStart时, 就会同步调用, 而OnTimeSegmentOrigin则要等到下一帧才会调用
            // 而因为我们用的是dotween来进行运动, 而不是用OnTimeSegmentRunning来运动, 所以更适合写在这里
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._leaper = ability.AttachedBodied;
                    this._controlTrigger = ability.GetComponent<ControlTrigger>();

                    this._tweener?.Kill();

                    var stage = this.GetNodeInParent<Stage>();

                    this._leaper.transform.StopMove();

                    if (this._controlTrigger.GetValue(out var point)) {
                        var dir = (Vector3)point - this._leaper.transform.Position;
                        this._leaper.transform.LookAt(dir);
                        var position = this._leaper.transform.Position;
                        this._tweener = DOTween.To(() => position, p => {
                            position = p;
                            this._leaper.transform.Position = p;
                        }, point, 6.5f).SetSpeedBased().OnComplete(() => { this.manager.Abort(); }).SetEase(Ease.Linear);
                    }

                    Shortcut.InflictionStatus(this._leaper, this._leaper, StatusAlias.Wuqishou);

                    break;
                }
            }
        }

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            this._tweener?.Kill();
            this._tweener = null;
        }
    }
}