using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class FastAttack : ISpecialAttack
    {
        public (string, List<DamageResult>) GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.GetSpecial(ImplementedWeaponSpecials.FastAttack).IsEnabled
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var specialAttack = weaponDetails.GetSpecial(ImplementedWeaponSpecials.FastAttack);

            var rechargeTime = Math.Max(6.0 + weaponDetails.AttackTime, (weaponDetails.AttackTime * 15.0) - (specialAttack.SkillValue / 100.0));
            var wholeNumberOfAttacks = (int)(fightTime / rechargeTime);

            var result = Enumerable.Range(0, wholeNumberOfAttacks).Select(a => AttackHelper.RegularAttack(weaponDetails, playerInfo)).ToList();
            return (ImplementedWeaponSpecials.FastAttack, result);
        }

        public bool IsEnabled(WeaponSet weaponSet) => weaponSet.IsWeaponSpecialEnabled(ImplementedWeaponSpecials.FastAttack);
    }
}
