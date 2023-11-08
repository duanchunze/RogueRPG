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



namespace Hsenl.priority
{ 

public sealed partial class StateInfo :  priority.Info 
{
    public StateInfo(JSONNode _json)  : base(_json) 
    {
        { if(!_json["time_scale"].IsNumber) { throw new SerializationException(); }  TimeScale = _json["time_scale"]; }
        { if(!_json["duration"].IsNumber) { throw new SerializationException(); }  Duration = _json["duration"]; }
        { var __json0 = _json["aisles"]; if(!__json0.IsArray) { throw new SerializationException(); } Aisles = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  Aisles.Add(__v0); }   }
        { if(!_json["enter_pri"].IsNumber) { throw new SerializationException(); }  EnterPri = _json["enter_pri"]; }
        { if(!_json["resist_pri"].IsNumber) { throw new SerializationException(); }  ResistPri = _json["resist_pri"]; }
        { if(!_json["exclu_pri"].IsNumber) { throw new SerializationException(); }  ExcluPri = _json["exclu_pri"]; }
        { if(!_json["keep_pri"].IsNumber) { throw new SerializationException(); }  KeepPri = _json["keep_pri"]; }
        { if(!_json["dis_pri"].IsNumber) { throw new SerializationException(); }  DisPri = _json["dis_pri"]; }
        { if(!_json["run_pri"].IsNumber) { throw new SerializationException(); }  RunPri = _json["run_pri"]; }
        { var __json0 = _json["sp_pass"]; if(!__json0.IsArray) { throw new SerializationException(); } SpPass = new System.Collections.Generic.List<TagType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { TagType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (TagType)__e0.AsInt; }  SpPass.Add(__v0); }   }
        { var __json0 = _json["sp_inter"]; if(!__json0.IsArray) { throw new SerializationException(); } SpInter = new System.Collections.Generic.List<TagType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { TagType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (TagType)__e0.AsInt; }  SpInter.Add(__v0); }   }
        { var __json0 = _json["sp_exclu"]; if(!__json0.IsArray) { throw new SerializationException(); } SpExclu = new System.Collections.Generic.List<TagType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { TagType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (TagType)__e0.AsInt; }  SpExclu.Add(__v0); }   }
        { var __json0 = _json["sp_keep"]; if(!__json0.IsArray) { throw new SerializationException(); } SpKeep = new System.Collections.Generic.List<TagType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { TagType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (TagType)__e0.AsInt; }  SpKeep.Add(__v0); }   }
        { var __json0 = _json["sp_dis"]; if(!__json0.IsArray) { throw new SerializationException(); } SpDis = new System.Collections.Generic.List<TagType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { TagType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (TagType)__e0.AsInt; }  SpDis.Add(__v0); }   }
        { var __json0 = _json["sp_run"]; if(!__json0.IsArray) { throw new SerializationException(); } SpRun = new System.Collections.Generic.List<TagType>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { TagType __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = (TagType)__e0.AsInt; }  SpRun.Add(__v0); }   }
        { if(!_json["allow_reenter"].IsBoolean) { throw new SerializationException(); }  AllowReenter = _json["allow_reenter"]; }
        PostInit();
    }

    public StateInfo(float time_scale, float duration, System.Collections.Generic.List<int> aisles, int enter_pri, int resist_pri, int exclu_pri, int keep_pri, int dis_pri, int run_pri, System.Collections.Generic.List<TagType> sp_pass, System.Collections.Generic.List<TagType> sp_inter, System.Collections.Generic.List<TagType> sp_exclu, System.Collections.Generic.List<TagType> sp_keep, System.Collections.Generic.List<TagType> sp_dis, System.Collections.Generic.List<TagType> sp_run, bool allow_reenter )  : base() 
    {
        this.TimeScale = time_scale;
        this.Duration = duration;
        this.Aisles = aisles;
        this.EnterPri = enter_pri;
        this.ResistPri = resist_pri;
        this.ExcluPri = exclu_pri;
        this.KeepPri = keep_pri;
        this.DisPri = dis_pri;
        this.RunPri = run_pri;
        this.SpPass = sp_pass;
        this.SpInter = sp_inter;
        this.SpExclu = sp_exclu;
        this.SpKeep = sp_keep;
        this.SpDis = sp_dis;
        this.SpRun = sp_run;
        this.AllowReenter = allow_reenter;
        PostInit();
    }

    public static StateInfo DeserializeStateInfo(JSONNode _json)
    {
        return new priority.StateInfo(_json);
    }

    public float TimeScale { get; private set; }
    public float Duration { get; private set; }
    public System.Collections.Generic.List<int> Aisles { get; private set; }
    public int EnterPri { get; private set; }
    public int ResistPri { get; private set; }
    public int ExcluPri { get; private set; }
    public int KeepPri { get; private set; }
    public int DisPri { get; private set; }
    public int RunPri { get; private set; }
    public System.Collections.Generic.List<TagType> SpPass { get; private set; }
    public System.Collections.Generic.List<TagType> SpInter { get; private set; }
    public System.Collections.Generic.List<TagType> SpExclu { get; private set; }
    public System.Collections.Generic.List<TagType> SpKeep { get; private set; }
    public System.Collections.Generic.List<TagType> SpDis { get; private set; }
    public System.Collections.Generic.List<TagType> SpRun { get; private set; }
    public bool AllowReenter { get; private set; }

    public const int __ID__ = -1949098763;
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
        + "TimeScale:" + TimeScale + ","
        + "Duration:" + Duration + ","
        + "Aisles:" + Bright.Common.StringUtil.CollectionToString(Aisles) + ","
        + "EnterPri:" + EnterPri + ","
        + "ResistPri:" + ResistPri + ","
        + "ExcluPri:" + ExcluPri + ","
        + "KeepPri:" + KeepPri + ","
        + "DisPri:" + DisPri + ","
        + "RunPri:" + RunPri + ","
        + "SpPass:" + Bright.Common.StringUtil.CollectionToString(SpPass) + ","
        + "SpInter:" + Bright.Common.StringUtil.CollectionToString(SpInter) + ","
        + "SpExclu:" + Bright.Common.StringUtil.CollectionToString(SpExclu) + ","
        + "SpKeep:" + Bright.Common.StringUtil.CollectionToString(SpKeep) + ","
        + "SpDis:" + Bright.Common.StringUtil.CollectionToString(SpDis) + ","
        + "SpRun:" + Bright.Common.StringUtil.CollectionToString(SpRun) + ","
        + "AllowReenter:" + AllowReenter + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
