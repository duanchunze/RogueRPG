using System;
using System.Collections.Generic;
using Hsenl.numeric;

namespace Hsenl {
    namespace EventType {
        public struct OnEntityTagsChanged {
            public Entity entity;
        }
    }

    public static class Shortcut {
        #region Status Related

        /// <summary>
        /// 施加一个状态
        /// </summary>
        /// <param name="inflictor"></param>
        /// <param name="target"></param>
        /// <param name="alias"></param>
        /// <param name="duration"></param>
        /// <param name="replaceActionInfo">允许替换该状态原本的config action info</param>
        /// <param name="stack">施加层数</param>
        /// <returns></returns>
        public static Status InflictionStatus(Bodied inflictor, Bodied target, string alias, float duration = float.MinValue,
            timeline.TimeActionInfo replaceActionInfo = null, int stack = 1) {
            var statusBar = target.FindBodiedInIndividual<StatusBar>();
            var config = Tables.Instance.TbStatusConfig.GetByAlias(alias);
            var status = statusBar.GetStatus(config.Alias);
            if (status == null) {
                status = StatusFactory.CreateActorStatus(config);
                statusBar.AddStatus(status);
            }

            if (replaceActionInfo != null) {
                var actionType = replaceActionInfo.GetType();
                Foreach(status.GetComponent<TimeLine>().EntryNode);

                void Foreach(INode node) {
                    // 找到对应的actionInfo所在的action, 并替换其actionInfo, action里的数据并不是独立的, 而是共享info里的数据.
                    foreach (var child in node.ForeachChildren()) {
                        if (child is not IConfigInfoInitializer configInfoInitializer)
                            continue;

                        if (configInfoInitializer.InfoType == actionType) {
                            configInfoInitializer.InitInfo(replaceActionInfo);
                        }

                        Foreach(child);
                    }
                }
            }

            status.inflictor = inflictor;
            if (duration > float.MinValue) {
                status.GetComponent<TimeLine>().StageTillTime = duration;
            }
            else {
                status.GetComponent<TimeLine>().StageTillTime = status.Config.Duration;
            }

            var numerator = status.GetComponent<Numerator>();
            var max = numerator.GetValue(NumericType.MaxStack);
            if (max != 0) {
                var cur = numerator.GetValue(NumericType.Stack);
                cur += stack;
                if (cur > max)
                    cur = max;
                numerator.SetValue(NumericType.Stack, cur + stack);
            }

            status.Begin();
            return status;
        }

        /// 终止一个状态
        public static Status TerminationStatus(Bodied remover, Bodied target, string alias) {
            var statusHolder = target.FindBodiedInIndividual<StatusBar>();
            var status = statusHolder.GetStatus(alias);
            status?.Finish();
            return status;
        }

        public static Num GetStack(this Status self) {
            var numerator = self.GetComponent<Numerator>();
            return numerator.GetValue(NumericType.Stack);
        }

        #endregion

        #region Numerator Related

        /// <returns>获取当前血量的百分比</returns>
        /// <exception cref="ArgumentException"></exception>
        public static float GetHealthPct(Numerator numerator) {
            var hp = numerator.GetValue(NumericType.Hp);
            var max = numerator.GetValue(NumericType.MaxHp);
            if (max == 0)
                return 0;
            return hp / max;
        }

        /// <returns>加完后有多少</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int RecoverHealth(Numerator numerator, int value) {
            var cur = numerator.GetValue(NumericType.Hp);
            var max = numerator.GetValue(NumericType.MaxHp);
            if (value < 0) {
                throw new ArgumentException($"recover health is cant less than 0 '{value}'");
            }

            if (value > max - cur) {
                value = max - cur;
            }

            var final = cur + value;
            numerator.SetValue(NumericType.Hp, final);
            return final;
        }

        /// <returns>减完后还多少</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int SubtractHealth(Numerator numerator, int value) {
            var cur = numerator.GetValue(NumericType.Hp);
            if (value < 0) {
                throw new ArgumentException($"sub health is cant less than 0 '{value}'");
            }

            if (value > cur) {
                value = cur;
            }

