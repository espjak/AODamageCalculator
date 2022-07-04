using AODamageCalculator.Data.Nukes;
using AODamageCalculator.Data.Result;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data
{
    public interface IDamageCalculatorService
    {
        CalculationResult Calculate(WeaponSet weaponSet, PlayerInfo playerInfo);

        NukeCalculationResult Calculate(NukeDetails nukeDetails, PlayerInfo playerInfo);
    }
}
