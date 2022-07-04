using System.Collections.Generic;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class SpecialAttackAveragedResult
    {
        public string Name { get; set; }

        public int Damage { get; set; }

        public int NumberOfAttacks { get; set; }

        public List<string> Details { get; set; } = new();
    }
}
