//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;


namespace Hsenl
{ 
   
public sealed partial class Tables
{
    public card.TbCardSingletonConfig TbCardSingletonConfig {get; }
    public game.TbGameSingletonConfig TbGameSingletonConfig {get; }
    public synthesis.TbSynthesisConfig TbSynthesisConfig {get; }
    public faction.TbFactionConfig TbFactionConfig {get; }
    public actor.TbActorConfig TbActorConfig {get; }
    public pickable.TbPickableConfig TbPickableConfig {get; }
    public ability.TbAbilityConfig TbAbilityConfig {get; }
    public ability.TbAbilityPatchConfig TbAbilityPatchConfig {get; }
    public ability.TbAbilityTraitConfig TbAbilityTraitConfig {get; }
    public ability_assist.TbAbilityAssistConfig TbAbilityAssistConfig {get; }
    public status.TbStatusConfig TbStatusConfig {get; }
    public numeric.TbNumericActorConfig TbNumericActorConfig {get; }
    public ai.TbAIConfig TbAIConfig {get; }
    public checkpoint.TbCheckpointConfig TbCheckpointConfig {get; }
    public adventure.TbAdventureConfig TbAdventureConfig {get; }
    public bolt.TbBoltConfig TbBoltConfig {get; }
    public localization.TbLocalizationConfig TbLocalizationConfig {get; }
    public localization.TbLocalizationAbilityConfig TbLocalizationAbilityConfig {get; }
    public localization.TbLocalizationAbilityPatchConfig TbLocalizationAbilityPatchConfig {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbCardSingletonConfig = new card.TbCardSingletonConfig(loader("card_tbcardsingletonconfig")); 
        tables.Add("card.TbCardSingletonConfig", TbCardSingletonConfig);
        TbGameSingletonConfig = new game.TbGameSingletonConfig(loader("game_tbgamesingletonconfig")); 
        tables.Add("game.TbGameSingletonConfig", TbGameSingletonConfig);
        TbSynthesisConfig = new synthesis.TbSynthesisConfig(loader("synthesis_tbsynthesisconfig")); 
        tables.Add("synthesis.TbSynthesisConfig", TbSynthesisConfig);
        TbFactionConfig = new faction.TbFactionConfig(loader("faction_tbfactionconfig")); 
        tables.Add("faction.TbFactionConfig", TbFactionConfig);
        TbActorConfig = new actor.TbActorConfig(loader("actor_tbactorconfig")); 
        tables.Add("actor.TbActorConfig", TbActorConfig);
        TbPickableConfig = new pickable.TbPickableConfig(loader("pickable_tbpickableconfig")); 
        tables.Add("pickable.TbPickableConfig", TbPickableConfig);
        TbAbilityConfig = new ability.TbAbilityConfig(loader("ability_tbabilityconfig")); 
        tables.Add("ability.TbAbilityConfig", TbAbilityConfig);
        TbAbilityPatchConfig = new ability.TbAbilityPatchConfig(loader("ability_tbabilitypatchconfig")); 
        tables.Add("ability.TbAbilityPatchConfig", TbAbilityPatchConfig);
        TbAbilityTraitConfig = new ability.TbAbilityTraitConfig(loader("ability_tbabilitytraitconfig")); 
        tables.Add("ability.TbAbilityTraitConfig", TbAbilityTraitConfig);
        TbAbilityAssistConfig = new ability_assist.TbAbilityAssistConfig(loader("ability_assist_tbabilityassistconfig")); 
        tables.Add("ability_assist.TbAbilityAssistConfig", TbAbilityAssistConfig);
        TbStatusConfig = new status.TbStatusConfig(loader("status_tbstatusconfig")); 
        tables.Add("status.TbStatusConfig", TbStatusConfig);
        TbNumericActorConfig = new numeric.TbNumericActorConfig(loader("numeric_tbnumericactorconfig")); 
        tables.Add("numeric.TbNumericActorConfig", TbNumericActorConfig);
        TbAIConfig = new ai.TbAIConfig(loader("ai_tbaiconfig")); 
        tables.Add("ai.TbAIConfig", TbAIConfig);
        TbCheckpointConfig = new checkpoint.TbCheckpointConfig(loader("checkpoint_tbcheckpointconfig")); 
        tables.Add("checkpoint.TbCheckpointConfig", TbCheckpointConfig);
        TbAdventureConfig = new adventure.TbAdventureConfig(loader("adventure_tbadventureconfig")); 
        tables.Add("adventure.TbAdventureConfig", TbAdventureConfig);
        TbBoltConfig = new bolt.TbBoltConfig(loader("bolt_tbboltconfig")); 
        tables.Add("bolt.TbBoltConfig", TbBoltConfig);
        TbLocalizationConfig = new localization.TbLocalizationConfig(loader("localization_tblocalizationconfig")); 
        tables.Add("localization.TbLocalizationConfig", TbLocalizationConfig);
        TbLocalizationAbilityConfig = new localization.TbLocalizationAbilityConfig(loader("localization_tblocalizationabilityconfig")); 
        tables.Add("localization.TbLocalizationAbilityConfig", TbLocalizationAbilityConfig);
        TbLocalizationAbilityPatchConfig = new localization.TbLocalizationAbilityPatchConfig(loader("localization_tblocalizationabilitypatchconfig")); 
        tables.Add("localization.TbLocalizationAbilityPatchConfig", TbLocalizationAbilityPatchConfig);
        PostInit();

        TbCardSingletonConfig.Resolve(tables); 
        TbGameSingletonConfig.Resolve(tables); 
        TbSynthesisConfig.Resolve(tables); 
        TbFactionConfig.Resolve(tables); 
        TbActorConfig.Resolve(tables); 
        TbPickableConfig.Resolve(tables); 
        TbAbilityConfig.Resolve(tables); 
        TbAbilityPatchConfig.Resolve(tables); 
        TbAbilityTraitConfig.Resolve(tables); 
        TbAbilityAssistConfig.Resolve(tables); 
        TbStatusConfig.Resolve(tables); 
        TbNumericActorConfig.Resolve(tables); 
        TbAIConfig.Resolve(tables); 
        TbCheckpointConfig.Resolve(tables); 
        TbAdventureConfig.Resolve(tables); 
        TbBoltConfig.Resolve(tables); 
        TbLocalizationConfig.Resolve(tables); 
        TbLocalizationAbilityConfig.Resolve(tables); 
        TbLocalizationAbilityPatchConfig.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbCardSingletonConfig.TranslateText(translator); 
        TbGameSingletonConfig.TranslateText(translator); 
        TbSynthesisConfig.TranslateText(translator); 
        TbFactionConfig.TranslateText(translator); 
        TbActorConfig.TranslateText(translator); 
        TbPickableConfig.TranslateText(translator); 
        TbAbilityConfig.TranslateText(translator); 
        TbAbilityPatchConfig.TranslateText(translator); 
        TbAbilityTraitConfig.TranslateText(translator); 
        TbAbilityAssistConfig.TranslateText(translator); 
        TbStatusConfig.TranslateText(translator); 
        TbNumericActorConfig.TranslateText(translator); 
        TbAIConfig.TranslateText(translator); 
        TbCheckpointConfig.TranslateText(translator); 
        TbAdventureConfig.TranslateText(translator); 
        TbBoltConfig.TranslateText(translator); 
        TbLocalizationConfig.TranslateText(translator); 
        TbLocalizationAbilityConfig.TranslateText(translator); 
        TbLocalizationAbilityPatchConfig.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}