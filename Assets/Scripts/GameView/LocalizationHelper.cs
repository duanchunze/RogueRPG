﻿using System;
using Hsenl.ability;
using Hsenl.card;
using Hsenl.item;
using Hsenl.localization;

namespace Hsenl.View {
    public static class LocalizationHelper {
        private static string _localizationValue;

        private static LocalizationConfig _localizationConfig;

        private const string Null = "<NULL>";

        private static LocalizationConfig LocalizationConfig {
            get {
                if (Localization.Instance?.Value == null)
                    return null;

                var v = Localization.Instance.Value;
                if (_localizationValue == v) {
                    return _localizationConfig;
                }

                _localizationValue = v;

                _localizationConfig = Tables.Instance.TbLocalizationConfig.Get(v);
                return _localizationConfig;
            }
        }


        public static string GetStringOrDefault<T>(T e) where T : Enum {
            var config = LocalizationConfig;
            if (config == null)
                return e.ToString();

            switch (e) {
                case LocalizationKey key: {
                    return config.Map.GetValueOrDefault(key, key.ToString());
                }

                case TagType tag: {
                    return config.TagTypeMap.GetValueOrDefault(tag, tag.ToString());
                }

                case DamageType damageType: {
                    return config.DamageTypeMap.GetValueOrDefault(damageType, damageType.ToString());
                }

                case NumericType numericType: {
                    return config.NumericTypeMap.GetValueOrDefault(numericType, numericType.ToString());
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(e));
            }
        }

        public static string GetStringOrDefault(Bright.Config.BeanBase bean) {
            var config = LocalizationConfig;
            if (config == null)
                return Null;

            switch (bean) {
                case behavior.Info behaviorInfo: {
                    return config.BeansBehaviorMap.GetValueOrDefault(bean.GetType().Name, Null);
                }

                case procedureline.Info procedurelineInfo: {
                    return config.BeansProcedurelineMap.GetValueOrDefault(bean.GetType().Name, Null);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(bean));
            }
        }

        // ability ---------------------------------

        public static LocalizationAbilityConfig GetLocalizationAbilityConfig(AbilityConfig abilityConfig) {
            if (Localization.Instance?.Value == null)
                return null;

            var config = Tables.Instance.TbLocalizationAbilityConfig.Get(abilityConfig.Alias);
            if (config == null) {
                Log.Error($"Cant not find LocalizationAbility by '{abilityConfig.Alias}'");
            }

            return config;
        }

        public static string GetAbilityLocalizationName(AbilityConfig self) {
            var localizationConfig = GetLocalizationAbilityConfig(self);
            if (localizationConfig == null)
                return null;

            return localizationConfig.Name.GetValueOrDefault(Localization.Instance?.Value);
        }

        public static string GetAbilityLocalizationDesc(AbilityConfig self) {
            var localizationConfig = GetLocalizationAbilityConfig(self);
            if (localizationConfig == null)
                return null;

            return localizationConfig.Desc.GetValueOrDefault(Localization.Instance?.Value);
        }

        // ability patch ----------------------------

        public static LocalizationAbilityPatchConfig GetLocalizationAbilityPatchConfig(AbilityPatchConfig abilityPatchConfig) {
            if (Localization.Instance?.Value == null)
                return null;

            var config = Tables.Instance.TbLocalizationAbilityPatchConfig.Get(abilityPatchConfig.Alias);
            if (config == null) {
                Log.Error($"Cant not find LocalizationAbilityPatch by '{abilityPatchConfig.Alias}'");
            }

            return config;
        }

        public static string GetAbilityPatchLocalizationName(AbilityPatchConfig self) {
            var localizationConfig = GetLocalizationAbilityPatchConfig(self);
            if (localizationConfig == null)
                return null;

            return localizationConfig.Name.GetValueOrDefault(Localization.Instance?.Value);
        }
        
        // prop -------------------------------------
        
        public static LocalizationPropConfig GetLocalizationPropConfig(PropConfig propConfig) {
            if (Localization.Instance?.Value == null)
                return null;

            var config = Tables.Instance.TbLocalizationPropConfig.Get(propConfig.Alias);
            if (config == null) {
                Log.Error($"Cant not find LocalizationProp by '{propConfig.Alias}'");
            }

            return config;
        }

        public static string GetPropLocalizationName(PropConfig self) {
            var localizationConfig = GetLocalizationPropConfig(self);
            if (localizationConfig == null)
                return null;

            return localizationConfig.Name.GetValueOrDefault(Localization.Instance?.Value);
        }
    }
}