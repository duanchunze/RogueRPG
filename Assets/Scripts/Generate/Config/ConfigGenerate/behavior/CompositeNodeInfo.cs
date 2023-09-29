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
/// 综合节点
/// </summary>
public abstract partial class CompositeNodeInfo :  behavior.BreadNodeInfo 
{
    public CompositeNodeInfo(JSONNode _json)  : base(_json) 
    {
        PostInit();
    }

    public CompositeNodeInfo(int index, int parent_index )  : base(index,parent_index) 
    {
        PostInit();
    }

    public static CompositeNodeInfo DeserializeCompositeNodeInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "SelectorNodeInfo": return new behavior.SelectorNodeInfo(_json);
            case "SequentialNodeInfo": return new behavior.SequentialNodeInfo(_json);
            case "ParalleNodeInfo": return new behavior.ParalleNodeInfo(_json);
            case "ParalleSelectorNodeInfo": return new behavior.ParalleSelectorNodeInfo(_json);
            case "ParalleSequentialNodeInfo": return new behavior.ParalleSequentialNodeInfo(_json);
            case "AINodeInfo": return new behavior.AINodeInfo(_json);
            default: throw new SerializationException();
        }
    }



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
