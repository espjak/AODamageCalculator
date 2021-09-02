namespace AODamageCalculator.Data
{
    public class WeaponSet
    {
        public WeaponInfo MainHand { get; set; } = new WeaponInfo("Main hand") { MinDamage = 125, MaxDamage = 320, CritModifier = 350, AttackTime = 1.3, RechargeTime = 1.3 };

        public WeaponInfo OffHand { get; set; } = new WeaponInfo("Off hand");

        public bool OffHandInUse { get; set; }
    }
}
