namespace AODamageCalculator.Data.Weapon
{
    public class WeaponSpecial
    {
        public string Name { get; set; }

        public int SkillValue { get; set; }

        public bool HasModifier => Modifier > 0;

        public int Modifier { get; set; }

        public bool IsEnabled => SkillValue > 0;
    }
}
