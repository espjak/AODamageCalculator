using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class SpecialAttackIterationResult
    {
        public SpecialAttackIterationResult(int iteration, Dictionary<string, List<DamageResult>> results)
        {
            Iteration = iteration;
            Results = results;
        }

        public int Iteration { get; set; }

        public Dictionary<string, List<DamageResult>> Results { get; set; }
    }
}
