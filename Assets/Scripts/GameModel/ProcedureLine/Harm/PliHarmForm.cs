using Hsenl.numeric;

namespace Hsenl {
    public struct PliHarmForm {
        public enum ResultType : byte {
            Success,
            NotFindComponent,
            Break,
            TargetInvalid,
            TargetDisappear, // 目标消失（dispose了）
        }

        public Harmable harmable;
        public Hurtable hurtable;
        public Component source; // 伤害源, 比如某技能或某状态

        public Numerator harmNumerator;
        public Numerator hurtNumerator;
        public Numerator sourceNumerator;

        public DamageFormulaInfo damageFormulaInfo; // 可以直接指定伤害信息, 如果该值为空的话, 则必须指定下面两个参数
        public DamageType damageType;
        public float damage;
        public float damageRatio;
        
        // ------------------- 以上参数需要提前提供

        public float amr; // 护甲
        public float lrt; // 光抗
        public float drt; // 暗抗
        public float frt; // 火抗
        public float irt; // 冰抗
        public float dex; // 攻击者命中
        public float eva; // 被攻击者闪避
        public float pvamp; // 物理吸血
        public float pcrit; // 物理暴击
        public float pcit; // 物理暴击强化
        public float blk; // 格挡率
        public float astun; // 攻击硬直
        public float hstun; // 受击硬直

        public string hitsound;
        public string hitfx;

        // 结果

        public ResultType success;
        public int finalDamage;

        public bool iseva; // 是否闪避
        public bool isblk; // 是否格挡
        public bool sneakAtk; // 是否是偷袭(敌人没有目标时, 攻击敌人)
        public bool backhit; // 背后攻击
        public bool ispcrit; // 是否物暴
        public float fluctuate; // 波动比例
        public float score; // 评分（roll的怎么样）
    }
}