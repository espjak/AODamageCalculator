using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class CalculationResult
    {
        public CalculationResult()
        {
            WeaponResults = new List<WeaponResults>();
        }

        public CalculationResult(List<WeaponResults> weaponResults, int fightTime, int iterations)
        {
            TotalDamage = weaponResults.Sum(ws => ws.TotalDamage);
            WeaponResults = weaponResults;
            FightTime = fightTime;
            Iterations = iterations;
        }

        public int TotalDamage { get; set; }

        public List<WeaponResults> WeaponResults { get; set; }

        public int Iterations { get; set; }

        public int FightTime { get; set; }
    }
}
