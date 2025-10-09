using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// handles weapon info during runtime

[CreateAssetMenu( menuName = "Weapons/OwnedWeaponDB")]
public class OwnedWeaponDB : ScriptableObject
{
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
}

[System.Serializable]
public class OwnedWeapon
{
    public string weapon_id;
    public int currentLevel;
    public int killCount; // can subtract from level up requirement to get # of kills till next level

    [System.NonSerialized] public WeaponScriptable weaponData;
}
