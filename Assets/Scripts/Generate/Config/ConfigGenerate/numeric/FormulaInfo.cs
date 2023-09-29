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



namespace Hsenl.numeric
{ 

/// <summary>
/// 数值配方
/// </summary>
public sealed partial class FormulaInfo :  numeric.Info 
{
    public FormulaInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["type"].IsNumber) { throw new SerializationException(); }  Type = (NumericType)_json["type"].AsInt; }
        { if(!_json["pct"].IsNumber) { throw new SerializationException(); }  Pct = _json["pct"]; }
        { if(!_json["fix"].IsNumber) { throw new SerializationException(); }  Fix = _json["fix"]; }
        PostInit();
    }

    public FormulaInfo(NumericType type, float pct, float fix )  : base() 
    {
        this.Type = type;
        this.Pct = pct;
        this.Fix = fix;
        PostInit();
    }

    public static FormulaInfo DeserializeFormulaInfo(JSONNode _json)
    {
        return new numeric.FormulaInfo(_json);
    }

    public NumericType Type { get; private set; }
    public float Pct { get; private set; }
    public float Fix { get; private set; }

    public const int __ID__ = -1581296205;
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
        + "Type:" + Type + ","
        + "Pct:" + Pct + ","
        + "Fix:" + Fix + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
