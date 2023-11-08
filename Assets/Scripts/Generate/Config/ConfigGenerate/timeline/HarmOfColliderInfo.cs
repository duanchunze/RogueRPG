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

/// <summary>
/// 剑魔的q
/// </summary>
public sealed partial class HarmOfColliderInfo :  timeline.TsHarmInfo 
{
    public HarmOfColliderInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["harm_formula"].IsObject) { throw new SerializationException(); }  HarmFormula = numeric.DamageFormulaInfo.DeserializeDamageFormulaInfo(_json["harm_formula"]);  }
        { if(!_json["collider_name"].IsString) { throw new SerializationException(); }  ColliderName = _json["collider_name"]; }
        { if(!_json["hit_fx"].IsString) { throw new SerializationException(); }  HitFx = _json["hit_fx"]; }
        { if(!_json["hit_sound"].IsString) { throw new SerializationException(); }  HitSound = _json["hit_sound"]; }
        PostInit();
    }

    public HarmOfColliderInfo(int model, float origin, float dest, numeric.DamageFormulaInfo harm_formula, string collider_name, string hit_fx, string hit_sound )  : base(model,origin,dest) 
    {
        this.HarmFormula = harm_formula;
        this.ColliderName = collider_name;
        this.HitFx = hit_fx;
        this.HitSound = hit_sound;
        PostInit();
    }

    public static HarmOfColliderInfo DeserializeHarmOfColliderInfo(JSONNode _json)
    {
        return new timeline.HarmOfColliderInfo(_json);
    }

    public numeric.DamageFormulaInfo HarmFormula { get; private set; }
    /// <summary>
    /// 碰撞器名
    /// </summary>
    public string ColliderName { get; private set; }
    public string HitFx { get; private set; }
    public string HitSound { get; private set; }

    public const int __ID__ = 469690170;
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
        + "Origin:" + Origin + ","
        + "Dest:" + Dest + ","
        + "HarmFormula:" + HarmFormula + ","
        + "ColliderName:" + ColliderName + ","
        + "HitFx:" + HitFx + ","
        + "HitSound:" + HitSound + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
