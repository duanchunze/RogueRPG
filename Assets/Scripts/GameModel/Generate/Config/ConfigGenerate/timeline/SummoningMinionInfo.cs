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

public sealed partial class SummoningMinionInfo :  timeline.TimePointInfo 
{
    public SummoningMinionInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["minion_alias"].IsString) { throw new SerializationException(); }  MinionAlias = _json["minion_alias"]; }
        PostInit();
    }

    public SummoningMinionInfo(int model, float point, string minion_alias )  : base(model,point) 
    {
        this.MinionAlias = minion_alias;
        PostInit();
    }

    public static SummoningMinionInfo DeserializeSummoningMinionInfo(JSONNode _json)
    {
        return new timeline.SummoningMinionInfo(_json);
    }

    /// <summary>
    /// 随从名
    /// </summary>
    public string MinionAlias { get; private set; }

    public const int __ID__ = 239342392;
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
        + "MinionAlias:" + MinionAlias + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
