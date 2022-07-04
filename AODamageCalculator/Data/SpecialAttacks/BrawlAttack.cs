using System;
using System.Collections.Generic;
using System.Linq;
using AODamageCalculator.Data.Result;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class BrawlAttack : ISpecialAttack
    {
        public SpecialAttackResult GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.SpecialSupported(ImplementedWeaponSpecials.Brawl)
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var weaponSpecial = weaponDetails.GetSpecial(ImplementedWeaponSpecials.Brawl);

            var rechargeTime = 15;
            var wholeNumberOfAttacks = Math.Max(1, fightTime / rechargeTime);
            var brawlWeaponDetails = CreateBrawlWeaponDetails(weaponSpecial.SkillValue);
            var result = new SpecialAttackResult { Name = ImplementedWeaponSpecials.Brawl };
            foreach (var attack in Enumerable.Range(0, wholeNumberOfAttacks))
            {
                result.DamageResults.Add(attack, new List<DamageResult> { AttackHelper.WeaponSpecialAttack(brawlWeaponDetails, playerInfo, weaponSpecial) });
            }

            result.Details.Add($"Number of attacks: {wholeNumberOfAttacks}");

            return result;
        }

        public bool IsEnabled(WeaponSet weaponSet) => weaponSet.IsWeaponSpecialEnabled(ImplementedWeaponSpecials.Brawl);

        public WeaponDetails CreateBrawlWeaponDetails(int brawlSkill)
        {
            var lowQlBrawl = BrawlTemplates.GetLowTemplate(brawlSkill);
            var highQlBrawl = BrawlTemplates.GetHigTemplate(brawlSkill);

            if (brawlSkill == lowQlBrawl.BrawlSkill)
                return new WeaponDetails { Damage = lowQlBrawl.Damage, CritModifier = lowQlBrawl.CritModifier };
            if (brawlSkill == highQlBrawl.BrawlSkill)
                return new WeaponDetails { Damage = highQlBrawl.Damage, CritModifier = highQlBrawl.CritModifier };

            var interpolatedMinDamage = InterpolationHelper.LinearInterpolation(brawlSkill, (lowQlBrawl.BrawlSkill, lowQlBrawl.Damage.Min), (highQlBrawl.BrawlSkill, highQlBrawl.Damage.Min));
            var interpolatedMaxDamage = InterpolationHelper.LinearInterpolation(brawlSkill, (lowQlBrawl.BrawlSkill, lowQlBrawl.Damage.Max), (highQlBrawl.BrawlSkill, highQlBrawl.Damage.Max));
            var interpolatedCritModifier = InterpolationHelper.LinearInterpolation(brawlSkill, (lowQlBrawl.BrawlSkill, lowQlBrawl.CritModifier), (highQlBrawl.BrawlSkill, highQlBrawl.CritModifier));

            return new WeaponDetails
            {
                CritModifier = interpolatedCritModifier,
                Damage = new IntRange(interpolatedMinDamage, interpolatedMaxDamage),
            };
        }
    }
}
