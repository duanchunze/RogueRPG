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



namespace Hsenl.pickable
{ 

/// <summary>
/// 掉落物所包裹的东西的信息
/// </summary>
public abstract partial class WrappageInfo :  pickable.Info 
{
    public WrappageInfo(JSONNode _json)  : base(_json) 
    {
        PostInit();
    }

    public WrappageInfo()  : base() 
    {
        PostInit();
    }

    public static WrappageInfo DeserializeWrappageInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "WrappageCoinInfo": return new pickable.WrappageCoinInfo(_json);
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
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
