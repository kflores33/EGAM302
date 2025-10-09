using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// handles weapon info during runtime

[CreateAssetMenu( menuName = "Weapons/OwnedWeaponDB")]
public class OwnedWeaponDB : ScriptableObject
{
    [System.NonSerialized] public UniversalValues UniversalValues;

    public List<OwnedWeapon> ownedWeapons = new List<OwnedWeapon>();

    public OwnedWeapon GetWeapon(string id)
    {
        return ownedWeapons.FirstOrDefault(w => w.weapon_id == id); // using System.Linq, FirstOrDefault() returns the first element that matches the specified condition
                                                                    // In this instance, "w" (OwnedWeapon) is the value that gets returned if its id matches the value given
                                                                    // "OrDefault" makes it so that it doesn't return an exception if the value comes back null
    }

    public void AddWeapon(WeaponScriptable weapon)
    {
        if (GetWeapon(weapon.weapon_id) != null) return; // if this weapon already exists in the list of owned weapons, ignore
        ownedWeapons.Add(new OwnedWeapon
        {
            weapon_id = weapon.weapon_id,
            currentLevel = 0,
            killCount = 0,
            weaponData = weapon
        });
    }

    public List<ShopWeapon> shopWeapons = new List<ShopWeapon>();

    public ShopWeapon GetShopWeapon(string id)
    {
        return shopWeapons.FirstOrDefault(s => s.weapon_id == id);
    }

    public void AddShopWeapon(WeaponScriptable weapon)
    {
        if (GetShopWeapon(weapon.weapon_id) != null) return;
        shopWeapons.Add(new ShopWeapon
        {
            weapon_id = weapon.weapon_id,
            price = weapon.levels[2].kills_to_level * UniversalValues.killReqToBlood,
            weaponData = weapon
        });
    }
    public void RemoveShopWeapon(ShopWeapon weapon)
    {
        if (!shopWeapons.Contains(weapon)) return;
        shopWeapons.Remove(weapon);
    }
}

[System.Serializable]
public class OwnedWeapon
{
    public string weapon_id;
    public int currentLevel;
    public int killCount; // can subtract from level up requirement to get # of kills till next level

    [System.NonSerialized] public WeaponScriptable weaponData;
}
[System.Serializable]
public class ShopWeapon
{
    public string weapon_id;
    public float price;

    [System.NonSerialized] public WeaponScriptable weaponData;
}
