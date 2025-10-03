using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Characters/Character")]
public class CharacterScriptable : ScriptableObject
{
    public string characterName; 
    public string character_id; 
    public Material characterMaterial; 
    public List<string> weapon_proficiencies; // e.g., "sword", "axe"
    public float proficiency_modifier = 1; // e.g., 1.1 for 10% damage increase
    public int max_weapon_count = 1;
    public float max_HP = 30;
    public float respawn_time;
}
