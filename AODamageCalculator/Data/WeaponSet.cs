namespace AODamageCalculator.Data
{
    public class WeaponSet
    {
        public Weapon MainHand { get; set; } = new Weapon();

        public Weapon OffHand { get; set; } = new Weapon();

        public bool OffHandInUse => OffHand.Name != null;

        public bool IsWeaponSpecialEnabled(string special)
        {
            if (MainHand.WeaponDetails.SpecialSupported(special) && MainHand.WeaponDetails.GetSpecial(special).IsEnabled)
                return true;
            if (OffHand.WeaponDetails.SpecialSupported(special) && OffHand.WeaponDetails.GetSpecial(special).IsEnabled)
                return true;

            return false;
        }
    }
}
