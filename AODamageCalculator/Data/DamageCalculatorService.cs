using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.SpecialAttacks;

namespace AODamageCalculator.Data
{
    public class DamageCalculatorService : IDamageCalculatorService
    {
        private readonly IEnumerable<ISpecialAttack> _specialAttacks;

        public DamageCalculatorService(IEnumerable<ISpecialAttack> specialAttacks)
        {
            _specialAttacks = specialAttacks;
        }

        public CalculationResult Calculate(WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var fightTime = 60;
            var iterations = 500;
            var numberOfAttacksByWeapon = GetNumberOfAttacks(weaponSet, playerInfo, fightTime);
            var weaponResults = new Dictionary<string, WeaponResult>();
            var specialAttackResult = new SpecialAttackResult();
            foreach (var (weaponInfo, attacks) in numberOfAttacksByWeapon)
            {
                weaponResults.Add(weaponInfo.Name, new WeaponResult(weaponInfo));
            }

            foreach (var iteration in Enumerable.Range(0, iterations))
            {
                // Regular attacks handling
                foreach (var (weaponInfo, attacks) in numberOfAttacksByWeapon)
                {
                    var weaponResult = weaponResults[weaponInfo.Name];
                    var damageResults = new List<DamageResult>();
                    foreach (var attack in Enumerable.Range(0, attacks))
                    {
                        damageResults.Add(AttackHelper.RegularAttack(weaponInfo, playerInfo));
                    }
                    weaponResult.AddIterationResult(new IterationResult(iteration, damageResults));
                }

                // Special attacks handling
                specialAttackResult.AddIterationResult(new SpecialAttackIterationResult(iteration, CalculateSpecialAttacks(fightTime, weaponSet, playerInfo))); ;
            }

            return new CalculationResult(weaponResults.Values.ToList(), specialAttackResult, fightTime, iterations);
        }

        private Dictionary<string, List<DamageResult>> CalculateSpecialAttacks(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var result = _specialAttacks
                .Where(sa => sa.IsEnabled(weaponSet))
                .Select(sa => sa.GetAttackResult(fightTime, weaponSet, playerInfo));

            return result.ToDictionary(r => r.Item1, r => r.Item2);
        }

        private Dictionary<WeaponInfo, int> GetNumberOfAttacks(WeaponSet weaponSet, PlayerInfo playerInfo, int fightTime)
        {
            var attacks = new Dictionary<WeaponInfo, int>();
            if (weaponSet.OffHandInUse)
            {
                var granularityMultiplier = 1000;
                var mainHandAttacks = 0;
                var mainHandAttackCounter = 0;
                var mainHandRechargeCounter = 0;
                var mainHandAttacking = true;
                var offHandAttacks = 0;
                var offHandAttackCounter = 0;
                var offHandRechargeCounter = weaponSet.OffHand.AdjustedRechargeTime(playerInfo) * granularityMultiplier;
                var mainHandAttackTicks = weaponSet.MainHand.AdjustedAttackTime(playerInfo) * granularityMultiplier;
                var mainHandRechargeTicks = weaponSet.MainHand.AdjustedRechargeTime(playerInfo) * granularityMultiplier;
                var offHandAttackTicks = weaponSet.OffHand.AdjustedAttackTime(playerInfo) * granularityMultiplier;
                var offHandRechargeTicks = weaponSet.OffHand.AdjustedRechargeTime(playerInfo) * granularityMultiplier;
                var offHandAttacking = false;
                foreach (var tick in Enumerable.Range(0, fightTime * granularityMultiplier))
                {
                    if (!mainHandAttacking && !offHandAttacking)
                    {
                        mainHandRechargeCounter++;
                        offHandRechargeCounter++;
                    }

                    if (mainHandAttacking)
                        mainHandAttackCounter++;
                    if (offHandAttacking)
                        offHandAttackCounter++;

                    if (mainHandAttackCounter >= mainHandAttackTicks)
                    {
                        mainHandAttacks++;
                        mainHandAttackCounter = 0;
                        mainHandAttacking = false;
                    }
                    if (mainHandRechargeCounter >= mainHandRechargeTicks && !offHandAttacking)
                    {
                        mainHandRechargeCounter = 0;
                        mainHandAttacking = true;
                    }

                    if (offHandAttackCounter >= offHandAttackTicks)
                    {
                        offHandAttacks++;
                        offHandAttackCounter = 0;
                        offHandAttacking = false;
                    }
                    if (offHandRechargeCounter >= offHandRechargeTicks && !mainHandAttacking)
                    {
                        offHandRechargeCounter = 0;
                        offHandAttacking = true;
                    }
                }
                attacks.Add(weaponSet.MainHand, mainHandAttacks);
                attacks.Add(weaponSet.OffHand, offHandAttacks);
            }
            else
            {
                var weaponInfo = weaponSet.MainHand;
                var attackCycle = weaponInfo.AdjustedAttackTime(playerInfo) + weaponInfo.AdjustedRechargeTime(playerInfo);

                var numberOfAttacks = (int)(fightTime / attackCycle);
                attacks.Add(weaponInfo, numberOfAttacks);
            }

            return attacks;
        }
    }
}
