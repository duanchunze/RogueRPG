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

public sealed partial class HarmOfDibozhanBoltInfo :  timeline.TpHarmInfo 
{
    public HarmOfDibozhanBoltInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["harm_formula"].IsObject) { throw new SerializationException(); }  HarmFormula = numeric.DamageFormulaInfo.DeserializeDamageFormulaInfo(_json["harm_formula"]);  }
        { if(!_json["bolt_config_alias"].IsString) { throw new SerializationException(); }  BoltConfigAlias = _json["bolt_config_alias"]; }
        { if(!_json["num"].IsNumber) { throw new SerializationException(); }  Num = _json["num"]; }
        { if(!_json["internal_time"].IsNumber) { throw new SerializationException(); }  InternalTime = _json["internal_time"]; }
        { if(!_json["internal_distance"].IsNumber) { throw new SerializationException(); }  InternalDistance = _json["internal_distance"]; }
        PostInit();
    }

    public HarmOfDibozhanBoltInfo(int model, float point, numeric.DamageFormulaInfo harm_formula, string bolt_config_alias, int num, float internal_time, float internal_distance )  : base(model,point) 
    {
        this.HarmFormula = harm_formula;
        this.BoltConfigAlias = bolt_config_alias;
        this.Num = num;
        this.InternalTime = internal_time;
        this.InternalDistance = internal_distance;
        PostInit();
    }

    public static HarmOfDibozhanBoltInfo DeserializeHarmOfDibozhanBoltInfo(JSONNode _json)
    {
        return new timeline.HarmOfDibozhanBoltInfo(_json);
    }

    /// <summary>
    /// 伤害的配方(可造成多段伤害)((list#sep=|),numeric.DamageFormulaInfo#sep=&gt;)
    /// </summary>
    public numeric.DamageFormulaInfo HarmFormula { get; private set; }
    public string BoltConfigAlias { get; private set; }
    /// <summary>
    /// 伤害次数
    /// </summary>
    public int Num { get; private set; }
    public float InternalTime { get; private set; }
    public float InternalDistance { get; private set; }

    public const int __ID__ = -2093156632;
    public override int GetTypeId() => __ID__;

    public override void Resolve(Dictionary<string, object> _tables)
    {
        base.Resolve(_tables);
        HarmFormula?.Resolve(_tables);
        PostResolve();
    }

    public override void TranslateText(System.Func<string, string, string> translator)
    {
        base.TranslateText(translator);
        HarmFormula?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Model:" + Model + ","
        + "Point:" + Point + ","
        + "HarmFormula:" + HarmFormula + ","
        + "BoltConfigAlias:" + BoltConfigAlias + ","
        + "Num:" + Num + ","
        + "InternalTime:" + InternalTime + ","
        + "InternalDistance:" + InternalDistance + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
