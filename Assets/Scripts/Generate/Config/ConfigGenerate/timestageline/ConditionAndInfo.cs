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



namespace Hsenl.timestageline
{ 

/// <summary>
/// 阶段线条件, 采用与逻辑
/// </summary>
public sealed partial class ConditionAndInfo :  timestageline.TimeStageLineInfo 
{
    public ConditionAndInfo(JSONNode _json)  : base(_json) 
    {
        PostInit();
    }

    public ConditionAndInfo(int index, int parent_index )  : base(index,parent_index) 
    {
        PostInit();
    }

    public static ConditionAndInfo DeserializeConditionAndInfo(JSONNode _json)
    {
        return new timestageline.ConditionAndInfo(_json);
    }


    public const int __ID__ = 1822857395;
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
        + "Index:" + Index + ","
        + "ParentIndex:" + ParentIndex + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
