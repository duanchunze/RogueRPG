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
/// 根据敌人总血量的百分比
/// </summary>
public sealed partial class FormulaInfo2 :  numeric.FormulaInfo 
{
    public FormulaInfo2(JSONNode _json)  : base(_json) 
    {
        { if(!_json["pct"].IsNumber) { throw new SerializationException(); }  Pct = _json["pct"]; }
        PostInit();
    }

    public FormulaInfo2(float pct )  : base() 
    {
        this.Pct = pct;
        PostInit();
    }

    public static FormulaInfo2 DeserializeFormulaInfo2(JSONNode _json)
    {
        return new numeric.FormulaInfo2(_json);
    }

    public float Pct { get; private set; }

    public const int __ID__ = -1775542049;
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
        + "Pct:" + Pct + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
