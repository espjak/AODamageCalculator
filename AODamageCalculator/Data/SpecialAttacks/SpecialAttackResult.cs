using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Result;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class SpecialAttackResult
    {
        public string Name { get; set; }

        public Dictionary<int, List<DamageResult>> DamageResults { get; set; } = new();

        public int TotalDamage()
        {
            var totalDamage = DamageResults.Sum(dr =>
            {
                var damage = dr.Value.Sum(a => a.Damage);
                return DamageCap > 0 ? Math.Min(damage, DamageCap) : damage;
            });

            return totalDamage;
        }

        public int NumberOfAttacks => DamageResults.Count;

        public List<string> Details { get; set; } = new();

        public int DamageCap { get; set; }
    }
}
