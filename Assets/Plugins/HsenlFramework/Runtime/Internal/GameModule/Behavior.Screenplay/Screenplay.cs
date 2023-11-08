using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;

namespace Hsenl {
    /* 剧本
     * 剧本, 你可以把它理解为一个Manager, 他可以处理大事, 也可以处理小事, 小到比如一个小任务, 让你去杀5只野猪,
     * 大到他可以是整个游戏流程的剧本, 决定你整个游戏生涯, 触发什么剧情, 世界线发生何种改变
     * 然后, 剧本的特点就是他带有数据存储.
     * 具体用例
     * 1、开启一场出征(像元气骑士那样, 开启一场冒险, 带过程, 带规则, 有结算)
     * 2、领取一个任务
     * 3、游戏故事线(一个存档)
     *
     * 步骤:
     * 比如现在我要开启一场出征, 我需要有一个出征管理器, 这个出征管理器是由管理器和剧本组件组成.
     * 以一个档案(record)为单位, 开启一个剧本, 每个剧本下具体哪些node, 可以随便添加
     */
    [Serializable]
    public abstract class Screenplay<TRecord, TNode> : Bodied, IBehaviorTree, IUpdate where TRecord : IRecord where TNode : INode {
        public Bodied Bodied => this;

        [ShowInInspector]
        protected TRecord record;

        [ShowInInspector]
        protected TNode entryNode;

        protected INode currentNode;

        private string _recordDir;
        private string _recordPath;

        public ref TRecord Record => ref this.record;

        public IBlackboard Blackboard => null;

        public float DeltaTime { get; protected set; }

        INode IBehaviorTree.CurrentNode {
            get => this.currentNode;
            set => this.currentNode = value;
        }

        public void SetRecordPath(string dir, string recordName) {
            this._recordDir = dir;
            this._recordPath = dir + "/" + recordName + ".sprecord";
        }

        public void SetRecord(TRecord r) {
            this.record = r;
        }

        public void Save() {
            if (!Directory.Exists(this._recordDir))
                throw new Exception("record dir is not exists");

            var bin = SerializeHelper.SerializeOfMemoryPack(this.record);
            using var fs = new FileStream(this._recordPath, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);
            fs.Write(bin);
        }

        public bool Load() {
            if (!File.Exists(this._recordPath))
                throw new Exception("record path is not exists");

            using var fs = new FileStream(this._recordPath, FileMode.Open, FileAccess.Read);
            var len = (int)fs.Length;
            var bin = new byte[len];
            fs.Read(bin, 0, len);
            this.record = SerializeHelper.DeserializeOfMemoryPack<TRecord>(bin);
            return true;
        }

        void IBehaviorTree.SetEntryNode(INode node) {
            this.SetEntryNode((TNode)node);
        }

        public void SetEntryNode(TNode node) {
            this.entryNode?.DestroyNode();
            this.entryNode = node;
            if (node == null)
                return;

            this.entryNode.StartNode(this);
            if (this.RealEnable) {
                this.entryNode.OpenNode();
            }
        }

        protected override void OnDeserializedOverall() {
            this.entryNode?.StartNode(this);
            if (this.RealEnable) {
                this.entryNode?.OpenNode();
            }
        }

        protected override void OnEnable() {
            this.entryNode?.OpenNode();
        }

        protected override void OnDisable() {
            this.Abort();
            this.entryNode?.CloseNode();
        }

        protected override void OnDestroy() {
            this.SetEntryNode(default);
        }

        public NodeStatus Tick() {
            if (this.entryNode == null)
                return NodeStatus.Success;

            var status = this.entryNode.TickNode();
            return status;
        }

        protected override void OnReset() {
            this.entryNode?.ResetNode();
        }

        public void Abort() {
            this.entryNode?.AbortNode();
        }

        public void Update() {
            this.DeltaTime = TimeInfo.DeltaTime;
            this.Tick();
        }
    }
}