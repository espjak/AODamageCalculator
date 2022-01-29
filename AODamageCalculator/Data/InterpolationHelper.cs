namespace AODamageCalculator.Data
{
    public static class InterpolationHelper
    {
        //  y = y1 + ((x - x1) / (x2 - x1)) * (y2 - y1)
        public static int LinearInterpolation(float ql, (float ql, float value) lowerQlValue, (float ql, float value) higherQlValue)
        {
            if (ql.Equals(lowerQlValue.ql))
                return (int)lowerQlValue.value;
            if (ql.Equals(higherQlValue.ql))
                return (int)higherQlValue.value;

            return (int)(lowerQlValue.value + (ql - lowerQlValue.ql) / (higherQlValue.ql - lowerQlValue.ql) * (higherQlValue.value - lowerQlValue.value));
        }
    }
}
