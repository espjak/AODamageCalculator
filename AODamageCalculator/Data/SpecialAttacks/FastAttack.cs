using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Result;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class FastAttack : ISpecialAttack
    {
        public SpecialAttackResult GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.SpecialSupported(ImplementedWeaponSpecials.FastAttack)
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var weaponSpecial = weaponDetails.GetSpecial(ImplementedWeaponSpecials.FastAttack);

            var rechargeTime = Math.Max(6.0 + weaponDetails.AttackTime, (weaponDetails.AttackTime * 15.0) - (weaponSpecial.SkillValue / 100.0));
            var wholeNumberOfAttacks = Math.Max(1, (int)(fightTime / rechargeTime));

            var result = new SpecialAttackResult { Name = ImplementedWeaponSpecials.FastAttack };
            foreach (var attack in Enumerable.Range(0, wholeNumberOfAttacks))
            {
                result.DamageResults.Add(attack, new List<DamageResult> { AttackHelper.WeaponSpecialAttack(weaponDetails, playerInfo, weaponSpecial) });
            }

            result.Details.Add($"Number of attacks: {wholeNumberOfAttacks}");

            return result;
        }

        public bool IsEnabled(WeaponSet weaponSet) => weaponSet.IsWeaponSpecialEnabled(ImplementedWeaponSpecials.FastAttack);
    }
}
