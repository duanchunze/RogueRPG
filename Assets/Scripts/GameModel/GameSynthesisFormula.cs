using System.Collections.Generic;

namespace Hsenl {
    public class SynthesisInfo {
        public Bodied[] members;
        public string achievement;
    }

    // 游戏卡牌合成公式
    public static class GameSynthesisFormula {
        public static bool Match(List<Bodied> materials, out List<SynthesisInfo> synthesisInfos) {
            var results = new List<SynthesisInfo>();
            using var cache = ListComponent<Bodied>.Rent();
            foreach (var synthesisConfig in Tables.Instance.TbSynthesisConfig.DataList) {
                cache.Clear();
                // 遍历合成公式里的成员名单, 逐一比对, 如果全部都找到了, 则符合合成条件
                foreach (var synthesisMember in synthesisConfig.SynthesisMembers) {
                    var index = materials.FindIndex(s => s.Name == synthesisMember);
                    if (index == -1) {
                        goto NOMATCH;
                    }

                    cache.Add(materials[index]);
                    materials.RemoveAt(index);
                }

                var synthesisInfo = new SynthesisInfo {
                    members = cache.ToArray(),
                    achievement = synthesisConfig.SynthesisAchievement
                };

                results.Add(synthesisInfo);

                continue;

                NOMATCH:
                foreach (var bodied in cache) {
                    materials.Add(bodied);
                }
            }

            synthesisInfos = results;
            return results.Count != 0;
        }
    }
}