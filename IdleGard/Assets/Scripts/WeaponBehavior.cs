using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public WeaponScriptable weaponData; // reference to weapon data scriptable object
    public WeaponInvSlot weaponInvSlot; // reference to inventory slot this weapon came from

    public UniversalValues universalValues;

    public int currentLevel { get; }
    public float attackRate { get; private set; }

    public bool draggable;

    private void Start()
    {
        string weaponWeight = weaponData.levels[currentLevel].weight;
        attackRate = universalValues.WeightTypes[weaponWeight].AttackRate;
    }

    private void Update()
    {
        if (draggable)
        {
            Vector3 draggedPos = Input.mousePosition;
            draggedPos.z = -6;

            transform.position = draggedPos;

            if (Input.GetMouseButtonUp(0)) // check when mouse 1 is released
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
                {
                    if (hitInfo.collider == null) return; // if we didn't hit anything, exit the function

                    if (hitInfo.collider.GetComponentInParent<WeaponBehavior>() != null) // if we hit a weapon
                    {
                        WeaponBehavior equippedWeapon = hitInfo.collider.GetComponentInParent<WeaponBehavior>();
                        GameObject weaponParent = equippedWeapon.transform.parent.gameObject; // get parent of currently held weapon
                        CharacterBehavior selectedCharacter = weaponParent.GetComponentInParent<CharacterBehavior>(); // get character holding the weapon

                        if (weaponParent == null || selectedCharacter == null)
                        {
                            Debug.LogError("Weapon parent or character is null.");
                            return;
                        }

                        WeaponInvSlot otherSlot = equippedWeapon.weaponInvSlot;
                        SpawnerBehavior.instance.RemoveEquippedWeapon(equippedWeapon, weaponParent, selectedCharacter, otherSlot); // remove currently held weapon
                        SpawnerBehavior.instance.EquipNewWeapon(this, weaponParent, selectedCharacter); // equip new weapon
                    }
                    else if (hitInfo.collider.GetComponentInParent<CharacterBehavior>() != null) // if we hit a character
                    {
                        CharacterBehavior selectedCharacter = hitInfo.collider.GetComponentInParent<CharacterBehavior>();
                        GameObject selectedSlot = null; // define variable

                        if (selectedCharacter.characterData.max_weapon_count < 2) // if only one weapon can be held, drop into first slot
                        {
                            selectedSlot = selectedCharacter.weaponSlots[0];
                            if (selectedCharacter.heldWeapons.Count > 0)
                            {
                                WeaponInvSlot otherSlot = selectedCharacter.heldWeapons[0].weaponInvSlot;
                                SpawnerBehavior.instance.RemoveEquippedWeapon(selectedCharacter.heldWeapons[0], selectedCharacter.weaponSlots[0], selectedCharacter, otherSlot);
                            }
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
                                WeaponInvSlot otherSlot = selectedCharacter.heldWeapons[0].weaponInvSlot;
                                SpawnerBehavior.instance.RemoveEquippedWeapon(selectedCharacter.heldWeapons[0], selectedCharacter.weaponSlots[0], selectedCharacter, otherSlot);
                                selectedSlot = selectedCharacter.weaponSlots[0];
                            }
                        }
                        SpawnerBehavior.instance.EquipNewWeapon(this, selectedSlot, selectedCharacter);
                    }
                    else
                    {
                        Debug.Log("hit nothing, destroying weapon");
                        weaponInvSlot.weaponIsActive = false; // if we hit something else, mark the inventory slot as inactive
                        Destroy(this.gameObject); // if we hit something else, destroy the weapon
                    }
                }
            }

        }
    }
}
