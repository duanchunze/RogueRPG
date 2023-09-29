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
/// ez的q
/// </summary>
public sealed partial class HarmOfDirectionBoltInfo :  timeline.TpHarmInfo 
{
    public HarmOfDirectionBoltInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["model_name"].IsString) { throw new SerializationException(); }  ModelName = _json["model_name"]; }
        { if(!_json["distance"].IsNumber) { throw new SerializationException(); }  Distance = _json["distance"]; }
        { if(!_json["speed"].IsNumber) { throw new SerializationException(); }  Speed = _json["speed"]; }
        { if(!_json["hit_fx"].IsString) { throw new SerializationException(); }  HitFx = _json["hit_fx"]; }
        { if(!_json["hit_sound"].IsString) { throw new SerializationException(); }  HitSound = _json["hit_sound"]; }
        PostInit();
    }

    public HarmOfDirectionBoltInfo(int model, float point, System.Collections.Generic.List<numeric.DamageFormulaInfo> harm_formulas, string model_name, float distance, float speed, string hit_fx, string hit_sound )  : base(model,point,harm_formulas) 
    {
        this.ModelName = model_name;
        this.Distance = distance;
        this.Speed = speed;
        this.HitFx = hit_fx;
        this.HitSound = hit_sound;
        PostInit();
    }

    public static HarmOfDirectionBoltInfo DeserializeHarmOfDirectionBoltInfo(JSONNode _json)
    {
        return new timeline.HarmOfDirectionBoltInfo(_json);
    }

    /// <summary>
    /// 投射物名
    /// </summary>
    public string ModelName { get; private set; }
    /// <summary>
    /// 最大距离
    /// </summary>
    public float Distance { get; private set; }
    /// <summary>
    /// 弹道速度
    /// </summary>
    public float Speed { get; private set; }
    public string HitFx { get; private set; }
    public string HitSound { get; private set; }

    public const int __ID__ = 99825674;
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
        + "Point:" + Point + ","
        + "HarmFormulas:" + Bright.Common.StringUtil.CollectionToString(HarmFormulas) + ","
        + "ModelName:" + ModelName + ","
        + "Distance:" + Distance + ","
        + "Speed:" + Speed + ","
        + "HitFx:" + HitFx + ","
        + "HitSound:" + HitSound + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
