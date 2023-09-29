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
public sealed partial class HarmOfBoxColliderInfo :  timeline.TsHarmInfo 
{
    public HarmOfBoxColliderInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["hit_fx"].IsString) { throw new SerializationException(); }  HitFx = _json["hit_fx"]; }
        { if(!_json["hit_sound"].IsString) { throw new SerializationException(); }  HitSound = _json["hit_sound"]; }
        PostInit();
    }

    public HarmOfBoxColliderInfo(int model, float origin, float dest, System.Collections.Generic.List<numeric.DamageFormulaInfo> harm_formulas, string hit_fx, string hit_sound )  : base(model,origin,dest,harm_formulas) 
    {
        this.HitFx = hit_fx;
        this.HitSound = hit_sound;
        PostInit();
    }

    public static HarmOfBoxColliderInfo DeserializeHarmOfBoxColliderInfo(JSONNode _json)
    {
        return new timeline.HarmOfBoxColliderInfo(_json);
    }

    public string HitFx { get; private set; }
    public string HitSound { get; private set; }

    public const int __ID__ = -1990865739;
    public override int GetTypeId() => __ID__;

    public override void Resolve(Dictionary<string, object> _tables)
    {
        base.Resolve(_tables);
        PostResolve();
    }

    public override void TranslateText(System.Func<string, string, string> translator)
    {
        base.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Model:" + Model + ","
        + "Origin:" + Origin + ","
        + "Dest:" + Dest + ","
        + "HarmFormulas:" + Bright.Common.StringUtil.CollectionToString(HarmFormulas) + ","
        + "HitFx:" + HitFx + ","
        + "HitSound:" + HitSound + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
