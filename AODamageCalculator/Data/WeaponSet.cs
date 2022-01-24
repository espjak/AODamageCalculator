namespace AODamageCalculator.Data
{
    public class WeaponSet
    {
        public Weapon MainHand { get; set; } = new Weapon();

        public Weapon OffHand { get; set; } = new Weapon();

        public bool OffHandInUse => OffHand.Name != null;
    }
}
