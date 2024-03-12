using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    // 挂载该组件可以掉落 Pickable
    // 掉落的方式分为以下几种:
    // 1、不固定掉落数, 每种掉落物都有自己的掉落概率, 概率roll到了, 就掉落.
    // 2、固定掉落数, 掉落数在一个指定的区间, 具体掉落多少通过权重来roll, 确定掉落数之后, 则在候选池中随机选该数的目标, 每种掉落物的权重也不同, 且可以重复, 所以高权重的会积压低权重的
    //    比如, 三个掉落物的权重分别为90, 1, 1, 那么90就会极大的积压另两个被随机到的概率. 效果就是, 每次掉落几乎都只掉落若干个90, 其他一个都没有
    // 3、也是固定掉落数, 大体上与第二种相同, 唯一区别就是不会重复, 例如上面的90, 1, 1, 当90被选中后, 就会从候选人中剔除, 造成的效果就是, 90几乎每一次都会被随机到, 但其他目标也会正常掉落
    [Serializable]
    public class Dropable : Unbodied {
        private List<DropByWeight> _dropByWeights = new();
        private List<int> _weights = new();
        private List<DropByProbability> _dropByProbabilities = new();
        private int2 _dropNumeberRange; // 可能掉落的物品的个数区间
        private List<int> _dropNumberWeights = new(); // 每个掉落个数的权重

        public Vector3 DropRange { get; private set; } = new Vector3(1, 1, 1);

        public void AddPossibleDrop(DropByWeight dropByWeight) {
            this._dropByWeights.Add(dropByWeight);
            this._weights.Add(dropByWeight.weight);
        }

        public void AddPossibleDrops(int count, Action<DropByWeight> callback) {
            for (var i = 0; i < count; i++) {
                var info = new DropByWeight();
                callback.Invoke(info);
                this._dropByWeights.Add(info);
                this._weights.Add(info.weight);
            }
        }

        public void AddPossibleDrop(DropByProbability dropByWeight) {
            this._dropByProbabilities.Add(dropByWeight);
        }

        public void AddPossibleDrops(int count, Action<DropByProbability> callback) {
            for (var i = 0; i < count; i++) {
                var info = new DropByProbability();
                callback.Invoke(info);
                this._dropByProbabilities.Add(info);
            }
        }

        public void SetDropCount(int min, int max, IList<int> weights) {
            if (weights.Count != max - min + 1)
                throw new ArgumentException("The quantity of drops count and weights must be the same");

            this._dropNumeberRange = new int2(min, max);
            this._dropNumberWeights.Clear();
            for (int i = 0, len = weights.Count; i < len; i++) {
                this._dropNumberWeights.Add(weights[i]);
            }
        }

        public void Drop(DropMode dropMode) {
            switch (dropMode) {
                case DropMode.UnfixedCount: {
                    foreach (var drop in this._dropByProbabilities) {
                        var rand = RandomHelper.mtRandom.NextFloat();
                        if (rand <= drop.probability) {
                            if (drop.stackable) {
                                PickableManager.Instance.Rent(drop.id, this.transform.Position, drop.count);
                            }
                            else {
                                var posMin = this.transform.Position - this.DropRange;
                                var posMax = this.transform.Position + this.DropRange;
                                for (var i = 0; i < drop.count; i++) {
                                    var pos = RandomHelper.mtRandom.NextFloat3(posMin, posMax);
                                    PickableManager.Instance.Rent(drop.id, pos, 1);
                                }
                            }
                        }
                    }

                    break;
                }
                case DropMode.FixedCount: {
                    var dropCount = RandomHelper.RandomInt32OfWeight(this._dropNumeberRange.x, this._dropNumeberRange.y, this._dropNumberWeights);
                    if (dropCount <= 0)
                        break;
                    
                    var finalDrops = RandomHelper.RandomArrayOfWeight(this._dropByWeights, this._weights, dropCount);

                    var posMin = this.transform.Position - this.DropRange;
                    var posMax = this.transform.Position + this.DropRange;
                    foreach (var drop in finalDrops) {
                        if (drop.stackable) {
                            PickableManager.Instance.Rent(drop.id, this.transform.Position, drop.count);
                        }
                        else {
                            for (var i = 0; i < drop.count; i++) {
                                var pos = RandomHelper.mtRandom.NextFloat3(posMin, posMax);
                                PickableManager.Instance.Rent(drop.id, pos, 1);
                            }
                        }
                    }

                    break;
                }
                case DropMode.FixedCountNotRepeat: {
                    var dropCount = RandomHelper.RandomInt32OfWeight(this._dropNumeberRange.x, this._dropNumeberRange.y, this._dropNumberWeights);
                    if (dropCount <= 0)
                        break;
                    
                    var finalDrops = RandomHelper.RandomArrayOfWeight(this._dropByWeights, this._weights, dropCount, 1);

                    var posMin = this.transform.Position - this.DropRange;
                    var posMax = this.transform.Position + this.DropRange;
                    foreach (var drop in finalDrops) {
                        if (drop.stackable) {
                            PickableManager.Instance.Rent(drop.id, this.transform.Position, drop.count);
                        }
                        else {
                            for (var i = 0; i < drop.count; i++) {
                                var pos = RandomHelper.mtRandom.NextFloat3(posMin, posMax);
                                PickableManager.Instance.Rent(drop.id, pos, 1);
                            }
                        }
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(dropMode), dropMode, null);
            }
        }

        public class DropByWeight {
            public string name;
            public int id;
            public int weight;
            public int count;
            public bool stackable;
        }

        public class DropByProbability {
            public string name;
            public int id;
            public float probability;
            public int count;
            public bool stackable;
        }
    }
}