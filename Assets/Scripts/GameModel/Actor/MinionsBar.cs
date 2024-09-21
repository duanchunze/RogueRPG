using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class MinionsBar : Unbodied {
        private List<Minion> _minions = new();
        private Entity _minionsHolder;
        private int _maxMinionCapacity;

        private Vector3[] _queuePointsCache;

        public int MaxMinionCapacity {
            get => this._maxMinionCapacity;
            set {
                if (this._maxMinionCapacity == value)
                    return;

                this._maxMinionCapacity = value;
                this._queuePointsCache = new Vector3[value];
            }
        }

        public IReadOnlyList<Minion> Minions => this._minions;

        protected override void OnAwake() {
            this.MaxMinionCapacity = 10;
        }

        protected override void OnDestroy() {
            if (this._minionsHolder != null)
                Destroy(this._minionsHolder);
        }

        private Entity GetOrCreateMinionsHolder() {
            if (this._minionsHolder == null) {
#if UNITY_EDITOR
                this._minionsHolder = Hsenl.Entity.Create($"{this.Entity.Name} Minions Holder", this.Entity);
#else
                this._minionsHolder = Hsenl.Entity.Create(null, this.Entity);
#endif
                this._minionsHolder.DontDestroyOnLoadWithUnity();
            }

            return this._minionsHolder;
        }

        public Minion Rent(string alias, Vector3 position) {
            var actor = ActorManager.Instance.Rent(alias, position);
            if (actor != null) {
                var minion = actor.GetOrAddComponent<Minion>();
                minion.master = this.Bodied;
                if (this._minions.Contains(minion)) {
                    Log.Error($"Minion is already haved '{minion.Name}'");
                    return null;
                }

                this._minions.Add(minion);
                minion.onOver += this.OnMinionOver;
                minion.SetParent(this.GetOrCreateMinionsHolder());
                return minion;
            }

            return null;
        }

        private void OnMinionOver(Minion m) {
            m.onOver -= this.OnMinionOver;
            this._minions.Remove(m);
        }

        // 整理随从的队列
        public void ArrangeMinionsQueue() {
            var points = this.GetMinionsQueuePoints();
            for (int i = 0; i < this._minions.Count; i++) {
                var minion = this._minions[i];
                var point = points[i];
                minion.transform.SetPosition(point);
            }
        }

        /// <summary>
        /// 获取整个随从队列的队形
        /// </summary>
        /// <returns></returns>
        public Span<Vector3> GetMinionsQueuePoints() {
            for (int i = 0; i < this._minions.Count; i++) {
                this.TryGetQueuePositionOfIndex(i, out var point);
                this._queuePointsCache[i] = point;
            }

            return this._queuePointsCache.AsSpan(0, this._minions.Count);
        }

        /// <summary>
        /// 获得一个随从在队列中应该站的位置
        /// </summary>
        /// <param name="minion"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool TryGetQueuePositionOfMinion(Minion minion, out Vector3 point) {
            for (int i = 0; i < this._minions.Count; i++) {
                if (this._minions[i] == minion) {
                    return this.TryGetQueuePositionOfIndex(i, out point);
                }
            }

            point = default;
            return false;
        }

        /// <summary>
        ///  根据随从在队列中的编号, 获取其应该站在哪个位置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool TryGetQueuePositionOfIndex(int index, out Vector3 point) {
            point = default;
            if (index < 0 || index >= this._minions.Count)
                return false;

            int columnNum = 3; // 总共分多少列
            var spaceEach = Vector3.One; // 每个随从之间的间隙空间

            // 算出当前索引在整个队列的第几排第几列
            var column = index % columnNum; // 第几列
            var row = index / columnNum; // 第几排

            // 算出当前排的中间列
            var curRowColumnNum = (this._minions.Count - 1) % columnNum; // 根据随从的总数, 算出最后一排总共有多少列
            var curRow = (this._minions.Count - 1) / columnNum; // 算出总共有多少排
            if (row != curRow) // 如果当前排不是最后一排, 那说明它肯定占满了, 所以按最大值算
                curRowColumnNum = columnNum - 1;
            float middleColumn = curRowColumnNum * 0.5f;

            var x = (column - middleColumn) * spaceEach.x;
            var z = row * spaceEach.z;
            z = -z;
            point = new Vector3(x, 0, z);
            point *= this.transform.Quaternion; // 让该点以原点为中心, 跟随主人旋转

            var anchor = this.transform.Position + this.transform.Forward * -1; // 锚点放在身后一码位置
            point += anchor;
            return true;
        }
    }
}