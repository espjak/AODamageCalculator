namespace AODamageCalculator.Data
{
    public interface IDamageDetails
    {
        public IntRange Damage { get; set; }

        public int CritModifier { get; set; }
    }
}
