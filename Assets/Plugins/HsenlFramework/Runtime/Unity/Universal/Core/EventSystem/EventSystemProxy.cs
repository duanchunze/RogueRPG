using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    public class EventSystemProxy : MonoBehaviour {
        [SerializeField, HideLabel]
        private EventSystemManager _event;

        public List<string> assembliesFilter = new() {
            "HsenlFramework.Editor"
        };

        [ReadOnly]
        public List<string> allLoadAssemblies = new();

        private const string basicAssemblyName = "HsenlFramework.Runtime";
        private Assembly _reloadAssembly;

        private void Awake() {
            if (!SingletonManager.IsDisposed<EventSystemManager>()) {
                SingletonManager.Unregister<EventSystemManager>();
            }

            SingletonManager.Register(ref this._event);
            this._event.RegisterAttributeType(typeof(MemoryPackableAttribute));

            var basicAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == basicAssemblyName);
            if (basicAssembly == null)
                throw new Exception("Can't find HsenlFramework.Runtime assembly!");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => {
                    return x.FullName == basicAssembly.FullName || x.GetReferencedAssemblies()
                        .Any(xx => xx.FullName == basicAssembly.FullName);
                })
                .Where(x => !this.assembliesFilter.Contains(x.GetName().Name))
                .ToArray();
            this._event.SetAssembles(assemblies);

            foreach (var assembly in assemblies) {
                if (assembly.GetName().Name == "GameHotReload")
                    this._reloadAssembly = assembly;
            }

            foreach (var assembly in assemblies) {
                this.allLoadAssemblies.Add(assembly.GetName().Name);
            }
        }

        private void Update() {
            // 热重载测试
            // if (InputController.GetButtonDown(InputCode.R)) {
            //     var dllHandle = YooAssets.LoadAssetSync<TextAsset>("GameHotReload.dll");
            //     var pdbHandle = YooAssets.LoadAssetSync<TextAsset>("GameHotReload.pdb");
            //     if (dllHandle.AssetObject == null) throw new Exception("Asset 'GameHotReload.dll' can not find from main package");
            //     byte[] dllBytes = ((TextAsset)dllHandle.AssetObject).bytes;
            //     byte[] pdbBytes = ((TextAsset)pdbHandle.AssetObject).bytes;
            //     this._reloadAssembly = Assembly.Load(dllBytes, pdbBytes);
            //     this._event.AddOrReplaceAssembles(new[] { this._reloadAssembly });
            //     // unity不支持程序集的卸载, 所以, 每次热重载, 该程序集都会增加一个, 可能导致的问题就是在获取type的时候, 如果没注意是从旧的程序集里获取的type,
            //     // 就可能导致意料之外的问题. 作为测试没问题, 但实际程序中, 不要使用.
            //     // 所以, 对于unity的包, 不要使用热重载功能, 只在写服务端的时候, 再使用热重载功能.
            //     // 如果热重载后, 还必须要重新编译, 直接进入的话, 会有多个GameHotReload程序集, 导致未知错误
            //     Log.Info("热重载成功!");
            // }
        }
    }
}