using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using AODamageCalculator.Data;

namespace AODamageCalculator.Services
{
    public class WeaponService
    {
        private readonly Lazy<Dictionary<string, List<Weapon>>> _weapons;
        private readonly Lazy<List<WeaponInfo>> _weaponInfos;

        public WeaponService()
        {
            _weapons = new Lazy<Dictionary<string, List<Weapon>>>(GetWeaponsFromDb);
            _weaponInfos = new Lazy<List<WeaponInfo>>(() => BuildWeaponInfos(_weapons.Value));
        }

        public IEnumerable<WeaponInfo> SearchWeapons(string search)
        {
            if (search == null)
                return _weaponInfos.Value;

            return _weaponInfos.Value.Where(w => w.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }

        private Dictionary<string, List<Weapon>> GetWeaponsFromDb()
        {
            var allWeapons = new List<Weapon>();
            using var connection = new SQLiteConnection("Data Source=aoitems.db;");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM tblAOWeapons JOIN tblAODamage ON tblAOWeapons.aoid = tblAODamage.aoid";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var weapon = new Weapon();
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
                }
                weaponDetails.AddWeaponSpecials(weaponSpecialsDef.ToWeaponSpecials().ToList());
                weapon.WeaponInfo = weaponInfo;
                weapon.WeaponDetails = weaponDetails;

                allWeapons.Add(weapon);
            }
            
            return allWeapons.GroupBy(w => w.Name).ToDictionary(g => g.Key, g => g.ToList());
        }

        private List<WeaponInfo> BuildWeaponInfos(Dictionary<string, List<Weapon>> weapons)
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

                var interpolatedMinDamage = LinearInterpolation(ql, (lowestQlWeapon.Ql, lowestQlWeapon.WeaponDetails.Damage.Min), (highestQlWeapon.Ql, highestQlWeapon.WeaponDetails.Damage.Min));
                var interpolatedMaxDamage = LinearInterpolation(ql, (lowestQlWeapon.Ql, lowestQlWeapon.WeaponDetails.Damage.Max), (highestQlWeapon.Ql, highestQlWeapon.WeaponDetails.Damage.Max));
                var interpolatedDamage = new IntRange(interpolatedMinDamage, interpolatedMaxDamage);
                var interpolatedCritModifier = LinearInterpolation(ql, (lowestQlWeapon.Ql, lowestQlWeapon.WeaponDetails.CritModifier), (highestQlWeapon.Ql, highestQlWeapon.WeaponDetails.CritModifier));

                return new WeaponDetails
                {
                    AttackTime = lowestQlWeapon.WeaponDetails.AttackTime,
                    RechargeTime = lowestQlWeapon.WeaponDetails.RechargeTime,
                    CritModifier = interpolatedCritModifier,
                    Damage = new IntRange(interpolatedMinDamage, interpolatedMaxDamage)
                };
            }

            return new WeaponDetails();
        }

        //  y = y1 + ((x - x1) / (x2 - x1)) * (y2 - y1)
        private int LinearInterpolation(float ql, (float ql, float damage) lowerQlDamage, (float ql, float damage) higherQlDamage)
        {
            if (ql.Equals(lowerQlDamage.ql))
                return (int)lowerQlDamage.damage;
            if (ql.Equals(higherQlDamage.ql))
                return (int) higherQlDamage.damage;

            return (int)(lowerQlDamage.damage + (ql - lowerQlDamage.ql)/(higherQlDamage.ql-lowerQlDamage.ql) * (higherQlDamage.damage - lowerQlDamage.damage));
        }
    }
}
