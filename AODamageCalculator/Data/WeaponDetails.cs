using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class WeaponDetails
    {
        public IntRange Damage { get; set; } = new IntRange(0, 0);

        public int CritModifier { get; set; }

        public double AttackTime { get; set; } = 1.0;

        public double RechargeTime { get; set; } = 1.0;

        public List<WeaponSpecial> WeaponSpecials { get; set; } = new List<WeaponSpecial>();

        public double AdjustedAttackTime(PlayerInfo playerInfo) => Math.Round(Math.Max(1.0, AttackTime - (playerInfo.Initiatives / 600.0) - ((playerInfo.AggDef - 75) / 100.0)), 2);

        public double AdjustedRechargeTime(PlayerInfo playerInfo) => Math.Round(Math.Max(1.0, RechargeTime - (playerInfo.Initiatives / 300.0) - ((playerInfo.AggDef - 75) / 100.0)), 2);

        public WeaponSpecial GetSpecial(string name) => WeaponSpecials.FirstOrDefault(ws => ws.Name.Equals(name));

        public bool SpecialSupported(string name) => WeaponSpecials.Any(ws => ws.Name.Equals(name));

        public void AddWeaponSpecials(List<WeaponSpecial> weaponSpecials)
        {
            WeaponSpecials.AddRange(weaponSpecials);
        }
    }
}
