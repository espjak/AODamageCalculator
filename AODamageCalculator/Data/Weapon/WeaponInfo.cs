namespace AODamageCalculator.Data.Weapon
{
    public class WeaponInfo
    {
        public string Name { get; set; }

        public IntRange QlRange { get; set; } = new IntRange(1, 1);

        public string GetDisplayValue() 
        {
            if (Name == null)
                return string.Empty;

            return QlRange.HasNoRange ? Name : $"{Name} (QL{QlRange.Min}-{QlRange.Max})";

        }
    }
}
