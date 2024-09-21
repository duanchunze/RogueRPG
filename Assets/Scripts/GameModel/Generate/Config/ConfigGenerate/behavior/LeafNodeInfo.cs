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



namespace Hsenl.behavior
{ 

/// <summary>
/// 支叶节点
/// </summary>
public abstract partial class LeafNodeInfo :  behavior.Info 
{
    public LeafNodeInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["parent_index"].IsNumber) { throw new SerializationException(); }  ParentIndex = _json["parent_index"]; }
        PostInit();
    }

    public LeafNodeInfo(int parent_index )  : base() 
    {
        this.ParentIndex = parent_index;
        PostInit();
    }

    public static LeafNodeInfo DeserializeLeafNodeInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "AbortNodeInfo": return new behavior.AbortNodeInfo(_json);
            case "ai.PlayerAutoCasterInfo": return new ai.PlayerAutoCasterInfo(_json);
            case "ai.PatrolInfo": return new ai.PatrolInfo(_json);
            case "ai.IntelligentAssaultInfo": return new ai.IntelligentAssaultInfo(_json);
            case "ai.MinionDefaultIntelligentAssaultInfo": return new ai.MinionDefaultIntelligentAssaultInfo(_json);
            case "ai.FollowMaster": return new ai.FollowMaster(_json);
            case "casterevaluate.CooldownCheckInfo": return new casterevaluate.CooldownCheckInfo(_json);
            case "casterevaluate.ManaCheckInfo": return new casterevaluate.ManaCheckInfo(_json);
            case "casterevaluate.PrioritiesEvaluateInfo": return new casterevaluate.PrioritiesEvaluateInfo(_json);
            case "casterevaluate.PickTargetInfo": return new casterevaluate.PickTargetInfo(_json);
            case "casterevaluate.PickAndApproachTargetInfo": return new casterevaluate.PickAndApproachTargetInfo(_json);
            case "casterevaluate.HealthLowerInfo": return new casterevaluate.HealthLowerInfo(_json);
            case "casterevaluate.CasterSummoningInfo": return new casterevaluate.CasterSummoningInfo(_json);
            case "adventurescheme.DefaultCheckpointsAdventureInfo": return new adventurescheme.DefaultCheckpointsAdventureInfo(_json);
            case "adventurescheme.DefaultBigMapAdventureInfo": return new adventurescheme.DefaultBigMapAdventureInfo(_json);
            default: throw new SerializationException();
        }
    }

    public int ParentIndex { get; private set; }


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
