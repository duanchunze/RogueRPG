namespace Hsenl {
    public struct PliDamageArbitramentForm {
        public enum ResultType : byte {
            Success,
            NotFindComponent,
            Break,
            TargetInvalid,
            TargetDisappear, // 目标消失（dispose了）
        }

        public Harmable harm;
        public Hurtable hurt;

        private Numerator _harmNumerator;
        private Numerator _hurtNumerator;

        public Numerator HarmNumerator => this._harmNumerator ??= this.harm.GetComponent<Numerator>();
        public Numerator HurtNumerator => this._hurtNumerator ??= this.hurt.GetComponent<Numerator>();

        public Component source; // 伤害源, 比如某技能或某状态

        public DamageType damageType;
        public float damage;

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
        public bool backhit; // 背后攻击
        public bool ispcrit; // 是否物暴
        public float fluctuate; // 波动比例
        public float score; // 评分（roll的怎么样）
    }
}