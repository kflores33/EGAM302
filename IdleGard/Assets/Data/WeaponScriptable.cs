using System.Collections.Generic;
using UnityEngine;
using static WeaponDataImport;

[CreateAssetMenu(menuName = "Weapons/Weapon")]
public class WeaponScriptable : ScriptableObject
{
    public string weapon_id;
    public string wpnname;
    public string ability_name;

    public List<string> type;
    public List<string> game;
    public List<WeaponLevel> levels; // index 0 = level 1, index 1 = level 2, etc.
}
