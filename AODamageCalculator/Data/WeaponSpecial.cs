namespace AODamageCalculator.Data
{
    public class WeaponSpecial
    {
        public string Name { get; set; }

        public int SkillValue { get; set; }

        public bool HasModifier { get; set; }

        public int Modifier { get; set; }

        public bool IsEnabled => SkillValue > 0;
    }
}
