//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace Hsenl.localization
{ 

public sealed partial class TbLocalizationAbilityPatchConfig
{
    private readonly Dictionary<string, localization.LocalizationAbilityPatchConfig> _dataMap;
    private readonly List<localization.LocalizationAbilityPatchConfig> _dataList;
    
    public TbLocalizationAbilityPatchConfig(JSONNode _json)
    {
        _dataMap = new Dictionary<string, localization.LocalizationAbilityPatchConfig>();
        _dataList = new List<localization.LocalizationAbilityPatchConfig>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = localization.LocalizationAbilityPatchConfig.DeserializeLocalizationAbilityPatchConfig(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Alias, _v);
        }
        PostInit();
    }

    public Dictionary<string, localization.LocalizationAbilityPatchConfig> DataMap => _dataMap;
    public List<localization.LocalizationAbilityPatchConfig> DataList => _dataList;

    public localization.LocalizationAbilityPatchConfig GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public localization.LocalizationAbilityPatchConfig Get(string key) => _dataMap[key];
    public localization.LocalizationAbilityPatchConfig this[string key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}