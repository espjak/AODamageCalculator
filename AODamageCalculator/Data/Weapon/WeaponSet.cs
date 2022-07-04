namespace AODamageCalculator.Data.Weapon
{
    public class WeaponSet
    {
        public WeaponEntity MainHand { get; set; } = new();

        public WeaponEntity OffHand { get; set; } = new();

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
