using System.Collections.Generic;
using Hsenl.timeline;
using MemoryPack;

namespace Hsenl.View {
    [MemoryPackable]
    public partial class TpOpenWarningBoard2 : TpInfo<OpenWarningBoard2Info> {
        private readonly List<Numerator> _numerators = new(2);
        
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.MainBodied;
                    if (owner == null)
                        break;

                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = owner.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);

                    
                    {
                        var ownerTran = owner.transform;
                        var openWarnBoards = this.manager.Blackboard.GetOrCreateData<List<Entity>>("AbilityOpenWarnBoards");
                        var tsize = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Tsize);
                        var crange = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Crange);
                        var warnBoard = WarningBoardManager.Instance.Rent(this.info.AssetName);
                        openWarnBoards.Add(warnBoard);

                        warnBoard.transform.LocalScale = new Vector3(tsize, 1, crange);
                        warnBoard.transform.Quaternion = ownerTran.Quaternion;
                        warnBoard.transform.Position = ownerTran.Position + Vector3.Forward * crange * 0.5f * ownerTran.Quaternion;

                        warnBoard.SetParent(ability.Entity);
                        warnBoard.Active = true;
                    }

                    break;
                }
            }
        }
    }
}