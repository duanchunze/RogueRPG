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



namespace Hsenl.numeric
{ 

/// <summary>
/// 示例: 造成30%物理攻击力 + 70%法术攻击力的真实伤害
/// </summary>
public sealed partial class DamageFormulaInfo :  numeric.Info 
{
    public DamageFormulaInfo(JSONNode _json)  : base(_json) 
    {
        { var __json0 = _json["damage_formulas"]; if(!__json0.IsArray) { throw new SerializationException(); } DamageFormulas = new System.Collections.Generic.List<numeric.FormulaInfo>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { numeric.FormulaInfo __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = numeric.FormulaInfo.DeserializeFormulaInfo(__e0);  }  DamageFormulas.Add(__v0); }   }
        { if(!_json["damage_type"].IsNumber) { throw new SerializationException(); }  DamageType = (DamageType)_json["damage_type"].AsInt; }
        PostInit();
    }

    public DamageFormulaInfo(System.Collections.Generic.List<numeric.FormulaInfo> damage_formulas, DamageType damage_type )  : base() 
    {
        this.DamageFormulas = damage_formulas;
        this.DamageType = damage_type;
        PostInit();
    }

    public static DamageFormulaInfo DeserializeDamageFormulaInfo(JSONNode _json)
    {
        return new numeric.DamageFormulaInfo(_json);
    }

    /// <summary>
    /// 配方
    /// </summary>
    public System.Collections.Generic.List<numeric.FormulaInfo> DamageFormulas { get; private set; }
    /// <summary>
    /// 伤害类型. 物理、法术、真实...
    /// </summary>
    public DamageType DamageType { get; private set; }

    public const int __ID__ = -1487852668;
    public override int GetTypeId() => __ID__;

    public override void Resolve(Dictionary<string, object> _tables)
    {
        base.Resolve(_tables);
        foreach(var _e in DamageFormulas) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public override void TranslateText(System.Func<string, string, string> translator)
    {
        base.TranslateText(translator);
        foreach(var _e in DamageFormulas) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "DamageFormulas:" + Bright.Common.StringUtil.CollectionToString(DamageFormulas) + ","
        + "DamageType:" + DamageType + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
