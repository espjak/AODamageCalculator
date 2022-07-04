namespace AODamageCalculator.Data.Result
{
    public class DamageResult
    {
        public DamageResult(int damage, bool criticalHit)
        {
            Damage = damage;
            CriticalHit = criticalHit;
        }

        public int Damage { get; set; }

        public bool CriticalHit { get; set; }
    }
}
