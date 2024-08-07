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



namespace Hsenl.ability
{ 

public sealed partial class TbAbilityTraitConfig
{
    private readonly Dictionary<int, ability.AbilityTraitConfig> _dataMap;
    private readonly List<ability.AbilityTraitConfig> _dataList;
    
    public TbAbilityTraitConfig(JSONNode _json)
    {
        _dataMap = new Dictionary<int, ability.AbilityTraitConfig>();
        _dataList = new List<ability.AbilityTraitConfig>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = ability.AbilityTraitConfig.DeserializeAbilityTraitConfig(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, ability.AbilityTraitConfig> DataMap => _dataMap;
    public List<ability.AbilityTraitConfig> DataList => _dataList;

    public ability.AbilityTraitConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public ability.AbilityTraitConfig Get(int key) => _dataMap[key];
    public ability.AbilityTraitConfig this[int key] => _dataMap[key];

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