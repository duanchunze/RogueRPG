using System.Collections.Generic;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.SplitBolt)]
    public class PlhHarm_SplitBolt : AProcedureLineHandler<PliHarmForm, PlwSplitBoltOnHit> {
        private readonly List<SelectionTarget> _cache = new();

        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, PlwSplitBoltOnHit worker, object userToken) {
            var selector = item.harmable.GetComponent<SelectorDefault>();
            var faction = item.harmable.GetComponent<Faction>();
            IReadOnlyBitlist constrainsTags = null;
            switch (item.source) {
                case Ability ability: {
                    constrainsTags = faction.GetTagsOfFactionTypes(ability.factionTypes);
                    break;
                }
            }

            this._cache.Clear();
            var splitDir = item.hurtable.transform.Position - item.harmable.transform.Position;
            splitDir.Normalize();
            selector
                .SearcherSectorBody(item.hurtable.transform.Position, worker.info.Radius, splitDir, 120)
                .FilterAlive()
                .FilterTargets(item.hurtable.Bodied)
                .FilterTags(constrainsTags, null)
                .FilterObstacles()
                .SelectNearests(worker.info.SplitNum)
                .Wrap(this._cache);

            this.FireBolts(item, worker);


            return ProcedureLineHandleResult.Success;
        }

        private void FireBolts(PliHarmForm item, PlwSplitBoltOnHit worker) {
            if (this._cache.Count != 0) {
                for (int i = 0; i < this._cache.Count; i++) {
                    var selectionTarget = this._cache[i];
                    var hurtable = selectionTarget.GetComponent<Hurtable>();
                    if (hurtable == null)
                        continue;

                    var bolt = BoltManager.Instance.Rent(worker.info.BoltAlias, false);
                    var tspd = worker.info.Speed;
                    var tsize = worker.info.Size;
                    if (tspd <= 0) {
                        Log.Error($"bolt speed is 0 or less than 0 '{tspd}'");
                        continue;
                    }

                    this.FireAsync(item, worker, bolt, hurtable, tspd, tsize);
                }
            }
        }

        private async void FireAsync(PliHarmForm item, PlwSplitBoltOnHit worker, Bolt bolt, Hurtable hurtable, float tspd, float tsize) {
            await TpHarmOfTargetedBolt.FireToTarget(bolt, hurtable, item.hurtable.transform.Position, tspd, tsize);

            var damageForm = new PliHarmForm() {
                harmable = item.harmable,
                hurtable = hurtable,
                source = item.source,
                damageType = item.damageType,
                damage = item.damage * 0.7f,
                astun = item.astun,
                hitsound = item.hitsound,
                hitfx = item.hitfx,
            };

            // 这里用的是GM的pl, 因为自己的pl上面还带有该worker, 再造成伤害的话, 还会再触发分裂造成无限分裂
            GameManager.Instance.ProcedureLine.StartLine(damageForm);
        }
    }
}