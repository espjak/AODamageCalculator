namespace AODamageCalculator.Data.Weapon
{
    public class WeaponEntity
    {
        public string Name { get; set; }

        public int Ql { get; set; } = 1;

        public WeaponInfo WeaponInfo { get; set; } = new WeaponInfo();

        public WeaponDetails WeaponDetails { get; set; } = new WeaponDetails();

        public void UpdateWeapon(WeaponInfo weaponInfo, WeaponDetails weaponDetails, int ql)
        {
            Name = weaponInfo.Name;
            Ql = ql;
            WeaponInfo = weaponInfo;
            WeaponDetails = weaponDetails;
        }
    }
}
