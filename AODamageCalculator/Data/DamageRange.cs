namespace AODamageCalculator.Data
{
    public class IntRange
    {
        public IntRange()
        {
        }

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; set; }

        public int Max { get; set; }

        public bool HasNoRange => Max == Min;
    }
}
