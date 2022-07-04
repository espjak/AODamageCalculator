using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class BurstAttack : ISpecialAttack
    {
        public SpecialAttackResult GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.SpecialSupported(ImplementedWeaponSpecials.Burst)
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var weaponSpecial = weaponDetails.GetSpecial(ImplementedWeaponSpecials.Burst);

            var rechargeTime = Math.Max(8.0 + weaponDetails.AttackTime, (weaponDetails.RechargeTime * 20.0 + (weaponSpecial.Modifier / 100.0)) - (weaponSpecial.SkillValue / 25.0));
            var wholeNumberOfAttacks = Math.Max(1, (int)(fightTime / rechargeTime));

            var result = new SpecialAttackResult { Name = ImplementedWeaponSpecials.Burst };
            foreach (var attack in Enumerable.Range(0, wholeNumberOfAttacks))
            {
                result.DamageResults.Add(attack, Enumerable.Range(0, 3).Select(a => AttackHelper.WeaponSpecialAttack(weaponDetails, playerInfo, weaponSpecial, false)).ToList());
            }

            result.Details.Add($"Number of attacks: {wholeNumberOfAttacks}");

            return result;
        }

        public bool IsEnabled(WeaponSet weaponSet) => weaponSet.IsWeaponSpecialEnabled(ImplementedWeaponSpecials.Burst);
    }
}
