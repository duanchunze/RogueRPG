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
/// 面包节点
/// </summary>
public abstract partial class BreadNodeInfo :  behavior.Info 
{
    public BreadNodeInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["index"].IsNumber) { throw new SerializationException(); }  Index = _json["index"]; }
        { if(!_json["parent_index"].IsNumber) { throw new SerializationException(); }  ParentIndex = _json["parent_index"]; }
        PostInit();
    }

    public BreadNodeInfo(int index, int parent_index )  : base() 
    {
        this.Index = index;
        this.ParentIndex = parent_index;
        PostInit();
    }

    public static BreadNodeInfo DeserializeBreadNodeInfo(JSONNode _json)
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

    public int Index { get; private set; }
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
        + "Index:" + Index + ","
        + "ParentIndex:" + ParentIndex + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
