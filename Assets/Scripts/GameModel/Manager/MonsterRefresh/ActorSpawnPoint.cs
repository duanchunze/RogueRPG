using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hsenl {
    public class ActorSpawnPoint : MonoBehaviour {
        public bool createOfAnother;
        public int configId;
        public bool mainMan;
        public bool controlable;

        public bool autoSpawn;
        public List<string> oriAbilites;

        private bool startSpawn;

        private void OnEnable() {
            if (TimerManager.Instance == null) return;
            if (!this.startSpawn) return;
            this.AutoSpawn();
        }

        private void Start() {
            if (TimerManager.Instance == null) return;
            this.startSpawn = true;
            this.AutoSpawn();
        }

        private void AutoSpawn() {
            if (this.autoSpawn) {
                var actor = this.Spawn();
                var abiBar = actor.FindBodiedInIndividual<AbilitesBar>();
                foreach (var abilityAlias in this.oriAbilites) {
                    var abi = AbilityFactory.Create(abilityAlias);
                    abiBar.EquipAbility(abi);
                }
            }
        }

        public Actor Spawn(Entity entity = null) {
            var config = Tables.Instance.TbActorConfig.GetById(this.configId);
            this.gameObject.name = config.Alias;
            if (entity == null) {
                if (this.createOfAnother) {
                    entity = Entity.Create(this.name);
                    this.gameObject.SetActive(false);
                }
                else {
                    entity = Entity.Create(this.gameObject);
                }
            }

            var actor = ActorManager.Instance.Rent(this.configId, this.transform.position, entity);
            entity = actor.Entity;
            entity.SetParent(null);
            if (this.mainMan) {
                GameManager.Instance.SetMainMan(actor);
                GameManager.Instance.SetMainControl(entity.GetComponent<Control>());
                GameManager.Instance.SetCameraFocus(entity.UnityTransform);
                entity.DontDestroyOnLoadWithUnity();
            }

            if (this.controlable) {
                GameManager.Instance.AddControlTarget(entity.GetComponent<Control>());
            }

            return actor;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(this.transform.position, 0.25f);
        }
    }
}