using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class WeaponResult
    {
        public WeaponResult(Weapon weapon)
        {
            Weapon = weapon;
            DamageResults = new List<IterationResult>();
        }

        public void AddIterationResult(IterationResult result) => DamageResults.Add(result);

        public Weapon Weapon { get; set; }

        public List<IterationResult> DamageResults { get; set; }

        public int MinimumDamage => DamageResults.Min(r => r.MinimumDamage);

        public int HighestDamage => DamageResults.Max(r => r.HighestDamage);

        public int CriticalDamage => DamageResults.Max(r => r.CriticalDamage);

        public int TotalDamage => (int)DamageResults.Average(r => r.Results.Sum(innerResult => innerResult.Damage));

        public int NumberOfAttacks => (int)DamageResults.Average(r => r.Results.Count);

        public int NumberOfCriticalHits => (int)DamageResults.Average(r => r.CriticalHits.Count());
    }
}
