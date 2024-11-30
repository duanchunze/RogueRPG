using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    public enum UILayer {
        Hidden,
        Low,
        Mid,
        High,
    }

    [DisallowMultipleComponent]
    public class UIManager : UnitySingleton<UIManager> {
        public Camera uiCamera;
        private readonly Dictionary<string, IUI> _singles = new();
        private readonly Dictionary<string, Queue<IUI>> _multi = new();

        [ShowInInspector, ReadOnly]
        private Dictionary<UILayer, UnityEngine.Transform> _layers = new();

        protected override void Awake() {
            base.Awake();
            foreach (var layerName in Enum.GetNames(typeof(UILayer))) {
                var layer = this.transform.Find(layerName);
                if (layer == null) {
                    Log.Error($"cant find ui layer by name '{layerName}'");
                    continue;
                }

                this._layers.Add(Enum.Parse<UILayer>(layerName), layer);
            }
        }

        public static T SingleOpen<T>(UILayer layer) where T : IUI {
            return (T)SingleOpen(typeof(T).Name, layer);
        }

        public static IUI SingleOpen(string uiName, UILayer layer) {
            if (!Instance._singles.TryGetValue(uiName, out var ui)) {
                var uiAsset = GetUIAssetSync(uiName);
                var uiObj = (GameObject)UnityEngine.Object.Instantiate(uiAsset);
                ui = uiObj.GetComponent<IUI>();
                Assert.NullReference(ui, "cant get component");
                Instance._singles[uiName] = ui;
            }

            ui.InternalOpen(UIOpenType.Single, Instance._layers[layer]);
            return ui;
        }

        public static async HTask<T> SingleOpenAsync<T>(UILayer layer) where T : IUI {
            return (T)await SingleOpenAsync(typeof(T).Name, layer);
        }

        public static async HTask<IUI> SingleOpenAsync(string uiName, UILayer layer) {
            if (!Instance._singles.TryGetValue(uiName, out var ui)) {
                var uiAsset = await GetUIAssetAsync(uiName);
                var uiObj = (GameObject)UnityEngine.Object.Instantiate(uiAsset);
                ui = uiObj.GetComponent<IUI>();
                Assert.NullReference(ui, "cant get component");
                Instance._singles[uiName] = ui;
            }

            ui.InternalOpen(UIOpenType.Single, Instance._layers[layer]);
            return ui;
        }

        public static T SwitchSingleUI<T>(UILayer layer) where T : IUI {
            var uiName = typeof(T).Name;
            if (!Instance._singles.TryGetValue(uiName, out var ui)) {
                var uiAsset = GetUIAssetSync(uiName);
                var uiObj = (GameObject)UnityEngine.Object.Instantiate(uiAsset);
                ui = uiObj.GetComponent<IUI>();
                Assert.NullReference(ui, "cant get component");
                Instance._singles[uiName] = ui;
                ui.InternalOpen(UIOpenType.Single, Instance._layers[layer]);
                return (T)ui;
            }

            if (ui.IsOpen)
                ui.InternalClose();
            else
                ui.InternalOpen(UIOpenType.Single, Instance._layers[layer]);
            return (T)ui;
        }

        public static T MultiOpen<T>(UILayer layer) where T : IUI {
            return (T)MultiOpen(typeof(T).Name, layer);
        }

        public static IUI MultiOpen(string uiName, UILayer layer) {
            IUI ui = null;
            if (Instance._multi.TryGetValue(uiName, out var queue)) {
                if (queue.Count != 0)
                    ui = queue.Dequeue();
            }

            if (ui == null) {
                var uiAsset = GetUIAssetSync(uiName);
                var uiObj = (GameObject)UnityEngine.Object.Instantiate(uiAsset);
                ui = uiObj.GetComponent<IUI>();
                Assert.NullReference(ui, "cant get component");
            }

            ui.InternalOpen(UIOpenType.Multi, Instance._layers[layer]);
            return ui;
        }

        public static async HTask<T> MultiOpenAsync<T>(UILayer layer) where T : IUI {
            return (T)await MultiOpenAsync(typeof(T).Name, layer);
        }

        public static async HTask<IUI> MultiOpenAsync(string uiName, UILayer layer) {
            IUI ui = null;
            if (Instance._multi.TryGetValue(uiName, out var queue)) {
                if (queue.Count != 0)
                    ui = queue.Dequeue();
            }

            if (ui == null) {
                var uiAsset = await GetUIAssetAsync(uiName);
                var uiObj = (GameObject)UnityEngine.Object.Instantiate(uiAsset);
                ui = uiObj.GetComponent<IUI>();
                Assert.NullReference(ui, "cant get component");
            }

            ui.InternalOpen(UIOpenType.Multi, Instance._layers[layer]);
            return ui;
        }

        public static void MultiClose(IUI ui) {
            ui.InternalClose();
            if (!Instance._multi.TryGetValue(ui.Name, out var queue)) {
                queue = new Queue<IUI>();
                Instance._multi[ui.Name] = queue;
            }

            queue.Enqueue(ui);
        }

        public static T SingleClose<T>() where T : IUI {
            var ui = SingleClose(typeof(T).Name);
            return (T)ui;
        }

        public static IUI SingleClose(string uiName) {
            if (!Instance._singles.TryGetValue(uiName, out var ui)) {
                // Log.Error($"single close ui fail, ui have not been single opened '{uiName}'");
                return null;
            }

            ui.InternalClose();
            return ui;
        }

        public static T GetSingleUI<T>() where T : IUI {
            var ui = GetSingleUI(typeof(T).Name);
            return (T)ui;
        }
        
        public static IUI GetSingleUI(string uiName) {
            if (!Instance._singles.TryGetValue(uiName, out var ui)) {
                return null;
            }

            return ui;
        }

        private static UnityEngine.Object GetUIAssetSync(string uiName) {
            return YooAssets.LoadAssetSync(uiName)?.AssetObject;
        }

        private static async HTask<UnityEngine.Object> GetUIAssetAsync(string uiName) {
            var handle = await YooAssets.LoadAssetAsync(uiName);
            return handle.AssetObject;
        }

        public static bool WorldToUIPosition(RectTransform rect, UnityEngine.Vector3 worldPos, out UnityEngine.Vector3 uiWorldPos, Camera uiCamera = null, Camera worldCamera = null) {
            if (uiCamera == null)
                uiCamera = UIManager.Instance.uiCamera;

            return UnityHelper.UI.WorldToUIPosition(rect, worldPos, out uiWorldPos, uiCamera, worldCamera);
        }

        public static bool WorldToUILocalPosition(RectTransform rect, UnityEngine.Vector3 worldPos, out UnityEngine.Vector2 uiLocalPos, Camera uiCamera = null,
            Camera worldCamera = null) {
            if (uiCamera == null)
                uiCamera = UIManager.Instance.uiCamera;

            return UnityHelper.UI.WorldToUILocalPosition(rect, worldPos, out uiLocalPos, uiCamera, worldCamera);
        }
    }
}