            var final = cur - value;
            numerator.SetValue(NumericType.Hp, final);
            return final;
        }

        // public static (int curr, int max) RecoverEnergy(Numerator numerator, int value) {
        //     var cur = numerator.GetValue(NumericType.Energy);
        //     var max = numerator.GetValue(NumericType.MaxEnergy);
        //     if (value < 0) {
        //         throw new ArgumentException($"recover energy is cant less than 0 '{value}'");
        //     }
        //
        //     if (value > max - cur) {
        //         value = max - cur;
        //     }
        //
        //     var final = cur + value;
        //     numerator.SetValue(NumericType.Energy, final);
        //     return (final, max);
        // }
        //
        // public static int SubtractEnergy(Numerator numerator, int value) {
        //     var cur = numerator.GetValue(NumericType.Energy);
        //     if (value < 0) {
        //         throw new ArgumentException($"sub energy is cant less than 0 '{value}'");
        //     }
        //
        //     if (value > cur) {
        //         value = cur;
        //     }
        //
        //     var final = cur - value;
        //     numerator.SetValue(NumericType.Energy, final);
        //     return final;
        // }

        public static float GetManaPct(Numerator numerator) {
            var hp = numerator.GetValue(NumericType.Mana);
            var max = numerator.GetValue(NumericType.MaxMana);
            if (max == 0)
                return 0;
            return hp / max;
        }

        /// <returns>加完后的数值, mana的最大数值</returns>
        /// <exception cref="ArgumentException"></exception>
        public static (int curr, int max) RecoverMana(Numerator numerator, int value) {
            var cur = numerator.GetValue(NumericType.Mana);
            var max = numerator.GetValue(NumericType.MaxMana);
            if (value < 0) {
                throw new ArgumentException($"recover mana is cant less than 0 '{value}'");
            }

            if (value > max - cur) {
                value = max - cur;
            }

            var final = cur + value;
            numerator.SetValue(NumericType.Mana, final);
            return (final, max);
        }

        /// <returns>减完后的数值</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int SubtractMana(Numerator numerator, int value) {
            var cur = numerator.GetValue(NumericType.Mana);
            if (value < 0) {
                throw new ArgumentException($"sub mana is cant less than 0 '{value}'");
            }

            if (value > cur) {
                value = cur;
            }

            var final = cur - value;
            numerator.SetValue(NumericType.Mana, final);
            return final;
        }

        public static float GetExpPct(Numerator numerator) {
            var cur = numerator.GetValue(NumericType.Exp);
            var max = numerator.GetValue(NumericType.MaxExp);
            if (max == 0)
                return 0f;

            cur.ToFloat();
            return cur / max;
        }

        /// <returns>实际添加了多少经验</returns>
        /// <exception cref="ArgumentException"></exception>
        public static long AddExp(Numerator numerator, long value) {
            if (!numerator.IsHasValue(NumericType.Exp) && !numerator.IsHasValue(NumericType.MaxExp))
                return -1;

            var cur = numerator.GetValue(NumericType.Exp);
            var max = numerator.GetValue(NumericType.MaxExp);
            if (value < 0) {
                throw new ArgumentException($"recover exp is cant less than 0 '{value}'");
            }

            if (value > max - cur) {
                value = max - cur;
            }

            var final = cur + value;
            numerator.SetValue(NumericType.Exp, final);
            return value;
        }

        /// <returns>实际扣除了多少经验</returns>
        /// <exception cref="ArgumentException"></exception>
        public static long SubtractExp(Numerator numerator, long value) {
            var cur = numerator.GetValue(NumericType.Exp);
            if (value < 0) {
                throw new ArgumentException($"sub exp is cant less than 0 '{value}'");
            }

            if (value > cur) {
                value = cur;
            }

            var final = cur - value;
            numerator.SetValue(NumericType.Exp, final);
            return value;
        }

        public static Num GetLv(Numerator numerator) {
            var lv = numerator.GetValue(NumericType.LV);
            return lv;
        }

        public static long AddLv(Numerator numerator, long value) {
            var cur = numerator.GetValue(NumericType.LV);
            if (value < 0) {
                throw new ArgumentException($"recover exp is cant less than 0 '{value}'");
            }

            var final = cur + value;
            numerator.SetValue(NumericType.LV, final);
            return final;
        }

        #endregion

        #region Pickup

        public static void Pickup(Picker picker, Pickable pickable) {
            picker.Pickup(pickable);
        }

        #endregion

        #region Ability

        public static bool IsAbilityAutoCast(Ability ability) {
            var controlTrigger = ability.GetComponent<ControlTrigger>();
            return controlTrigger.ControlCode == (int)ControlCode.AutoTrigger;
        }

        public static void OpenAbilityAutoCast(Ability ability) {
            var controlTrigger = ability.GetComponent<ControlTrigger>();
            controlTrigger.ControlCode = (int)ControlCode.AutoTrigger;
            ability.FindScopeInParent<AbilitesBar>()?.Changed();
        }

        public static void CloseAbilityAutoCast(Ability ability) {
            var controlTrigger = ability.GetComponent<ControlTrigger>();
            controlTrigger.ControlCode = (int)ControlCode.None;
            ability.FindScopeInParent<AbilitesBar>()?.Changed();
        }

        #endregion

        public static bool IsMainMan(Bodied bodied) {
            return bodied == GameManager.Instance.MainMan;
        }

        public static void TakeDamage(Harmable harmable, Hurtable hurtable, Component source, DamageFormulaInfo damageFormulaInfo,
            List<ProcedureLine> procedureLines, string hitfx, string hitsound, float damageRatio = 1f) {
            var damageForm = new PliHarmForm() {
                harmable = harmable,
                hurtable = hurtable,
                source = source,
                damageFormulaInfo = damageFormulaInfo,
                damageRatio = damageRatio,
                hitfx = hitfx,
                hitsound = hitsound,
            };

            ProcedureLine.MergeStartLineAsync(procedureLines, damageForm, userToken: procedureLines).Tail();
        }

        public static bool IsDead(Bodied bodied) {
            var priorities = bodied.GetComponent<Prioritizer>();
            if (priorities == null) return false;
            return priorities.ContainsState(StatusAlias.Death);
        }

        public static void AddTag(this Entity self, TagType tag) {
            self.Tags.Add(tag);
            EventSystem.Publish(new EventType.OnEntityTagsChanged() { entity = self });
        }

        public static void RemoveTag(this Entity self, TagType tag) {
            self.Tags.Remove(tag);
            EventSystem.Publish(new EventType.OnEntityTagsChanged() { entity = self });
        }
    }
}