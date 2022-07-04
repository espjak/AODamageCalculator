using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public interface ISpecialAttack
    {
        public SpecialAttackResult GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo);

        public bool IsEnabled(WeaponSet weaponSet);
    }
}
