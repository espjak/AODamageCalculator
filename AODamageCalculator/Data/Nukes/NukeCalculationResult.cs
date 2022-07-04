namespace AODamageCalculator.Data.Nukes
{
    public class NukeCalculationResult
    {
        public int TotalDamage { get; set; }

        public PlayerInfo PlayerInfo { get; set; }

        public int Iterations { get; set; }

        public int FightTime { get; set; }

        public double DamagePerSecond { get; set; }
    }
}
