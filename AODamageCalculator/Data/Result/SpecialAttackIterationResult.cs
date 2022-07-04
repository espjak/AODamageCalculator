using System.Collections.Generic;
using AODamageCalculator.Data.SpecialAttacks;

namespace AODamageCalculator.Data.Result
{
    public class SpecialAttackIterationResult
    {
        public SpecialAttackIterationResult(int iteration, Dictionary<string, SpecialAttackResult> results)
        {
            Iteration = iteration;
            Results = results;
        }

        public int Iteration { get; set; }

        public Dictionary<string, SpecialAttackResult> Results { get; set; }
    }
}
