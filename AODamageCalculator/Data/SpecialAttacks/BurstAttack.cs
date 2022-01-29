using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class BurstAttack : ISpecialAttack
    {
        public (string, List<DamageResult>) GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.GetSpecial(ImplementedWeaponSpecials.Burst).IsEnabled
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var specialAttack = weaponDetails.GetSpecial(ImplementedWeaponSpecials.Burst);

            var rechargeTime = Math.Max(8.0 + weaponDetails.AttackTime, (weaponDetails.RechargeTime * 20.0 + (specialAttack.Modifier / 100.0)) - (specialAttack.SkillValue / 25.0));
            var wholeNumberOfBursts = (int)(fightTime / rechargeTime);

            var result = Enumerable.Range(0, wholeNumberOfBursts * 3).Select(a => AttackHelper.RegularAttack(weaponDetails, playerInfo, supportsCriticalHit: false)).ToList();
            return (ImplementedWeaponSpecials.Burst, result);
        }

        public bool IsEnabled(WeaponSet weaponSet) => weaponSet.IsWeaponSpecialEnabled(ImplementedWeaponSpecials.Burst);
    }
}
