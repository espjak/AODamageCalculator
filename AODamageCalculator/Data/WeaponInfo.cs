using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class WeaponInfo
    {
        public WeaponInfo(string name)
        {
            Name = name;
            WeaponSpecials = GetSupportedSpecials();
        }

        public string Name { get; set; }

        public int MinDamage { get; set; }

        public int MaxDamage { get; set; }

        public int CritModifier { get; set; }

        public double AttackTime { get; set; } = 1.0;

        public double RechargeTime { get; set; } = 1.0;

        public List<WeaponSpecial> WeaponSpecials { get; set; }

        public double AdjustedAttackTime(PlayerInfo playerInfo) => Math.Round(Math.Max(1.0, AttackTime - (playerInfo.Initiatives / 600.0) - ((playerInfo.AggDef - 75) / 100.0)), 2);

        public double AdjustedRechargeTime(PlayerInfo playerInfo) => Math.Round(Math.Max(1.0, RechargeTime - (playerInfo.Initiatives / 300.0) - ((playerInfo.AggDef - 75) / 100.0)), 2);

        public WeaponSpecial GetSpecial(string name) => WeaponSpecials.FirstOrDefault(ws => ws.Name.Equals(name));

        private List<WeaponSpecial> GetSupportedSpecials()
        {
            return new List<WeaponSpecial>
            {
                new WeaponSpecial { Name = SupportedWeaponSpecials.FlingShot },
                new WeaponSpecial { Name = SupportedWeaponSpecials.Burst, HasModifier = true },
            };
        }
    }
}
