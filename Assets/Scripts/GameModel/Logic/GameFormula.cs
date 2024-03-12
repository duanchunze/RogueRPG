using System;
using System.Runtime.CompilerServices;

namespace Hsenl {
    // 游戏公式计算
    public static class GameFormula {
        /// <summary>
        /// 攻速转冷却时间
        /// </summary>
        /// <param name="aspd">攻速, 1代表每秒攻击默认次数 (该次数由basic决定, 当basic为1时, 该次数为1, basic为0.5时, 该次数为2)</param>
        /// <param name="basic">基础参考时间, 默认为1</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Aspd2CoolTime(float aspd, float basic = 1) {
            // 例如现在给普攻是1的攻速, 增加50%的速度，而假设现在普攻的基础冷却为1.67秒
            // 1 / 1.67 = 0.598，相当于在攻速为1的情况下, 该普攻每秒攻击0.598下
            // 0.598 * （1 + 0.5） = 0.898，加速后，每秒攻击0.898下
            // 1 / 0.898 = 1.113，算出加速后，普攻的持续时间只需要1.113秒, 也就是普攻的cd时间
            var frequency = 1 / basic;
            frequency *= aspd;
            return 1 / frequency;
        }

        // 根据基础冷却和法术极速, 计算出实际冷却. 采用的是LOL那种线性递减式的冷却算法
        public static float CalculateCooldownTime(float cooldown, int abilityHaste) {
            var ratio = 50f / (50f + abilityHaste);
            var result = cooldown * ratio;
            return result;
        }

        // 计算格挡率, 同样是也线性递减的算法
        public static float CalculateBlockRate(int blk) {
            var ratio = 50f / (50f + blk);
            var result = 1 - ratio;
            return result;
        }

        public static float CastSpeed2CastDuration(float castDuration, float castSpeed) {
            var ratio = 50 / (50 + castSpeed);
            var result = castDuration * ratio;
            return result;
        }

        public static float HitStun2StatusDuration(float stunValue) {
            var result = stunValue * 0.01f;
            return result;
        }

        public static float CalculateFrameSlower(float astun) {
            // 总的来说, 攻击硬直越大, 帧减速的程度就越大
            return 50f / (100f + astun);
        }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="ads">攻击欲望</param>
        // /// <param name="breathingType"></param>
        // /// <returns></returns>
        // public static float CalculateBreathingTime(Num ads, BreathingType breathingType) {
        //     switch (breathingType) {
        //         case BreathingType.OnHitStun:
        //             switch ((int)ads) {
        //                 case >= 100: {
        //                     return 0;
        //                 }
        //                 
        //                 case >= 70: {
        //                     break;
        //                 }
        //                 
        //                 case >= 50: {
        //                     break;
        //                 }
        //                 
        //                 case >= 30: {
        //                     break;
        //                 }
        //                 
        //                 case >= 0: {
        //                     break;
        //                 }
        //             }
        //
        //             RandomHelper.mtRandom.NextFloat(3f, 5f);
        //             break;
        //         case BreathingType.OnCastSuccess:
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(breathingType), breathingType, null);
        //     }
        // }
    }
}