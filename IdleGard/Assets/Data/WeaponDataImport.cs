using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Collections;


// this should (in theory) import the .json data and populate scriptable objects with the information (i dont know how to do that yet...)
public class WeaponDataImport : MonoBehaviour
{
    public TextAsset WeaponJson;




}

[System.Serializable]
public class WeaponLevel
{
    public int level; // not to be confused with its number in the index---this just sorts the list numerically

    public string story;
    public int attack_strength; // for this game's purposes, damage done per hit
    public string weight; // generalized to one of 4 weight classes

    // ability variables
    public float MP_cost; // amount of "mana" required to cast
    public string ability_desc; // to be displayed to player
    public int ability_count; // if ability has a projectile, the number of projectiles fired
    public float ability_sec; // length of ability duration, if applicable
    public float ability_atk_multi = 1; // this should be 1 unless stated otherwise
    public float ability_damage_multi; // as in, damage taken by player
    public float ability_speed_multi; // speed of player, but in this game it can be considered attack speed
    public int kills_to_level; // amount of kills required to level up to next level 
}
[System.Serializable]
public class WeaponJsonData
{
    public string wpnname; // to be displayed to player
    public List<string> type; // can be of multiple types
    public List<string> game; // can be from multiple games
    public string weapon_id;
    public string ability_name;
    public List<WeaponLevel> levels; // index 0 = level 1, index 1 = level 2, etc.
}