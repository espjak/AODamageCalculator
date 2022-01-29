using System.Collections.Generic;
using System.Linq;

namespace AODamageCalculator.Data.SpecialAttacks
{
    public class BrawlAttack : ISpecialAttack
    {
        public (string, List<DamageResult>) GetAttackResult(int fightTime, WeaponSet weaponSet, PlayerInfo playerInfo)
        {
            var weaponDetails = weaponSet.MainHand.WeaponDetails.GetSpecial(ImplementedWeaponSpecials.Brawl).IsEnabled
                ? weaponSet.MainHand.WeaponDetails
                : weaponSet.OffHand.WeaponDetails;

            var specialAttack = weaponDetails.GetSpecial(ImplementedWeaponSpecials.Brawl);

            var rechargeTime = 15;
            var wholeNumberOfAttacks = (int)(fightTime / rechargeTime);
            var brawlWeaponDetails = CreateBrawlWeaponDetails(specialAttack.SkillValue);

            var result = Enumerable.Range(0, wholeNumberOfAttacks).Select(a => AttackHelper.RegularAttack(brawlWeaponDetails, playerInfo, (playerInfo.AttackRating + specialAttack.SkillValue) / 2)).ToList();
            return (ImplementedWeaponSpecials.Brawl, result);
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
