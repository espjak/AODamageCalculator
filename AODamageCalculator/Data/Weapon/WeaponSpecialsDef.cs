using System.Collections.Generic;

namespace AODamageCalculator.Data.Weapon
{
    public class WeaponSpecialsDef
    {
        public int BurstSkill { get; set; }

        public int BurstRecharge { get; set; }

        public int FlingShotSkill { get; set; }

        public int FastAttackSkill { get; set; }

        public int BrawlSkill { get; set; }

        public int FullAutoSkill { get; set; }

        public int FullAutoRecharge { get; set; }

        public IEnumerable<WeaponSpecial> ToWeaponSpecials()
        {
            if (BurstSkill > 0)
                yield return new WeaponSpecial { Name = ImplementedWeaponSpecials.Burst, Modifier = BurstRecharge };
            if (FlingShotSkill > 0)
                yield return new WeaponSpecial { Name = ImplementedWeaponSpecials.FlingShot };
            if (FastAttackSkill > 0)
                yield return new WeaponSpecial { Name = ImplementedWeaponSpecials.FastAttack };
            if (BrawlSkill > 0)
                yield return new WeaponSpecial { Name = ImplementedWeaponSpecials.Brawl };
            if (FullAutoSkill > 0)
                yield return new WeaponSpecial { Name = ImplementedWeaponSpecials.FullAuto, Modifier = FullAutoRecharge };
        }
    }
}
