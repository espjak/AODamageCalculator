using System;
using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data
{
    public class DamageCalculatorService
    {
        private Random randomizer = new Random();

        public CalculationResult Calculate(WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var fightTime = 60;
            var iterations = 1000;
            var numberOfAttacksByWeapon = GetNumberOfAttacks(weaponSet, playerInfo, fightTime);
            var weaponResults = new List<WeaponResults>();
            foreach (var kvp in numberOfAttacksByWeapon)
            {
                var iterationResults = new List<IterationResult>();
                foreach (var iteration in Enumerable.Range(0, iterations))
                {
                    var iterationResult = new List<DamageResult>();
                    foreach (var attack in Enumerable.Range(0, kvp.Value))
                    {
                        iterationResult.Add(Attack(kvp.Key, playerInfo));
                    }
                    iterationResults.Add(new IterationResult(iteration, iterationResult));
                }
                weaponResults.Add(new WeaponResults(kvp.Key, iterationResults));
            }
             
            return new CalculationResult(weaponResults, fightTime, iterations);
        }

        private DamageResult Attack(WeaponInfo weaponInfo, PlayerInfo playerInfo)
        {
            var isCrit = IsCrit(playerInfo);
            var baseDamage = isCrit ?
                weaponInfo.MaxDamage + weaponInfo.CritModifier :
                randomizer.Next(weaponInfo.MinDamage, weaponInfo.MaxDamage);

            return new DamageResult((int)GetModifiedDamage(baseDamage, playerInfo), isCrit);
        }

        private double GetModifiedDamage(double baseDamage, PlayerInfo playerInfo)
        {
            var attackRatingReductionModifier = 0.3;
            if (playerInfo.AttackRating <= 1000)
                return baseDamage * (1.0 + (playerInfo.AttackRating / 400.0)) + playerInfo.AddDamage;

            return baseDamage * (3.5 + ((playerInfo.AttackRating - 1000.0) * attackRatingReductionModifier) / 400.0) + playerInfo.AddDamage;
        }

        private Dictionary<WeaponInfo, int> GetNumberOfAttacks(WeaponSet weaponSet, PlayerInfo playerInfo, int fightTime)
        {
            var attacks = new Dictionary<WeaponInfo, int>();
            if (weaponSet.OffHandInUse)
            {
                var mainHandAttacks = 0;
                var mainHandAttackCounter = 0;
                var mainHandRechargeCounter = 0;
                var mainHandAttacking = true;
                var offHandAttacks = 0;
                var offHandAttackCounter = 0;
                var offHandRechargeCounter = weaponSet.OffHand.AdjustedRechargeTime(playerInfo);
                var offHandAttacking = false;
                foreach (var second in Enumerable.Range(0, fightTime))
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

                    if (mainHandAttackCounter >= weaponSet.MainHand.AdjustedAttackTime(playerInfo))
                    {
                        mainHandAttacks++;
                        mainHandAttackCounter = 0;
                        mainHandAttacking = false;
                    }
                    if (mainHandRechargeCounter >= weaponSet.MainHand.AdjustedRechargeTime(playerInfo) && !offHandAttacking)
                    {
                        mainHandRechargeCounter = 0;
                        mainHandAttacking = true;
                    }

                    if (offHandAttackCounter >= weaponSet.OffHand.AdjustedAttackTime(playerInfo))
                    {
                        offHandAttacks++;
                        offHandAttackCounter = 0;
                        offHandAttacking = false;
                    }
                    if (offHandRechargeCounter >= weaponSet.OffHand.AdjustedRechargeTime(playerInfo) && !mainHandAttacking)
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

        private bool IsCrit(PlayerInfo playerInfo)
        {
            var critRoll = randomizer.Next(0, 100);

            return playerInfo.CritChance >= critRoll; 
        }
    }
}
