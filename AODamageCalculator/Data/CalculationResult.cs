using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class CalculationResult
    {
        public CalculationResult()
        {
            WeaponResults = new List<WeaponResult>();
        }

        public CalculationResult(List<WeaponResult> weaponResults, SpecialAttackResult specialAttackResult, int fightTime, int iterations)
        {
            TotalDamage = weaponResults.Sum(ws => ws.TotalDamage) + specialAttackResult.TotalDamage().Sum(td => td.Value);
            WeaponResults = weaponResults;
            SpecialAttackResult = specialAttackResult;
            FightTime = fightTime;
            Iterations = iterations;
            DamagePieChart = BuildDamagePieChartItems(weaponResults, specialAttackResult);
        }

        public int TotalDamage { get; set; }

        public List<WeaponResult> WeaponResults { get; set; }

        public SpecialAttackResult SpecialAttackResult { get; set; }

        public DamagePieChartItem[] DamagePieChart { get; set; }

        public int Iterations { get; set; }

        public int FightTime { get; set; }

        private DamagePieChartItem[] BuildDamagePieChartItems(List<WeaponResult> weaponResults, SpecialAttackResult specialAttackResult)
        {
            var items = new List<DamagePieChartItem>();

            weaponResults.ForEach(wr => items.Add(new DamagePieChartItem { Source = wr.WeaponInfo.Name, Damage = wr.TotalDamage }));
            foreach (var (name, damage) in specialAttackResult.TotalDamage())
            {
                items.Add(new DamagePieChartItem { Source = name, Damage = damage });
            }

            return items.ToArray();
        }
    }
}
