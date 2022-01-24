﻿using System.Collections.Generic;

namespace AODamageCalculator.Data
{
    public class WeaponSpecialsDef
    {
        public int BurstSkill { get; set; }

        public int BurstRecharge { get; set; }

        public int FlingShotSkill { get; set; }

        public IEnumerable<WeaponSpecial> ToWeaponSpecials()
        {
            if (BurstSkill > 0)
                yield return new WeaponSpecial { Name = "Burst", Modifier = BurstRecharge };
            if (FlingShotSkill > 0)
                yield return new WeaponSpecial { Name = "Fling shot" };
        }
    }
}
