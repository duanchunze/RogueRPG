using System;
using Bright.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIAbilityInfo : UI<UIAbilityInfo> {
        public Text nameText;
        public Text cdText;
        public Text manaCostText;
        public Text descText;
        public RectTransform statsElementHolder;
        public RectTransform statsElementTemplate;

        public void FillIn(Ability ability) {
            var config = ability.Config;

            // tag信息
            // 施法信息(触发条件)
            // 目标阵营(对敌方, 对友方)

            // 基本信息
            var abiNumerator = ability.GetComponent<Numerator>();
            this.nameText.text = LocalizationHelper.GetAbilityLocalizationName(config);
            this.cdText.text = LocalizationHelper.GetStringOrDefault(LocalizationKey.cd) + $" {abiNumerator.GetValue(NumericType.CD)}";
            this.manaCostText.text = LocalizationHelper.GetStringOrDefault(LocalizationKey.mana_cost) + $" {abiNumerator.GetValue(NumericType.ManaCost)}";
            this.descText.text = LocalizationHelper.GetAbilityLocalizationDesc(config) + "\n";

            // 数值信息
            var numerator = ability.GetComponent<Numerator>();
            var keys = numerator.GetAllAttachNumericKeys();
            this.statsElementHolder.NormalizeChildren(this.statsElementTemplate, keys.Length);
            for (int i = 0, len = keys.Length; i < len; i++) {
                var element = this.statsElementHolder.GetChild(i);
                var key = keys[i];
                var tuple = NumericNodeKey.Split(key);
                if (!Enum.IsDefined(typeof(NumericType), (int)tuple.numType))
                    continue;

                var numericType = (NumericType)tuple.numType;
                string numericName = LocalizationHelper.GetStringOrDefault(numericType);
                element.Find("NameText").GetComponent<Text>().text = numericName;
                var value = numerator.GetAttachValue(tuple.numType, tuple.layer);
                var valueStr = string.Empty;
                if (value.pct != 1) {
                    valueStr += $"*{value.pct}";
                }

                valueStr += $" +{value.fix}";

                element.Find("ValueText").GetComponent<Text>().text = valueStr;
            }

            // 动作信息
            var stageLine = ability.GetComponent<StageLine>();
            string actionStr = null;
            var timeNodes = stageLine.EntryNode.GetNodesInChildren<ITimeNode>();
            string harmStr = null;
            foreach (var timeNode in timeNodes) {
                switch (timeNode) {
                    case IHarmInfo harmInfo: {
                        switch (harmInfo.HarmInfo) {
                            // 通用方案, 如果有特定方案, 可以在上面添加case
                            default: {
                                var info = harmInfo.HarmInfo;
                                var harmFormula = AssemblyHelper.GetPropertyValue<numeric.DamageFormulaInfo>(info.GetType(), "HarmFormula", info);
                                foreach (var damageFormula in harmFormula.DamageFormulas) {
                                    if (damageFormula.Fix != 0) {
                                        harmStr += LocalizationHelper.GetStringOrDefault(damageFormula.Type) +
                                                   $"(*{damageFormula.Pct} +{damageFormula.Fix})";
                                    }
                                    else {
                                        harmStr += LocalizationHelper.GetStringOrDefault(damageFormula.Type) +
                                                   $"(*{damageFormula.Pct})";
                                    }
                                }

                                harmStr += LocalizationHelper.GetStringOrDefault(harmFormula.DamageType);

                                var format = LocalizationHelper.GetStringOrDefault((BeanBase)info);
                                actionStr = string.Format(format!, $"<color=red>{harmStr}</color>");
                                break;
                            }
                        }

                        break;
                    }
                }
            }

            actionStr += "\n";
            // 特性(后缀词条)


            this.descText.text += actionStr;
        }
    }
}