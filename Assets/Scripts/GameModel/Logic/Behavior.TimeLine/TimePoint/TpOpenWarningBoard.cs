﻿using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpOpenWarningBoard : TpInfo<timeline.OpenWarningBoardInfo> {
        private readonly List<Numerator> _numerators = new(2);

        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var owner = ability.AttachedBodied;
                    if (owner == null)
                        break;

                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = owner?.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);

                    var tsize = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Tsize);

                    var openWarnBoards = this.manager.Blackboard.GetOrCreateData<List<Entity>>("AbilityOpenWarnBoards");
                    var directions = this.manager.Blackboard.GetOrCreateData<List<Vector3>>("AbilityCastDirections");
                    directions.Clear();
                    foreach (var target in ability.targets) {
                        var dir = target.transform.Position - owner.transform.Position;
                        var warnBoard = WarningBoardManager.Instance.Rent(this.info.WarnName);
                        openWarnBoards.Add(warnBoard);
                        warnBoard.transform.Position = owner.transform.Position;
                        switch (this.info.Type) {
                            case 0: {
                                warnBoard.transform.LocalScale = Vector3.one * tsize;
                                break;
                            }

                            case 1: {
                                var crange = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Crange);
                                warnBoard.transform.LocalScale = new Vector3(tsize, 1, crange);
                                break;
                            }
                        }

                        warnBoard.transform.LookAt(dir);
                        directions.Add(dir.normalized);
                        warnBoard.Active = true;
                    }

                    break;
                }
            }
        }

        protected override void OnNodeAbort() {
            var list = this.manager.Blackboard.GetOrCreateData<List<Entity>>("AbilityOpenWarnBoards");
            for (int i = list.Count - 1; i >= 0; i--) {
                var entity = list[i];
                if (entity.Name == this.info.WarnName) {
                    WarningBoardManager.Instance.Return(entity);
                    list.RemoveAt(i);
                }
            }
        }
    }
}