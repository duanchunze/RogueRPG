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

public sealed partial class CloseWarningBoardInfo :  timeline.TimePointInfo 
{
    public CloseWarningBoardInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["warn_name"].IsString) { throw new SerializationException(); }  WarnName = _json["warn_name"]; }
        PostInit();
    }

    public CloseWarningBoardInfo(int model, float point, string warn_name )  : base(model,point) 
    {
        this.WarnName = warn_name;
        PostInit();
    }

    public static CloseWarningBoardInfo DeserializeCloseWarningBoardInfo(JSONNode _json)
    {
        return new timeline.CloseWarningBoardInfo(_json);
    }

    /// <summary>
    /// 警示标名称
    /// </summary>
    public string WarnName { get; private set; }

    public const int __ID__ = -1163793277;
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
        + "WarnName:" + WarnName + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
