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

public abstract partial class TsStatusHarmInfo :  timeline.TsHarmInfo 
{
    public TsStatusHarmInfo(JSONNode _json)  : base(_json) 
    {
        PostInit();
    }

    public TsStatusHarmInfo(int model, float origin, float dest )  : base(model,origin,dest) 
    {
        PostInit();
    }

    public static TsStatusHarmInfo DeserializeTsStatusHarmInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "TsStatusContinuousHarmInfo": return new timeline.TsStatusContinuousHarmInfo(_json);
            case "LiuXueInfo": return new timeline.LiuXueInfo(_json);
            case "RanshaoInfo": return new timeline.RanshaoInfo(_json);
            case "WenyiInfo": return new timeline.WenyiInfo(_json);
            case "WenyichuanboInfo": return new timeline.WenyichuanboInfo(_json);
            default: throw new SerializationException();
        }
    }



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
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
