using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using AODamageCalculator.Data;
using AODamageCalculator.Data.Weapon;

namespace AODamageCalculator.Services
{
    public class WeaponService
    {
        private readonly Lazy<Dictionary<string, List<WeaponEntity>>> _weapons;
        private readonly Lazy<List<WeaponInfo>> _weaponInfos;

        public WeaponService()
        {
            _weapons = new Lazy<Dictionary<string, List<WeaponEntity>>>(GetWeaponsFromDb);
            _weaponInfos = new Lazy<List<WeaponInfo>>(() => BuildWeaponInfos(_weapons.Value));
        }

        public IEnumerable<WeaponInfo> SearchWeapons(string search)
        {
            if (search == null)
                return _weaponInfos.Value;

            return _weaponInfos.Value.Where(w => w.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }

        private Dictionary<string, List<WeaponEntity>> GetWeaponsFromDb()
        {
            var allWeapons = new List<WeaponEntity>();
            using var connection = new SQLiteConnection("Data Source=Resources/aoitems.db;");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM tblAOWeapons JOIN tblAODamage ON tblAOWeapons.aoid = tblAODamage.aoid";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var weapon = new WeaponEntity();
                var weaponInfo = new WeaponInfo();
                var weaponDetails = new WeaponDetails();
                var weaponSpecialsDef = new WeaponSpecialsDef();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);

                    if (name == "name")
                        weaponInfo.Name = weapon.Name = reader.GetString(i);
                    else if (name == "attack")
                        weaponDetails.AttackTime = reader.GetInt32(i) / 100.0;
                    else if (name == "recharge")
                        weaponDetails.RechargeTime = reader.GetInt32(i) / 100.0;
                    else if (name == "dcrit")
                        weaponDetails.CritModifier = reader.GetInt32(i);
                    else if (name == "dmin")
                        weaponDetails.Damage.Min = reader.GetInt32(i);
                    else if (name == "dmax")
                        weaponDetails.Damage.Max = reader.GetInt32(i);
                    else if (name == "ql")
                        weapon.Ql = reader.GetInt32(i);
                    else if (name == "tburstskill")
                        weaponSpecialsDef.BurstSkill = reader.GetInt32(i);
                    else if (name == "tburstrecharge")
                        weaponSpecialsDef.BurstRecharge = reader.GetInt32(i);
                    else if (name == "tflingshot")
                        weaponSpecialsDef.FlingShotSkill = reader.GetInt32(i);
                    else if (name == "tfastattack")
                        weaponSpecialsDef.FastAttackSkill = reader.GetInt32(i);
                    else if (name == "tbrawl")
                        weaponSpecialsDef.BrawlSkill = reader.GetInt32(i);
                    else if (name == "tfullautoskill")
                        weaponSpecialsDef.FullAutoSkill = reader.GetInt32(i);
                    else if (name == "tfullautorecharge")
                        weaponSpecialsDef.FullAutoRecharge = reader.GetInt32(i);
                }
                weaponDetails.AddWeaponSpecials(weaponSpecialsDef.ToWeaponSpecials().ToList());
                weapon.WeaponInfo = weaponInfo;
                weapon.WeaponDetails = weaponDetails;

                allWeapons.Add(weapon);
            }
            
            return allWeapons.GroupBy(w => w.Name).ToDictionary(g => g.Key, g => g.ToList());
        }

        private List<WeaponInfo> BuildWeaponInfos(Dictionary<string, List<WeaponEntity>> weapons)
        {
            var weaponInfos = new List<WeaponInfo>();
            foreach (var weapon in weapons)
            {
                var lowestQl = weapon.Value.Min(w => w.Ql);
                var highestQl = weapon.Value.Max(w => w.Ql);

                weaponInfos.Add(new WeaponInfo { Name = weapon.Key, QlRange = new IntRange(lowestQl, highestQl) });
            }

            return weaponInfos;
        }

        public WeaponDetails GetWeaponDetails(string name, int ql)
        {
            if (name == null)
                return new WeaponDetails();

            if (_weapons.Value.TryGetValue(name, out var weapons))
            {
                var lowestQlWeapon = weapons.OrderBy(w => w.Ql).FirstOrDefault();
                if (ql == lowestQlWeapon.Ql)
                    return lowestQlWeapon.WeaponDetails;
                var highestQlWeapon = weapons.OrderByDescending(w => w.Ql).FirstOrDefault();
                if (ql == highestQlWeapon.Ql)
                    return highestQlWeapon.WeaponDetails;

                var interpolatedMinDamage = InterpolationHelper.LinearInterpolation(ql, (lowestQlWeapon.Ql, lowestQlWeapon.WeaponDetails.Damage.Min), (highestQlWeapon.Ql, highestQlWeapon.WeaponDetails.Damage.Min));
                var interpolatedMaxDamage = InterpolationHelper.LinearInterpolation(ql, (lowestQlWeapon.Ql, lowestQlWeapon.WeaponDetails.Damage.Max), (highestQlWeapon.Ql, highestQlWeapon.WeaponDetails.Damage.Max));
                var interpolatedCritModifier = InterpolationHelper.LinearInterpolation(ql, (lowestQlWeapon.Ql, lowestQlWeapon.WeaponDetails.CritModifier), (highestQlWeapon.Ql, highestQlWeapon.WeaponDetails.CritModifier));

                return new WeaponDetails
                {
                    AttackTime = lowestQlWeapon.WeaponDetails.AttackTime,
                    RechargeTime = lowestQlWeapon.WeaponDetails.RechargeTime,
                    CritModifier = interpolatedCritModifier,
                    Damage = new IntRange(interpolatedMinDamage, interpolatedMaxDamage),
                    WeaponSpecials = lowestQlWeapon.WeaponDetails.WeaponSpecials
                };
            }

            return new WeaponDetails();
        }
    }
}
