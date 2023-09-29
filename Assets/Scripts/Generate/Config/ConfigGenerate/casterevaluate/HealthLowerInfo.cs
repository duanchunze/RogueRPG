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



namespace Hsenl.casterevaluate
{ 

public sealed partial class HealthLowerInfo :  casterevaluate.CasterEvaluateInfo 
{
    public HealthLowerInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["threshold"].IsNumber) { throw new SerializationException(); }  Threshold = _json["threshold"]; }
        PostInit();
    }

    public HealthLowerInfo(int parent_index, float threshold )  : base(parent_index) 
    {
        this.Threshold = threshold;
        PostInit();
    }

    public static HealthLowerInfo DeserializeHealthLowerInfo(JSONNode _json)
    {
        return new casterevaluate.HealthLowerInfo(_json);
    }

    /// <summary>
    /// 低于该阈值触发
    /// </summary>
    public float Threshold { get; private set; }

    public const int __ID__ = -663322358;
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
        + "ParentIndex:" + ParentIndex + ","
        + "Threshold:" + Threshold + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
