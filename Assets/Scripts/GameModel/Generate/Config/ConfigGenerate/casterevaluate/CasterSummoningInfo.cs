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

/// <summary>
/// 评估是否超过最大召唤数之类的
/// </summary>
public sealed partial class CasterSummoningInfo :  casterevaluate.CasterEvaluateInfo 
{
    public CasterSummoningInfo(JSONNode _json)  : base(_json) 
    {
        PostInit();
    }

    public CasterSummoningInfo(int parent_index )  : base(parent_index) 
    {
        PostInit();
    }

    public static CasterSummoningInfo DeserializeCasterSummoningInfo(JSONNode _json)
    {
        return new casterevaluate.CasterSummoningInfo(_json);
    }


    public const int __ID__ = -918409190;
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
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
