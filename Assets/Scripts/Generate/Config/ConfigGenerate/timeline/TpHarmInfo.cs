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



namespace Hsenl.timeline
{ 

public abstract partial class TpHarmInfo :  timeline.TimePointInfo 
{
    public TpHarmInfo(JSONNode _json)  : base(_json) 
    {
        { var __json0 = _json["harm_formulas"]; if(!__json0.IsArray) { throw new SerializationException(); } HarmFormulas = new System.Collections.Generic.List<numeric.DamageFormulaInfo>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { numeric.DamageFormulaInfo __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = numeric.DamageFormulaInfo.DeserializeDamageFormulaInfo(__e0);  }  HarmFormulas.Add(__v0); }   }
        PostInit();
    }

    public TpHarmInfo(int model, float point, System.Collections.Generic.List<numeric.DamageFormulaInfo> harm_formulas )  : base(model,point) 
    {
        this.HarmFormulas = harm_formulas;
        PostInit();
    }

    public static TpHarmInfo DeserializeTpHarmInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "HarmOfTargetedInfo": return new timeline.HarmOfTargetedInfo(_json);
            case "HarmOfTargetedBoltInfo": return new timeline.HarmOfTargetedBoltInfo(_json);
            case "HarmOfDirectionBoltInfo": return new timeline.HarmOfDirectionBoltInfo(_json);
            case "HarmOfPointBoltInfo": return new timeline.HarmOfPointBoltInfo(_json);
            default: throw new SerializationException();
        }
    }

    /// <summary>
    /// 伤害的配方(可造成多段伤害)
    /// </summary>
    public System.Collections.Generic.List<numeric.DamageFormulaInfo> HarmFormulas { get; private set; }


    public override void Resolve(Dictionary<string, object> _tables)
    {
        base.Resolve(_tables);
        foreach(var _e in HarmFormulas) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public override void TranslateText(System.Func<string, string, string> translator)
    {
        base.TranslateText(translator);
        foreach(var _e in HarmFormulas) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "Model:" + Model + ","
        + "Point:" + Point + ","
        + "HarmFormulas:" + Bright.Common.StringUtil.CollectionToString(HarmFormulas) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
