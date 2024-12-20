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



namespace Hsenl.procedureline
{ 

/// <summary>
/// 造成伤害时, 有概率触发施法
/// </summary>
public sealed partial class HarmProbabilisticCastInfo :  procedureline.CastWorkerInfo 
{
    public HarmProbabilisticCastInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["probability"].IsNumber) { throw new SerializationException(); }  Probability = _json["probability"]; }
        PostInit();
    }

    public HarmProbabilisticCastInfo(float probability )  : base() 
    {
        this.Probability = probability;
        PostInit();
    }

    public static HarmProbabilisticCastInfo DeserializeHarmProbabilisticCastInfo(JSONNode _json)
    {
        return new procedureline.HarmProbabilisticCastInfo(_json);
    }

    /// <summary>
    /// 触发概率, 0-1之间
    /// </summary>
    public float Probability { get; private set; }

    public const int __ID__ = 536575997;
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
        + "Probability:" + Probability + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
