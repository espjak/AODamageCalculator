using System.Collections.Generic;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public interface ISpecialAttack
    {
        public (string, List<DamageResult>) GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo);

        public bool IsEnabled(WeaponSet weaponSet);
    }
}
