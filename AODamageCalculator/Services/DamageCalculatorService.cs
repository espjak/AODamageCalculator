using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Nukes;
using AODamageCalculator.Data.Result;
using AODamageCalculator.Data.SpecialAttacks;
using AODamageCalculator.Data.Weapon;

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
            if (!CanCalculate(weaponSet, playerInfo))
                return new CalculationResult();

            PrepareData(weaponSet, playerInfo);
            var fightTime = 60;
            var iterations = 500;
            var numberOfAttacksByWeapon = GetNumberOfAttacks(weaponSet, playerInfo, fightTime);
            var weaponResults = new Dictionary<string, WeaponResult>();
            var specialAttackResult = new SpecialAttackResults();
            foreach (var (weapon, attacks) in numberOfAttacksByWeapon)
            {
                weaponResults.Add(weapon.Name, new WeaponResult(weapon));
            }

            foreach (var iteration in Enumerable.Range(0, iterations))
            {
                // Regular attacks handling
                foreach (var (weapon, attacks) in numberOfAttacksByWeapon)
                {
                    var weaponResult = weaponResults[weapon.Name];
                    var damageResults = new List<DamageResult>();
                    foreach (var attack in Enumerable.Range(0, attacks))
                    {
                        damageResults.Add(AttackHelper.RegularAttack(weapon.WeaponDetails, playerInfo));
                    }
                    weaponResult.AddIterationResult(new IterationResult(iteration, damageResults));
                }

                // Special attacks handling
                specialAttackResult.AddIterationResult(new SpecialAttackIterationResult(iteration, CalculateSpecialAttacks(fightTime, weaponSet, playerInfo))); ;
            }

            return new CalculationResult(playerInfo, weaponResults.Values.ToList(), specialAttackResult, fightTime, iterations);
        }

        public NukeCalculationResult Calculate(NukeDetails nukeDetails, PlayerInfo playerInfo)
        {
            var adjustedAttackTime = nukeDetails.AttackTime - playerInfo.Initiatives / 200.0;
            var fightTime = 60;
            var iterations = 500;

            var numberOfAttacks = (int)(fightTime / (adjustedAttackTime + nukeDetails.RechargeTime));
            var iterationResult = new Dictionary<int, List<DamageResult>>();
            foreach (var iteration in Enumerable.Range(0, iterations))
            {
                var damageResults = new List<DamageResult>();
                foreach (var attack in Enumerable.Range(0, numberOfAttacks))
                {
                    damageResults.Add(AttackHelper.RegularAttack(nukeDetails, playerInfo, false, false));
                }
                iterationResult.Add(iteration, damageResults);
            }

            var averageDamage = (int)iterationResult.Average(kvp => kvp.Value.Sum(r => r.Damage));
            return new NukeCalculationResult
            {
                FightTime = fightTime,
                Iterations = iterations,
                PlayerInfo = playerInfo,
                TotalDamage = averageDamage,
                DamagePerSecond = averageDamage / fightTime
            };
        }

        private bool CanCalculate(WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            if (weaponSet.MainHand.Name == null)
                return false;

            return true;
        }

        private void PrepareData(WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            if (weaponSet.MainHand.Name == weaponSet.OffHand.Name)
                weaponSet.OffHand.Name = $"{weaponSet.OffHand.Name} off hand";
        }

        private Dictionary<string, SpecialAttackResult> CalculateSpecialAttacks(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var result = _specialAttacks
                .Where(sa => sa.IsEnabled(weaponSet))
                .Select(sa => sa.GetAttackResult(fightTime, weaponSet, playerInfo));

            return result.ToDictionary(r => r.Name, r => r);
        }

        private Dictionary<WeaponEntity, int> GetNumberOfAttacks(WeaponSet weaponSet, PlayerInfo playerInfo, int fightTime)
        {
            var attacks = new Dictionary<WeaponEntity, int>();
            if (weaponSet.OffHandInUse)
            {
                var granularityMultiplier = 1000;
                var mainHandAttacks = 0;
                var mainHandAttackCounter = 0;
                var mainHandRechargeCounter = 0;
                var mainHandAttacking = true;
                var offHandAttacks = 0;
                var offHandAttackCounter = 0;
                var offHandRechargeCounter = weaponSet.OffHand.WeaponDetails.AdjustedRechargeTime(playerInfo) * granularityMultiplier;
                var mainHandAttackTicks = weaponSet.MainHand.WeaponDetails.AdjustedAttackTime(playerInfo) * granularityMultiplier;
                var mainHandRechargeTicks = weaponSet.MainHand.WeaponDetails.AdjustedRechargeTime(playerInfo) * granularityMultiplier;
                var offHandAttackTicks = weaponSet.OffHand.WeaponDetails.AdjustedAttackTime(playerInfo) * granularityMultiplier;
                var offHandRechargeTicks = weaponSet.OffHand.WeaponDetails.AdjustedRechargeTime(playerInfo) * granularityMultiplier;
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
                var weapon = weaponSet.MainHand;
                var attackCycle = weapon.WeaponDetails.AdjustedAttackTime(playerInfo) + weapon.WeaponDetails.AdjustedRechargeTime(playerInfo);

                var numberOfAttacks = (int)(fightTime / attackCycle);
                attacks.Add(weapon, numberOfAttacks);
            }

            return attacks;
        }
    }
}
