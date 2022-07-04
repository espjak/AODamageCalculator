using System;
using AODamageCalculator.Data.Result;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data
{
    public static class AttackHelper
    {
        private static readonly Random Random = new();

        public static DamageResult RegularAttack(IDamageDetails damageDetails, PlayerInfo playerInfo, bool supportsCriticalHit = true, bool supportsARScaling = true)
        {
            var attackRating = playerInfo.AttackRating;
            return Attack(damageDetails, playerInfo, attackRating, supportsCriticalHit, supportsARScaling);
        }

        public static DamageResult WeaponSpecialAttack(WeaponDetails weaponDetails, PlayerInfo playerInfo, WeaponSpecial weaponSpecial, bool supportsCriticalHit = true)
        {
            var attackRating = weaponSpecial.SkillValue + playerInfo.AttackRating / 2;
            return Attack(weaponDetails, playerInfo, attackRating, supportsCriticalHit);
        }

        private static DamageResult Attack(IDamageDetails damageDetails, PlayerInfo playerInfo, int attackRating, bool supportsCriticalHit = true, bool supportsARScaling = true)
        {
            var isCriticalHit = supportsCriticalHit && IsCriticalHit(playerInfo);
            var baseDamage = isCriticalHit ?
                damageDetails.Damage.Max + damageDetails.CritModifier :
                Random.Next(damageDetails.Damage.Min, damageDetails.Damage.Max);

            var minimumDamage = isCriticalHit ? damageDetails.Damage.Min + damageDetails.CritModifier : damageDetails.Damage.Min;
            var minimumModifiedDamage = (int)GetModifiedDamage(minimumDamage, playerInfo, attackRating, false, supportsARScaling);
            var modifiedDamage = (int)GetModifiedDamage(baseDamage, playerInfo, attackRating, true, supportsARScaling);

            return new DamageResult(Math.Max(minimumModifiedDamage, modifiedDamage), isCriticalHit);
        }

        private static double GetModifiedDamage(double baseDamage, PlayerInfo playerInfo, int attackRating, bool useArmorClass, bool useARScaling)
        {
            var attackRatingReductionModifier = 0.3;
            var damageModifiedByAr = attackRating <= 1000 ?
                baseDamage * (1.0 + (attackRating / 400.0)) :
                baseDamage * (3.5 + ((attackRating - 1000.0) * attackRatingReductionModifier) / 400.0);

            if (useARScaling)
                return damageModifiedByAr + playerInfo.AddDamage - (useArmorClass ? (playerInfo.TargetAC / 10.0) : 0);
            else
                return baseDamage + playerInfo.AddDamage - (useArmorClass ? (playerInfo.TargetAC / 10.0) : 0);
        }

        private static bool IsCriticalHit(PlayerInfo playerInfo)
        {
            var criticalRoll = Random.Next(0, 100);

            return playerInfo.CritChance >= criticalRoll;
        }
    }
}
