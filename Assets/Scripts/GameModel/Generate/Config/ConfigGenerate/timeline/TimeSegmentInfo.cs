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

public abstract partial class TimeSegmentInfo :  timeline.TimeActionInfo 
{
    public TimeSegmentInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["model"].IsNumber) { throw new SerializationException(); }  Model = _json["model"]; }
        { if(!_json["origin"].IsNumber) { throw new SerializationException(); }  Origin = _json["origin"]; }
        { if(!_json["dest"].IsNumber) { throw new SerializationException(); }  Dest = _json["dest"]; }
        PostInit();
    }

    public TimeSegmentInfo(int model, float origin, float dest )  : base() 
    {
        this.Model = model;
        this.Origin = origin;
        this.Dest = dest;
        PostInit();
    }

    public static TimeSegmentInfo DeserializeTimeSegmentInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "MoveInfo": return new timeline.MoveInfo(_json);
            case "ModifyTagsInfo": return new timeline.ModifyTagsInfo(_json);
            case "ForceMovementInfo": return new timeline.ForceMovementInfo(_json);
            case "LeapInfo": return new timeline.LeapInfo(_json);
            case "JumpAttackInfo": return new timeline.JumpAttackInfo(_json);
            case "LiuXueInfo": return new timeline.LiuXueInfo(_json);
            case "RanshaoInfo": return new timeline.RanshaoInfo(_json);
            case "HarmOfColliderInfo": return new timeline.HarmOfColliderInfo(_json);
            case "HarmOfSphereColliderInfo": return new timeline.HarmOfSphereColliderInfo(_json);
            case "HarmOfBoxColliderInfo": return new timeline.HarmOfBoxColliderInfo(_json);
            default: throw new SerializationException();
        }
    }

    public int Model { get; private set; }
    public float Origin { get; private set; }
    public float Dest { get; private set; }


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
