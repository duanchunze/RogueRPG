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
/// 类似剑魔Q的那种警示
/// </summary>
public sealed partial class OpenWarningBoardInfo :  timeline.TimePointInfo 
{
    public OpenWarningBoardInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["asset_name"].IsString) { throw new SerializationException(); }  AssetName = _json["asset_name"]; }
        { if(!_json["center"].IsObject) { throw new SerializationException(); }  Center = hmath.Vector3.DeserializeVector3(_json["center"]);  }
        { if(!_json["size"].IsObject) { throw new SerializationException(); }  Size = hmath.Vector3.DeserializeVector3(_json["size"]);  }
        PostInit();
    }

    public OpenWarningBoardInfo(int model, float point, string asset_name, hmath.Vector3 center, hmath.Vector3 size )  : base(model,point) 
    {
        this.AssetName = asset_name;
        this.Center = center;
        this.Size = size;
        PostInit();
    }

    public static OpenWarningBoardInfo DeserializeOpenWarningBoardInfo(JSONNode _json)
    {
        return new timeline.OpenWarningBoardInfo(_json);
    }

    /// <summary>
    /// 资源名
    /// </summary>
    public string AssetName { get; private set; }
    public hmath.Vector3 Center { get; private set; }
    public hmath.Vector3 Size { get; private set; }

    public const int __ID__ = -303031665;
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
        + "Point:" + Point + ","
        + "AssetName:" + AssetName + ","
        + "Center:" + Center + ","
        + "Size:" + Size + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
