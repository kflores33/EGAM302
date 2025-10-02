using UnityEngine;
using System.Collections.Generic;

public class CharacterBehavior : MonoBehaviour
{
    public GameManager gameManager; // reference to game manager for accessing global game data

    public CharacterScriptable characterData; // reference to character data scriptable object

    public List<GameObject> weaponSlot = new List<GameObject>(); // references to weapon slots (empty game objects as children)
    public List<WeaponBehavior> heldWeapons = new List<WeaponBehavior>(); // reference to currently equipped weapon(s)

    // if theres a child with the weapon behavior script, switch to active state and grab reference to its data

    // get list of current enemies from game manager

    // function for damage calculation & coroutine for attack cooldown (based on weapon weight) 
    // add character specific modifiers (weapon proficiency)

    // send out damage to enemy on hit (via event system?) 

    //lock onto closest enemy
}
