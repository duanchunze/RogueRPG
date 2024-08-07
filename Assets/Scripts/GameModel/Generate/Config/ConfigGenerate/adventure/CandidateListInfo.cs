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



namespace Hsenl.adventure
{ 

public sealed partial class CandidateListInfo :  adventure.Info 
{
    public CandidateListInfo(JSONNode _json)  : base(_json) 
    {
        { var __json0 = _json["candidates"]; if(!__json0.IsArray) { throw new SerializationException(); } Candidates = new System.Collections.Generic.List<adventure.CandidateInfo>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { adventure.CandidateInfo __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = adventure.CandidateInfo.DeserializeCandidateInfo(__e0);  }  Candidates.Add(__v0); }   }
        PostInit();
    }

    public CandidateListInfo(System.Collections.Generic.List<adventure.CandidateInfo> candidates )  : base() 
    {
        this.Candidates = candidates;
        PostInit();
    }

    public static CandidateListInfo DeserializeCandidateListInfo(JSONNode _json)
    {
        return new adventure.CandidateListInfo(_json);
    }

    public System.Collections.Generic.List<adventure.CandidateInfo> Candidates { get; private set; }

    public const int __ID__ = 797836689;
    public override int GetTypeId() => __ID__;

    public override void Resolve(Dictionary<string, object> _tables)
    {
        base.Resolve(_tables);
        foreach(var _e in Candidates) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public override void TranslateText(System.Func<string, string, string> translator)
    {
        base.TranslateText(translator);
        foreach(var _e in Candidates) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "Candidates:" + Bright.Common.StringUtil.CollectionToString(Candidates) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
