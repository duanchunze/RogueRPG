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

public sealed partial class PickableConfig :  Bright.Config.BeanBase 
{
    public PickableConfig(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["alias"].IsString) { throw new SerializationException(); }  Alias = _json["alias"]; }
        { if(!_json["view_name"].IsString) { throw new SerializationException(); }  ViewName = _json["view_name"]; }
        { if(!_json["model_name"].IsString) { throw new SerializationException(); }  ModelName = _json["model_name"]; }
        { if(!_json["collider_name"].IsString) { throw new SerializationException(); }  ColliderName = _json["collider_name"]; }
        { if(!_json["wrappage"].IsObject) { throw new SerializationException(); }  Wrappage = pickable.Info.DeserializeInfo(_json["wrappage"]);  }
        { if(!_json["radius"].IsNumber) { throw new SerializationException(); }  Radius = _json["radius"]; }
        PostInit();
    }

    public PickableConfig(int id, string alias, string view_name, string model_name, string collider_name, pickable.Info wrappage, float radius ) 
    {
        this.Id = id;
        this.Alias = alias;
        this.ViewName = view_name;
        this.ModelName = model_name;
        this.ColliderName = collider_name;
        this.Wrappage = wrappage;
        this.Radius = radius;
        PostInit();
    }

    public static PickableConfig DeserializePickableConfig(JSONNode _json)
    {
        return new pickable.PickableConfig(_json);
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
    /// 名字
    /// </summary>
    public string ViewName { get; private set; }
    public string ModelName { get; private set; }
    public string ColliderName { get; private set; }
    public pickable.Info Wrappage { get; private set; }
    public float Radius { get; private set; }

    public const int __ID__ = -969378416;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        Wrappage?.Resolve(_tables);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        Wrappage?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Alias:" + Alias + ","
        + "ViewName:" + ViewName + ","
        + "ModelName:" + ModelName + ","
        + "ColliderName:" + ColliderName + ","
        + "Wrappage:" + Wrappage + ","
        + "Radius:" + Radius + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
