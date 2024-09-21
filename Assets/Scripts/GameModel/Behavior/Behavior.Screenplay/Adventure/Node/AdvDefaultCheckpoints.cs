using System;
using UnityEngine;

namespace Hsenl {
    // 默认的就是一种类似土豆兄弟似的闯关, 比较简单
    // 当然你也可以使用类似杀戮尖塔那种闯关, 只不过需要另写一套asc(adventure scheme)
    // 正常来说, node应该分开写, 但这里图方便, 所以写在一个脚本里了
    public class AdvDefaultCheckpoints : AdvInfo<adventurescheme.DefaultCheckpointsAdventureInfo, RcdDefaultCheckpointsAdventure> {
        private CheckpointManager _checkpointManager;

        private CheckpointManager CheckpointManager {
            get => this._checkpointManager;
            set {
                if (this._checkpointManager != null) {
                    this._checkpointManager.onCheckpointPassed -= this.OnCheckpointPassed;
                }

                this._checkpointManager = value;
                this._checkpointManager.onCheckpointPassed += this.OnCheckpointPassed;
            }
        }

        protected override void OnAwake() {
            // 冒险开启后, 先获取场景加载完成的回调
            // 然后根据关卡列表, 开始加载第一个关卡
            // 在场景加载完毕后, 获取场景中的关卡管理器, 如果么有, 就报错, 如果有, 就监听关卡管理器的通关事件
            // 当通关时, 进入商店系统
            // 等玩家买完后, 从候选关卡中, 随机抽选下一个关卡进入.
            // 然后重复上面的行为
            // 当闯关完毕后, 结束, 并结算

            this.manager.beginInvoke += this.Begin;
        }

        protected override void OnDestroy() {
            this.manager.beginInvoke -= this.Begin;
            this.CheckpointManager.onCheckpointPassed -= this.OnCheckpointPassed;
        }

        private void Begin() {
            this.NextCheckpoint();
        }

        private void OnCheckpointPassed() {
            // 进入购物流程
            var shopping = ProcedureManager.Procedure.GetState<ProcedureAdventure_Shopping>();
            shopping.onShoppingFinish += this.NextCheckpoint;

            ProcedureManager.Procedure.ChangeState<ProcedureAdventure_Shopping>();
        }

        private async void NextCheckpoint() {
            var adventureConfig = this.manager.Config;
            var record = this.Record;

            if (record.currentCheckpoint > record.totalCheckpoint) // 超过了总关卡数后, 就返回
                return;

            var candidatelistInfo = adventureConfig.Checkpoints[record.currentCheckpoint % adventureConfig.Checkpoints.Count];
            record.currentCheckpoint++;

            using var checkpointAliases = ListComponent<string>.Rent();
            using var weights = ListComponent<int>.Rent();
            foreach (var candidateInfo in candidatelistInfo.Candidates) {
                checkpointAliases.Add(candidateInfo.CheckpointAlias);
                weights.Add(candidateInfo.Weight);
            }

            var aliases = RandomHelper.RandomArrayOfWeight(checkpointAliases, weights, 1);
            var checkpointConfig = Tables.Instance.TbCheckpointConfig.GetByAlias(aliases[0]);
            ProcedureManager.Procedure.ChangeState<ProcedureAdventure_ChangeScene>((checkpointConfig.SceneName, LoadSceneMode.Single));
            await ProcedureManager.Procedure.GetState<ProcedureAdventure_ChangeScene>().AsyncDone();
            this.OnSceneChanged();
        }

        private async void OnSceneChanged() {
            var objectReference = UnityEngine.Object.FindObjectOfType<SceneObjectReference>();
            if (objectReference == null)
                throw new Exception("SceneObjectReference is not find in scene");

            var originalPoint = objectReference.Get<UnityEngine.Transform>("OriginalPoint");
            GameManager.Instance.MainMan.transform.SetPosition(originalPoint.position);
            var minionsBar = GameManager.Instance.MainMan.GetComponent<MinionsBar>();
            minionsBar?.ArrangeMinionsQueue();

            await Timer.WaitFrame();

            this.CheckpointManager = objectReference.Get<CheckpointManager>();
            this.CheckpointManager.monsterHpRatio = this.manager.Config.MonsterHpRatio[this.Record.currentCheckpoint - 1];
            this.CheckpointManager.Begin();

            ProcedureManager.Procedure.ChangeState<ProcedureAdventure_Battle>();
        }
    }
}