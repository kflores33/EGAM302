using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Weapons/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponScriptable> allWeapons; 

    private Dictionary<string, WeaponScriptable> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<string, WeaponScriptable>();
        foreach (WeaponScriptable w in allWeapons) // iterate through list of weapons
        {
            if (!lookup.ContainsKey(w.weapon_id)) // if there isn't already an entry in the dictionary with the key for the current weapon, add one
            {
                lookup[w.weapon_id] = w;
            }
        }
    }

    public WeaponScriptable GetWeaponById(string weapon_id)
    {
        if (lookup == null) Initialize(); // if there is no dictionary set up, set it up now
        
        return lookup.TryGetValue(weapon_id, out WeaponScriptable weapon) ? weapon : null;
        // (? is used in this case to make this a conditional statement where TryGetValue only returns a value if weapon_id matches a key---otherwise its null)
    }
}
