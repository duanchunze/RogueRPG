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

public abstract partial class Info :  Bright.Config.BeanBase 
{
    public Info(JSONNode _json) 
    {
        PostInit();
    }

    public Info() 
    {
        PostInit();
    }

    public static Info DeserializeInfo(JSONNode _json)
    {
        string type = _json["$type"];
        switch (type)
        {
            case "WrappageCoinInfo": return new pickable.WrappageCoinInfo(_json);
            default: throw new SerializationException();
        }
    }



    public virtual void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public virtual void TranslateText(System.Func<string, string, string> translator)
    {
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
