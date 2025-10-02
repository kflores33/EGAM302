using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon")]
public class WeaponScriptable : ScriptableObject
{
    public string wpnname; // to be displayed to player
    public List<string> type; // can be of multiple types
    public List<string> game; // can be from multiple games
    public string weapon_id; // used as key
    public string ability_name; // to be displayed to player
    public List<WeaponLevel> levels; // index 0 = level 1, index 1 = level 2, etc.
}
