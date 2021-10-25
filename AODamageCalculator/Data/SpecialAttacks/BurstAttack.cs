using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class BurstAttack : ISpecialAttack
    {
        public (string, List<DamageResult>) GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponInfo = weaponSet.MainHand.GetSpecial(SupportedWeaponSpecials.Burst).IsEnabled
                ? weaponSet.MainHand
                : weaponSet.OffHand;

            var specialAttack = weaponInfo.GetSpecial(SupportedWeaponSpecials.Burst);

            var rechargeTime = Math.Max(8.0 + weaponInfo.AttackTime, (weaponInfo.RechargeTime * 20.0 + (specialAttack.Modifier / 100.0)) - (specialAttack.SkillValue / 25.0));
            var wholeNumberOfBursts = (int)(fightTime / rechargeTime);

            var result = Enumerable.Range(0, wholeNumberOfBursts * 3).Select(a => AttackHelper.RegularAttack(weaponInfo, playerInfo, false)).ToList();
            return (SupportedWeaponSpecials.Burst, result);
        }

        public bool IsEnabled(WeaponSet weaponSet)
        {
            return weaponSet.MainHand.GetSpecial(SupportedWeaponSpecials.Burst).IsEnabled ||
                   (weaponSet.OffHandInUse && weaponSet.OffHand.GetSpecial(SupportedWeaponSpecials.Burst).IsEnabled);
        }
    }
}
