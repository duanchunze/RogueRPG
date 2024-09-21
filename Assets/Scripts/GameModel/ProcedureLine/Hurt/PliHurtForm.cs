namespace Hsenl {
    public struct PliHurtForm {
        public Harmable harmable;
        public Hurtable hurtable;

        public DamageType damageType;
        public int deductHp;

        public bool iseva; // 是否闪避
        public bool isblk; // 是否格挡
        public bool sneakAtk; // 偷袭
        public bool backhit; // 背后攻击
        public bool ispcrit; // 是否物暴
        public float fluctuate; // 波动比例
        public float astun;
        public float hstun;
        public float score; // 评分（roll的怎么样）

        public bool frameSlower; // 是否需要帧减速
    }
}