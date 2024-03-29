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
    public faction.TbFactionConfig TbFactionConfig {get; }
    public actor.TbActorConfig TbActorConfig {get; }
    public card.TbCardConfig TbCardConfig {get; }
    public card.TbCardSynthesisConfig TbCardSynthesisConfig {get; }
    public pickable.TbPickableConfig TbPickableConfig {get; }
    public ability.TbAbilityConfig TbAbilityConfig {get; }
    public ability_upgrade.TbAbilityUpgradeConfig TbAbilityUpgradeConfig {get; }
    public ability_assist.TbAbilityAssistConfig TbAbilityAssistConfig {get; }
    public status.TbStatusConfig TbStatusConfig {get; }
    public numeric.TbNumericActorConfig TbNumericActorConfig {get; }
    public ai.TbAIConfig TbAIConfig {get; }
    public checkpoint.TbCheckpointConfig TbCheckpointConfig {get; }
    public adventure.TbAdventureConfig TbAdventureConfig {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbCardSingletonConfig = new card.TbCardSingletonConfig(loader("card_tbcardsingletonconfig")); 
        tables.Add("card.TbCardSingletonConfig", TbCardSingletonConfig);
        TbGameSingletonConfig = new game.TbGameSingletonConfig(loader("game_tbgamesingletonconfig")); 
        tables.Add("game.TbGameSingletonConfig", TbGameSingletonConfig);
        TbFactionConfig = new faction.TbFactionConfig(loader("faction_tbfactionconfig")); 
        tables.Add("faction.TbFactionConfig", TbFactionConfig);
        TbActorConfig = new actor.TbActorConfig(loader("actor_tbactorconfig")); 
        tables.Add("actor.TbActorConfig", TbActorConfig);
        TbCardConfig = new card.TbCardConfig(loader("card_tbcardconfig")); 
        tables.Add("card.TbCardConfig", TbCardConfig);
        TbCardSynthesisConfig = new card.TbCardSynthesisConfig(loader("card_tbcardsynthesisconfig")); 
        tables.Add("card.TbCardSynthesisConfig", TbCardSynthesisConfig);
        TbPickableConfig = new pickable.TbPickableConfig(loader("pickable_tbpickableconfig")); 
        tables.Add("pickable.TbPickableConfig", TbPickableConfig);
        TbAbilityConfig = new ability.TbAbilityConfig(loader("ability_tbabilityconfig")); 
        tables.Add("ability.TbAbilityConfig", TbAbilityConfig);
        TbAbilityUpgradeConfig = new ability_upgrade.TbAbilityUpgradeConfig(loader("ability_upgrade_tbabilityupgradeconfig")); 
        tables.Add("ability_upgrade.TbAbilityUpgradeConfig", TbAbilityUpgradeConfig);
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
        PostInit();

        TbCardSingletonConfig.Resolve(tables); 
        TbGameSingletonConfig.Resolve(tables); 
        TbFactionConfig.Resolve(tables); 
        TbActorConfig.Resolve(tables); 
        TbCardConfig.Resolve(tables); 
        TbCardSynthesisConfig.Resolve(tables); 
        TbPickableConfig.Resolve(tables); 
        TbAbilityConfig.Resolve(tables); 
        TbAbilityUpgradeConfig.Resolve(tables); 
        TbAbilityAssistConfig.Resolve(tables); 
        TbStatusConfig.Resolve(tables); 
        TbNumericActorConfig.Resolve(tables); 
        TbAIConfig.Resolve(tables); 
        TbCheckpointConfig.Resolve(tables); 
        TbAdventureConfig.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbCardSingletonConfig.TranslateText(translator); 
        TbGameSingletonConfig.TranslateText(translator); 
        TbFactionConfig.TranslateText(translator); 
        TbActorConfig.TranslateText(translator); 
        TbCardConfig.TranslateText(translator); 
        TbCardSynthesisConfig.TranslateText(translator); 
        TbPickableConfig.TranslateText(translator); 
        TbAbilityConfig.TranslateText(translator); 
        TbAbilityUpgradeConfig.TranslateText(translator); 
        TbAbilityAssistConfig.TranslateText(translator); 
        TbStatusConfig.TranslateText(translator); 
        TbNumericActorConfig.TranslateText(translator); 
        TbAIConfig.TranslateText(translator); 
        TbCheckpointConfig.TranslateText(translator); 
        TbAdventureConfig.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}