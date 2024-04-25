using System;

namespace Hsenl {
    namespace EventType {
        public struct OnEntityTagsChanged {
            public Entity entity;
        }
    }

    public static class Shortcut {
        #region Status Related

        public static Status InflictionStatus(Bodied inflictor, Bodied target, string alias, float duration = float.MinValue,
            timeline.TimeActionInfo actionInfo = null) {
            var statusBar = target.FindScopeInBodied<StatusBar>();
            var config = Tables.Instance.TbStatusConfig.GetByAlias(alias);
            var status = statusBar.GetStatus(config.Alias);
            if (status == null) {
                status = StatusFactory.CreateActorStatus(config);
                statusBar.AddStatus(status);
            }

            if (actionInfo != null) {
                var actionType = actionInfo.GetType();
                status.GetComponent<TimeLine>().EntryNode.ForeachChildren(child => {
                    if (child is not IConfigInfoInitializer configInfoInitializer)
                        return;

                    if (configInfoInitializer.InfoType == actionType) {
                        configInfoInitializer.InitInfo(actionInfo);
                    }
                });
            }

            status.inflictor = inflictor;
            if (duration > float.MinValue) {
                status.GetComponent<TimeLine>().TillTime = duration;
            }
            else {
                status.GetComponent<TimeLine>().TillTime = status.Config.Duration;
            }

            status.Begin();
            return status;
        }

        public static Status TerminationStatus(Bodied remover, Bodied target, string alias) {
            var statusHolder = target.FindScopeInBodied<StatusBar>();
            var status = statusHolder.GetStatus(alias);
            status?.Finish();
            return status;
        }

        #endregion

        #region Numerator Related

        public static float GetHealthPct(Numerator numerator) {
            var hp = numerator.GetValue(NumericType.Hp);
            var max = numerator.GetValue(NumericType.MaxHp);
            return hp / max;
        }

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

        public static long AddExp(Numerator numerator, long value) {
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
            return final;
        }

        public static long SubtractExp(Numerator numerator, long value) {
            var cur = numerator.GetValue(NumericType.Exp);
            if (value < 0) {
                throw new ArgumentException($"sub exp is cant less than 0 '{value}'");
            }

            if (value > cur) {
                value = cur;
            }

            var final = cur + value;
            numerator.SetValue(NumericType.Exp, final);
            return final;
        }

        #endregion

        #region Pickup

        public static void Pickup(Picker picker, Pickable pickable) {
            picker.Pickup(pickable);
        }

        #endregion

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