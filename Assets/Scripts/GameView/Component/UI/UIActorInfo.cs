using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hsenl.View {
    // 角色信息UI, 包括属性, 技能之类的信息
    public class UIActorInfo : UI<UIActorInfo> {
        public RectTransform statsElementHolder;
        public RectTransform statsElementTemplate;
        public RectTransform abilityElementHolder;
        public RectTransform abilityElementTemplate;

        public void FillInActor(Actor actor) {
            var locationValue = Localization.Instance.Value;
            var localizationConfig = Tables.Instance.TbLocalizationConfig.GetByAlias(locationValue);
            
            var numerator = actor.GetComponent<Numerator>();
            var numericTypes = numerator.GetAllFinalNumericTypes();
            this.statsElementHolder.NormalizeChildren(this.statsElementTemplate, numericTypes.Length);
            for (int i = 0, len = numericTypes.Length; i < len; i++) {
                var element = this.statsElementHolder.GetChild(i);
                var numType = numericTypes[i];
                if (!Enum.IsDefined(typeof(NumericType), (int)numType))
                    continue;

                var numericType = (NumericType)numType;
                string numericName = null;
                localizationConfig?.NumericTypeMap.TryGetValue(numericType, out numericName);
                numericName ??= numericType.ToString();
                element.Find("NameText").GetComponent<Text>().text = numericName;
                element.Find("ValueText").GetComponent<Text>().text = numerator.GetFinalValue(numType).ToString();
            }

            var abiBar = actor.FindScopeInBodied<AbilityBar>();
            var abilities = abiBar.Abilities.Where(x => !x.Tags.Contains(TagType.AbilityImplicit)).ToArray(); // 获得所有非隐式技能
            this.abilityElementHolder.NormalizeChildren(this.abilityElementTemplate, abilities.Length);
            for (int i = 0, len = abilities.Length; i < len; i++) {
                var abi = abilities[i];
                var element = this.abilityElementHolder.GetChild(i);
                var eventListener = element.Find("Icon").GetOrAddComponent<UnityEventListener>();
                eventListener.onEnter = data => {
                    var abiInfoUI = UIManager.SingleOpen<UIAbilityInfo>(UILayer.High);
                    abiInfoUI.FillIn(abi);
                };
                eventListener.onExit = data => { UIManager.SingleClose<UIAbilityInfo>(); };
                element.Find("NameText").GetComponent<Text>().text = abilities[i].ViewName;
            }
        }
    }
}