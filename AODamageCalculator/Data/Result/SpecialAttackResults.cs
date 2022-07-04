using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.SpecialAttacks;

namespace AODamageCalculator.Data.Result
{
    public class SpecialAttackResults
    {
        private readonly List<SpecialAttackIterationResult> _iterationDamageResults;

        public SpecialAttackResults()
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
                    var damage = damageResults.TotalDamage();
                    if (sortedDamageLists.TryGetValue(name, out var damageList))
                        damageList.Add(damage);
                    else
                        sortedDamageLists.Add(name, new List<int> { damage });
                }
            });

            

            return sortedDamageLists.ToDictionary(t => t.Key, t => (int)t.Value.Average());
        }

        public Dictionary<string, SpecialAttackAveragedResult> GetAveragedResults()
        {
            var allResults = new Dictionary<string, List<SpecialAttackResult>>();
            _iterationDamageResults.ForEach(iterationResult =>
            {
                foreach (var iterationResultResult in iterationResult.Results)
                {
                    if (allResults.TryGetValue(iterationResultResult.Key, out var results))
                        results.Add(iterationResultResult.Value);
                    else
                        allResults.Add(iterationResultResult.Key, new List<SpecialAttackResult> { iterationResultResult.Value });
                }
            });

            return allResults.ToDictionary(r => r.Key, r => GetAveragedResult(r.Key, r.Value));
        }

        private SpecialAttackAveragedResult GetAveragedResult(string name, List<SpecialAttackResult> allResults)
        {
            var firstResult = allResults.First();
            if (firstResult == null)
                return new SpecialAttackAveragedResult { Name = name };

            return new SpecialAttackAveragedResult
            {
                Name = name,
                Damage = (int)allResults.Average(r => r.TotalDamage()),
                NumberOfAttacks = firstResult.NumberOfAttacks,
                Details = firstResult.Details
            };
        }
    }
}
