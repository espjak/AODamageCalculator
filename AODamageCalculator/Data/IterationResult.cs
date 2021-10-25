using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class IterationResult
    {
        public IterationResult(int iteration, List<DamageResult> results)
        {
            Iteration = iteration;
            Results = results;
        }

        public int Iteration { get; set; }

        public List<DamageResult> Results { get; set; }

        public int MinimumDamage => RegularHits.Min(dr => dr.Damage);

        public int HighestDamage => RegularHits.Max(dr => dr.Damage);

        public int CriticalDamage => CriticalHits.Any() ? CriticalHits.Max(dr => dr.Damage) : 0;

        public List<DamageResult> RegularHits => Results.Where(dr => !dr.CriticalHit).ToList();

        public List<DamageResult> CriticalHits => Results.Where(dr => dr.CriticalHit).ToList();
    }
}
