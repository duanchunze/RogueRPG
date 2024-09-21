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



namespace Hsenl.ability
{ 

public sealed partial class AbilityTraitConfig :  Bright.Config.BeanBase 
{
    public AbilityTraitConfig(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["alias"].IsString) { throw new SerializationException(); }  Alias = _json["alias"]; }
        { var __json0 = _json["numeric_nodes"]; if(!__json0.IsArray) { throw new SerializationException(); } NumericNodes = new System.Collections.Generic.List<numeric.AttachValueInfo>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { numeric.AttachValueInfo __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = numeric.AttachValueInfo.DeserializeAttachValueInfo(__e0);  }  NumericNodes.Add(__v0); }   }
        { var __json0 = _json["workers"]; if(!__json0.IsArray) { throw new SerializationException(); } Workers = new System.Collections.Generic.List<procedureline.WorkerInfo>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { procedureline.WorkerInfo __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = procedureline.WorkerInfo.DeserializeWorkerInfo(__e0);  }  Workers.Add(__v0); }   }
        PostInit();
    }

    public AbilityTraitConfig(int id, string alias, System.Collections.Generic.List<numeric.AttachValueInfo> numeric_nodes, System.Collections.Generic.List<procedureline.WorkerInfo> workers ) 
    {
        this.Id = id;
        this.Alias = alias;
        this.NumericNodes = numeric_nodes;
        this.Workers = workers;
        PostInit();
    }

    public static AbilityTraitConfig DeserializeAbilityTraitConfig(JSONNode _json)
    {
        return new ability.AbilityTraitConfig(_json);
    }

    /// <summary>
    /// 这是ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 别名
    /// </summary>
    public string Alias { get; private set; }
    /// <summary>
    /// 附加的数值
    /// </summary>
    public System.Collections.Generic.List<numeric.AttachValueInfo> NumericNodes { get; private set; }
    public System.Collections.Generic.List<procedureline.WorkerInfo> Workers { get; private set; }

    public const int __ID__ = -1183926582;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var _e in NumericNodes) { _e?.Resolve(_tables); }
        foreach(var _e in Workers) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var _e in NumericNodes) { _e?.TranslateText(translator); }
        foreach(var _e in Workers) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Alias:" + Alias + ","
        + "NumericNodes:" + Bright.Common.StringUtil.CollectionToString(NumericNodes) + ","
        + "Workers:" + Bright.Common.StringUtil.CollectionToString(Workers) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
