using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Result;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class FullAutoAttack : ISpecialAttack
    {
        public SpecialAttackResult GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.SpecialSupported(ImplementedWeaponSpecials.FullAuto)
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var weaponSpecial = weaponDetails.GetSpecial(ImplementedWeaponSpecials.FullAuto);

            var rechargeTime = Math.Max(10.0 + weaponDetails.AttackTime, (weaponDetails.RechargeTime * 40.0 + (weaponSpecial.Modifier / 100.0)) - (weaponSpecial.SkillValue / 25.0));
            var wholeNumberOfAttacks = Math.Max(1, (int)(fightTime / rechargeTime));
            var bullets = 5 + weaponSpecial.SkillValue / 100;

            var result = new SpecialAttackResult { Name = ImplementedWeaponSpecials.FullAuto };
            foreach (var attack in Enumerable.Range(0, wholeNumberOfAttacks))
            {
                result.DamageResults.Add(attack, PerformFullAuto(bullets, weaponSpecial, weaponDetails, playerInfo));
            }

            result.DamageCap = 15000;
            result.Details.Add($"Number of attacks: {wholeNumberOfAttacks}");
            result.Details.Add($"Number of bullets each attack: {bullets}");

            return result;
        }

        public bool IsEnabled(WeaponSet weaponSet) => weaponSet.IsWeaponSpecialEnabled(ImplementedWeaponSpecials.FullAuto);

        private List<DamageResult> PerformFullAuto(int bullets, WeaponSpecial weaponSpecial, WeaponDetails weaponDetails, PlayerInfo playerInfo)
        {
            var result = new List<DamageResult>();
            foreach (var bullet in Enumerable.Range(0, bullets))
            {
                var totalDamage = result.Sum(r => r.Damage);
                var attack = AttackHelper.WeaponSpecialAttack(weaponDetails, playerInfo, weaponSpecial);
                if (totalDamage > 10000)
                    attack.Damage /= 2;
                if (totalDamage > 11500)
                    attack.Damage /= 2;
                if (totalDamage > 13000)
                    attack.Damage /= 2;
                if (totalDamage > 14500)
                    attack.Damage /= 2;

                result.Add(attack);
            }

            return result;
        }
    }
}
