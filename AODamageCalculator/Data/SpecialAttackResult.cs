using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class SpecialAttackResult
    {
        private readonly List<SpecialAttackIterationResult> _iterationDamageResults;

        public SpecialAttackResult()
        {
            _iterationDamageResults = new List<SpecialAttackIterationResult>();
        }

        public void AddIterationResult(SpecialAttackIterationResult result) => _iterationDamageResults.Add(result);

        public Dictionary<string, int> TotalDamage()
        {
            var sortedDamageLists = new Dictionary<string, List<int>>();
            _iterationDamageResults.ForEach(idr =>
            {
                foreach (var (name, damageResults) in idr.Results)
                {
                    var damage = damageResults.Sum(dr => dr.Damage);
                    if (sortedDamageLists.TryGetValue(name, out var damageList))
                        damageList.Add(damage);
                    else
                        sortedDamageLists.Add(name, new List<int> { damage });
                }
            });

            return sortedDamageLists.ToDictionary(t => t.Key, t => (int)t.Value.Average());
        }
    }
}
