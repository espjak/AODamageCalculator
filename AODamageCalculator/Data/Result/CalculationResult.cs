using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data.Result
{
    public class CalculationResult
    {
        public CalculationResult()
        {
            WeaponResults = new List<WeaponResult>();
            SpecialAttackResults = new SpecialAttackResults();
            DamagePieChart = new MudChartData();
        }

        public CalculationResult(PlayerInfo playerInfo, List<WeaponResult> weaponResults, SpecialAttackResults specialAttackResults, int fightTime, int iterations)
        {
            TotalDamage = weaponResults.Sum(ws => ws.TotalDamage) + specialAttackResults.GetAveragedResults().Sum(avgResult => avgResult.Value.Damage);
            PlayerInfo = playerInfo;
            WeaponResults = weaponResults;
            SpecialAttackResults = specialAttackResults;
            FightTime = fightTime;
            Iterations = iterations;
            DamagePieChart = BuildDamagePieChartItems(weaponResults, specialAttackResults);
        }

        public int TotalDamage { get; set; }

        public PlayerInfo PlayerInfo { get; set; }

        public List<WeaponResult> WeaponResults { get; set; }

        public SpecialAttackResults SpecialAttackResults { get; set; }

        public MudChartData DamagePieChart { get; set; }

        public int Iterations { get; set; }

        public int FightTime { get; set; }

        private MudChartData BuildDamagePieChartItems(List<WeaponResult> weaponResults, SpecialAttackResults specialAttackResults)
        {
            var dataList = new List<double>();
            var labelList = new List<string>();

            weaponResults.ForEach(wr =>
            {
                dataList.Add(wr.TotalDamage);
                labelList.Add(wr.Weapon.Name);
            });

            foreach (var (name, avgResult) in specialAttackResults.GetAveragedResults())
            {
                dataList.Add(avgResult.Damage);
                labelList.Add(name);
            }

            return new MudChartData { Data = dataList.ToArray(), Labels = labelList.ToArray() };
        }
    }
}
