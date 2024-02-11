using System;
using System.Collections.Generic;
using Hsenl.EventType;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class GameManager : SingletonComponent<GameManager>, IUpdate {
        public ProcedureLine ProcedureLine { get; private set; }

        [ShowInInspector, ReadOnly]
        private readonly List<Control> _controlTargets = new();

        [ShowInInspector, ReadOnly]
        private Actor _mainMan;

        [ShowInInspector, ReadOnly]
        private Control _mainControl;

        [ShowInInspector, ReadOnly]
        private UnityEngine.Transform _cameraFocus;

        public Actor MainMan => this._mainMan;
        public Control MainControl => this._mainControl;
        public UnityEngine.Transform CameraFocus => this._cameraFocus;
        public IReadOnlyList<Control> ControlTargets => this._controlTargets;

        public int gold;

        protected override void OnAwake() {
            this.Init();
        }

        public void Update() {
            if (InputController.GetButtonDown(InputCode.P)) {
                var monsterRefresh = UnityEngine.Object.FindObjectOfType<MonsterRefreshManager>();
                foreach (var actor in monsterRefresh.aliveHolder.GetComponent<EntityReference>().Entity.GetComponentsInChildren<Actor>()) {
                    var hurtable = actor.GetComponent<Hurtable>();
                    var damageForm = new PliDamageArbitramentForm {
                        harm = this.GetComponent<Harmable>(),
                        hurt = hurtable,
                        source = this,
                        damageType = DamageType.PhysicalDamage,
                        damage = 30,
                        astun = 50,
                        dex = 1,
                    };

                    this.ProcedureLine.StartLine(ref damageForm);
                }
            }
        }

        public void Init() {
            this.ProcedureLine = this.GetComponent<ProcedureLine>();
        }

        public void SetCameraFocus(UnityEngine.Transform tra) {
            this._cameraFocus = tra;
            if (Camera.main == null) return;
            Camera.main.GetComponent<FollowTarget>().targetTransform = tra;
        }

        public void SetMainMan(Actor mainMan) {
            this._mainMan = mainMan;
        }

        public void SetMainControl(Control unityControl) {
            this._mainControl = unityControl;
        }

        public void AddControlTarget(Control unityControl) {
            if (unityControl == null) return;
            this._controlTargets.Add(unityControl);
        }

        public void RemoveControlTarget(Control unityControl) {
            if (unityControl == null) return;
            this._controlTargets.Remove(unityControl);
        }

        public void SetGold(int num) {
            this.gold = num;
            EventSystem.Publish(new OnGoldNumberUpdate() { goldNum = this.gold });
        }

        public void AddGold(int num) {
            this.gold += num;
            EventSystem.Publish(new OnGoldNumberUpdate() { goldNum = this.gold });
        }

        public void RemoveGold(int num) {
            this.gold -= num;
            EventSystem.Publish(new OnGoldNumberUpdate() { goldNum = this.gold });
        }
    }
}