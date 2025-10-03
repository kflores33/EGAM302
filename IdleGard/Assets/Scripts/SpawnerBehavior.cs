using Unity.VisualScripting;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    public static SpawnerBehavior instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    // public functions for spawning weapons, characters, and enemies into the scene

    public GameObject weaponPrefab;
    public GameObject characterPrefab;
    public GameObject enemyPrefab;

    //private bool shouldSpawnWeapon;

    //WeaponInvSlot weaponInvSlot;
    //WeaponScriptable selectedWeapon;
    //Vector3 spawnPosition;

    //private void Update()
    //{
    //    if (shouldSpawnWeapon && Input.GetMouseButtonUp(0))
    //    {
    //        SpawnWeapon(spawnPosition, selectedWeapon, weaponInvSlot);
    //        shouldSpawnWeapon = false;
    //    }
    //}

    // Call this method from other scripts to request a weapon spawn
    //public void RequestSpawnWeapon(Vector3 position, WeaponScriptable weapon, WeaponInvSlot slot)
    //{
    //    spawnPosition = position;
    //    selectedWeapon = weapon;
    //    weaponInvSlot = slot;
    //    shouldSpawnWeapon = true;
    //}

    public void SpawnWeapon(Vector3 position, WeaponScriptable selectedWeapon, WeaponInvSlot weaponInvSlot)
    {
        GameObject weapon = Instantiate(weaponPrefab, position, Quaternion.identity);
        
        weapon.GetComponent<WeaponBehavior>().weaponData = selectedWeapon; // assign the selected weapon data to the weapon behavior script
        weapon.GetComponent<WeaponBehavior>().draggable = true;

        // move stuff below to weapon object
        if (Input.GetMouseButtonUp(0)) // check when mouse 1 is released
        {
            Debug.Log("mouse released");
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo)) 
            {
                if (hitInfo.collider == null) return; // if we didn't hit anything, exit the function

                if (hitInfo.collider.GetComponent<WeaponBehavior>() != null) // if we hit a weapon
                {
                    WeaponBehavior equippedWeapon = hitInfo.collider.GetComponent<WeaponBehavior>();
                    GameObject weaponParent = equippedWeapon.transform.parent.gameObject; // get parent of currently held weapon
                    CharacterBehavior selectedCharacter = weaponParent.GetComponentInParent<CharacterBehavior>(); // get character holding the weapon

                    if (weaponParent == null || selectedCharacter == null)
                    {
                        Debug.LogError("Weapon parent or character is null.");
                        return;
                    }

                    RemoveEquippedWeapon(equippedWeapon, weaponParent, selectedCharacter, weaponInvSlot); // remove currently held weapon
                    EquipNewWeapon(weapon.GetComponent<WeaponBehavior>(), weaponParent, selectedCharacter); // equip new weapon
                }
                else if (hitInfo.collider.GetComponent<CharacterBehavior>() != null) // if we hit a character
                {
                    CharacterBehavior selectedCharacter = hitInfo.collider.GetComponent<CharacterBehavior>();
                    GameObject selectedSlot = null; // define variable

                    if (selectedCharacter.characterData.max_weapon_count == 1) // if only one weapon can be held, drop into first slot
                    {
                        if (selectedCharacter.heldWeapons.Count > 0) // if character is already holding a weapon, remove it first
                        {
                            RemoveEquippedWeapon(selectedCharacter.heldWeapons[0], selectedCharacter.weaponSlots[0], selectedCharacter, weaponInvSlot);
                        }
                        selectedSlot = selectedCharacter.weaponSlots[0];
                    }
                    else if (selectedCharacter.characterData.max_weapon_count > 1)
                    {
                        // if multiple weapons can be held, find first empty slot
                        for (int i = 0; i < selectedCharacter.characterData.max_weapon_count; i++)
                        {
                            if (selectedCharacter.weaponSlots[i].transform.childCount == 0) // if slot is empty
                            {
                                selectedSlot = selectedCharacter.weaponSlots[i];
                                break;
                            }
                        }
                        // if all slots are full, replace the first one
                        if (selectedSlot == null)
                        {
                            RemoveEquippedWeapon(selectedCharacter.heldWeapons[0], selectedCharacter.weaponSlots[0], selectedCharacter, weaponInvSlot);
                            selectedSlot = selectedCharacter.weaponSlots[0];
                        }
                    }
                    EquipNewWeapon(weapon.GetComponent<WeaponBehavior>(), selectedSlot, selectedCharacter);
                }
            }
        }
    }
    public void RemoveEquippedWeapon(WeaponBehavior equippedWeapon, GameObject weaponParent, CharacterBehavior selectedCharacter, WeaponInvSlot weaponInvSlot)
    {
        // before doing anything, be sure to communicate with save manager to update saved data

        equippedWeapon.transform.SetParent(null); // unparent currently held weapon
        selectedCharacter.heldWeapons.Remove(equippedWeapon); // remove old weapon from character's held weapons list
        weaponInvSlot.weaponIsActive = false;

        Destroy(equippedWeapon.gameObject); // destroy currently held weapon
    }
    public void EquipNewWeapon(WeaponBehavior newWeapon, GameObject weaponParent, CharacterBehavior selectedCharacter)
    {
        newWeapon.transform.SetParent(weaponParent.transform); // set new weapon as child of character
        selectedCharacter.heldWeapons.Add(newWeapon); // add new weapon to character's held weapons list
        newWeapon.transform.localPosition = Vector3.zero; // reset position to be centered in slot
        newWeapon.draggable = false;
    }
}
