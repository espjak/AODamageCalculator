namespace AODamageCalculator.Data
{
    public class WeaponSet
    {
        public Weapon MainHand { get; set; } = new Weapon();

        public Weapon OffHand { get; set; } = new Weapon();

        public bool OffHandInUse => OffHand.Name != null;

        public bool IsWeaponSpecialEnabled(string special)
        {
            if (MainHand.WeaponDetails.SpecialSupported(ImplementedWeaponSpecials.Burst) && MainHand.WeaponDetails.GetSpecial(ImplementedWeaponSpecials.Burst).IsEnabled)
                return true;
            if (OffHand.WeaponDetails.SpecialSupported(ImplementedWeaponSpecials.Burst) && OffHand.WeaponDetails.GetSpecial(ImplementedWeaponSpecials.Burst).IsEnabled)
                return true;

            return false;
        }
    }
}
