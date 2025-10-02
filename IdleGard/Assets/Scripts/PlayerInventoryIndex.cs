using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryIndex : MonoBehaviour
{
    public static PlayerInventoryIndex instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    // house lists of player owned items & their current level status
    // a copy of each item's associated scriptable object will be created upon player unlock
    // i.e. a copy of "butchers_joy_Weapon" called "butchers_joy_Owned_Weapon" will be updated with current exp/level information (housed in this script) upon dismissal of the weapon
    // maybe

    // in any case, this script will be notified if/when weapons are dismissed or unlocked by the player and will update information when necessary
    // game manager can grab a reference to any necessary data and have it spawned in-game.

    // up to date information on weapon/character stats will be held in their respective game objects (as long as they exist) and will be relayed to this script before said game object is destroyed
    // though the game object itself may not be destroyed, just the weapon/character data (which is replaced)

    // below is hypothetical code --- these dictionaries will contain necessary data (master lists may be housed in game manager instead?)
    //public Dictionary<string, WeaponData> MasterWeaponList;
    //public Dictionary<string, CharacterData> MasterCharacterList;

    // owned ___ data is essentially a copy that will have variables that the master variant does not --- namely number of kills, current level, etc. level information can be stored in an enum
    //public Dictionary<string, OwnedWeaponData> OwnedWeaponList;
    //public Dictionary<string, OwnedCharacterData> OwnedWeaponList;
}
