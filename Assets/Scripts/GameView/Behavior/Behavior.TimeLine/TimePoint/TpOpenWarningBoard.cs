using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace Hsenl.View {
    [MemoryPackable]
    public partial class TpOpenWarningBoard : TpInfo<timeline.OpenWarningBoardInfo> {
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

                    // open warning board
                    {
                        var ownerTran = owner.transform;
                        var openWarnBoards = this.manager.Blackboard.GetOrCreateData<List<Entity>>("AbilityOpenWarnBoards");
                        var tsize = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Tsize);
                        var warnBoard = WarningBoardManager.Instance.Rent(this.info.AssetName, false);
                        openWarnBoards.Add(warnBoard);
                        
                        var localScale = new Vector3(
                            this.info.Size.X * tsize,
                            this.info.Size.Y * tsize,
                            this.info.Size.Z * tsize);
                        warnBoard.transform.LocalScale = localScale;
                        
                        var dir = new Vector3(
                            this.info.Center.X * tsize, 
                            this.info.Center.Y * tsize, 
                            this.info.Center.Z * tsize);
                        warnBoard.transform.Position = ownerTran.Position + dir * ownerTran.Quaternion;
                        warnBoard.transform.Quaternion = ownerTran.Quaternion;
                        
                        warnBoard.SetParent(ability.Entity);
                        warnBoard.Active = true;
                    }

                    break;
                }
            }
        }

        
    }
}