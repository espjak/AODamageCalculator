namespace AODamageCalculator.Data.Nukes
{
    public class NukeDetails : IDamageDetails
    {
        public IntRange Damage { get; set; } = new IntRange(0, 0);

        public int CritModifier { get; set; } = 0;

        public double AttackTime { get; set; } = 1.0;

        public double MinimumAttackTime { get; set; } = 1.0;

        public double RechargeTime { get; set; } = 1.0;
    }
}
