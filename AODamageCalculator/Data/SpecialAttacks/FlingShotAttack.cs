using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class FlingShotAttack : ISpecialAttack
    {
        public (string, List<DamageResult>) GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponInfo = weaponSet.MainHand.GetSpecial(SupportedWeaponSpecials.FlingShot).IsEnabled
                ? weaponSet.MainHand
                : weaponSet.OffHand;

            var specialAttack = weaponInfo.GetSpecial(SupportedWeaponSpecials.FlingShot);

            var rechargeTime = Math.Max(6.0 + weaponInfo.AttackTime, (weaponInfo.AttackTime * 15.0) - (specialAttack.SkillValue / 100.0));
            var wholeNumberOfAttacks = (int)(fightTime / rechargeTime);

            var result = Enumerable.Range(0, wholeNumberOfAttacks).Select(a => AttackHelper.RegularAttack(weaponInfo, playerInfo)).ToList();
            return (SupportedWeaponSpecials.FlingShot, result);
        }

        public bool IsEnabled(WeaponSet weaponSet)
        {
            return weaponSet.MainHand.GetSpecial(SupportedWeaponSpecials.FlingShot).IsEnabled ||
                   (weaponSet.OffHandInUse && weaponSet.OffHand.GetSpecial(SupportedWeaponSpecials.FlingShot).IsEnabled);
        }
    }
}
