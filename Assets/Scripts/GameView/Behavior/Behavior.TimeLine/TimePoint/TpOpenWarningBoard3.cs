using System.Collections.Generic;
using Hsenl.timeline;
using MemoryPack;

namespace Hsenl.View {
    [MemoryPackable]
    public partial class TpOpenWarningBoard3 : TpInfo<OpenWarningBoard3Info> {
        private readonly List<Numerator> _numerators = new(2);

        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.MainBodied;
                    if (owner == null)
                        break;

                    if (ability.targets.Count == 0)
                        break;

                    var target = ability.targets[0];

                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = owner.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);

                    {
                        var openWarnBoards = this.manager.Blackboard.GetOrCreateData<List<Entity>>("AbilityOpenWarnBoards");
                        var tsize = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Tsize);
                        var warnBoard = WarningBoardManager.Instance.Rent(this.info.AssetName);
                        openWarnBoards.Add(warnBoard);

                        warnBoard.transform.LocalScale = new Vector3(tsize, 1, tsize);
                        warnBoard.transform.Position = target.transform.Position;
                        warnBoard.Active = true;
                    }

                    break;
                }
            }
        }
    }
}