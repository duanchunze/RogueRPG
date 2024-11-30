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

public sealed partial class DeadBodyVampirOfColliderInfo :  timeline.TimeSegmentInfo 
{
    public DeadBodyVampirOfColliderInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["collider_name"].IsString) { throw new SerializationException(); }  ColliderName = _json["collider_name"]; }
        { if(!_json["center"].IsObject) { throw new SerializationException(); }  Center = hmath.Vector3.DeserializeVector3(_json["center"]);  }
        { if(!_json["size"].IsObject) { throw new SerializationException(); }  Size = hmath.Vector3.DeserializeVector3(_json["size"]);  }
        { if(!_json["pct"].IsNumber) { throw new SerializationException(); }  Pct = _json["pct"]; }
        PostInit();
    }

    public DeadBodyVampirOfColliderInfo(int model, float origin, float dest, string collider_name, hmath.Vector3 center, hmath.Vector3 size, float pct )  : base(model,origin,dest) 
    {
        this.ColliderName = collider_name;
        this.Center = center;
        this.Size = size;
        this.Pct = pct;
        PostInit();
    }

    public static DeadBodyVampirOfColliderInfo DeserializeDeadBodyVampirOfColliderInfo(JSONNode _json)
    {
        return new timeline.DeadBodyVampirOfColliderInfo(_json);
    }

    /// <summary>
    /// 半径
    /// </summary>
    public string ColliderName { get; private set; }
    /// <summary>
    /// 吸取尸体总血量的百分之多少
    /// </summary>
    public hmath.Vector3 Center { get; private set; }
    public hmath.Vector3 Size { get; private set; }
    public float Pct { get; private set; }

    public const int __ID__ = -782373565;
    public override int GetTypeId() => __ID__;

    public override void Resolve(Dictionary<string, object> _tables)
    {
        base.Resolve(_tables);
        Center?.Resolve(_tables);
        Size?.Resolve(_tables);
        PostResolve();
    }

    public override void TranslateText(System.Func<string, string, string> translator)
    {
        base.TranslateText(translator);
        Center?.TranslateText(translator);
        Size?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Model:" + Model + ","
        + "Origin:" + Origin + ","
        + "Dest:" + Dest + ","
        + "ColliderName:" + ColliderName + ","
        + "Center:" + Center + ","
        + "Size:" + Size + ","
        + "Pct:" + Pct + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
