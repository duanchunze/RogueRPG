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

public sealed partial class PickAndApproachTargetInfo :  casterevaluate.CasterEvaluateInfo 
{
    public PickAndApproachTargetInfo(JSONNode _json)  : base(_json) 
    {
        PostInit();
    }

    public PickAndApproachTargetInfo(int parent_index )  : base(parent_index) 
    {
        PostInit();
    }

    public static PickAndApproachTargetInfo DeserializePickAndApproachTargetInfo(JSONNode _json)
    {
        return new casterevaluate.PickAndApproachTargetInfo(_json);
    }


    public const int __ID__ = -1647366092;
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
