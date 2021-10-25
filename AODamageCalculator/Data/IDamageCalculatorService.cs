namespace AODamageCalculator.Data
{
    public interface IDamageCalculatorService
    {
        CalculationResult Calculate(WeaponSet weaponSet, PlayerInfo playerInfo);
    }
}
