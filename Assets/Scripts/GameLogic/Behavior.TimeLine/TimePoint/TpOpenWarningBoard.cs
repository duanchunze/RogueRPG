using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpOpenWarningBoard : TpInfo<timeline.OpenWarningBoardInfo> {
        private readonly List<Numerator> _numerators = new(2);

        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var warnBoard = ability.FindChild(this.info.WarnName);
                    if (warnBoard == null) {
                        warnBoard = WarningBoardFactory.Create(this.info.WarnName);
                        warnBoard.SetParent(ability.Entity);
                    }

                    warnBoard.Active = true;

                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = ability.GetHolder()?.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);

                    var tsize = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Tsize);
                    if (tsize == 0)
                        tsize = 1;

                    warnBoard.transform.LocalScale = Vector3.one * tsize;
                    break;
                }
            }
        }
    }
}