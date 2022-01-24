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

        public CalculationResult(PlayerInfo playerInfo, List<WeaponResult> weaponResults, SpecialAttackResult specialAttackResult, int fightTime, int iterations)
        {
            TotalDamage = weaponResults.Sum(ws => ws.TotalDamage) + specialAttackResult.TotalDamage().Sum(td => td.Value);
            PlayerInfo = playerInfo;
            WeaponResults = weaponResults;
            SpecialAttackResult = specialAttackResult;
            FightTime = fightTime;
            Iterations = iterations;
            DamagePieChart = BuildDamagePieChartItems(weaponResults, specialAttackResult);
        }

        public int TotalDamage { get; set; }

        public PlayerInfo PlayerInfo { get; set; }

        public List<WeaponResult> WeaponResults { get; set; }

        public SpecialAttackResult SpecialAttackResult { get; set; }

        public MudChartData DamagePieChart { get; set; }

        public int Iterations { get; set; }

        public int FightTime { get; set; }

        private MudChartData BuildDamagePieChartItems(List<WeaponResult> weaponResults, SpecialAttackResult specialAttackResult)
        {
            var dataList = new List<double>();
            var labelList = new List<string>();

            weaponResults.ForEach(wr =>
            {
                dataList.Add(wr.TotalDamage);
                labelList.Add(wr.Weapon.Name);
            });

            foreach (var (name, damage) in specialAttackResult.TotalDamage())
            {
                dataList.Add(damage);
                labelList.Add(name);
            }

            return new MudChartData { Data = dataList.ToArray(), Labels = labelList.ToArray() };
        }
    }
}
