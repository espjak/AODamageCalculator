using System;

namespace AODamageCalculator.Data
{
    public static class AttackHelper
    {
        private static readonly Random _random = new Random();

        public static DamageResult RegularAttack(WeaponDetails weaponDetails, PlayerInfo playerInfo, bool supportsCriticalHit = true)
        {
            var isCriticalHit = supportsCriticalHit && IsCriticalHit(playerInfo);
            var baseDamage = isCriticalHit ?
                weaponDetails.Damage.Max + weaponDetails.CritModifier :
                _random.Next(weaponDetails.Damage.Min, weaponDetails.Damage.Max);

            var minimumDamage = isCriticalHit ? weaponDetails.Damage.Min + weaponDetails.CritModifier : weaponDetails.Damage.Min;
            var minimumModifiedDamage = (int)GetModifiedDamage(minimumDamage, playerInfo, false);
            var modifiedDamage = (int)GetModifiedDamage(baseDamage, playerInfo);

            return new DamageResult(Math.Max(minimumModifiedDamage, modifiedDamage), isCriticalHit);
        }

        private static double GetModifiedDamage(double baseDamage, PlayerInfo playerInfo, bool useArmorClass = true)
        {
            var attackRatingReductionModifier = 0.3;
            var damageModifiedByAR = playerInfo.AttackRating <= 1000 ?
                baseDamage * (1.0 + (playerInfo.AttackRating / 400.0)) :
                baseDamage * (3.5 + ((playerInfo.AttackRating - 1000.0) * attackRatingReductionModifier) / 400.0);

            return damageModifiedByAR + playerInfo.AddDamage - (useArmorClass ? (playerInfo.TargetAC / 10.0) : 0);
        }

        public static bool IsCriticalHit(PlayerInfo playerInfo)
        {
            var criticalRoll = _random.Next(0, 100);

            return playerInfo.CritChance >= criticalRoll;
        }
    }
}
