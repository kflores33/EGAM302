using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class CharacterBehavior : MonoBehaviour
{
    public CharacterScriptable characterData; // reference to character data scriptable object

    public List<GameObject> weaponSlots = new List<GameObject>(); // references to weapon slots (empty game objects as children)
    public List<WeaponBehavior> heldWeapons = new List<WeaponBehavior>(); // reference to currently equipped weapon(s)
    public List<Vector3> weaponSlotPos = new List<Vector3>();

    public List<EnemyBehavior> targetList = new List<EnemyBehavior>();

    public enum CharacterStates
    {
        Inactive,
        Active
    }
    public CharacterStates CurrentState;

    private void Awake()
    {
        WeaponSlotSetup();
    }

    private void Update()
    {
        switch(CurrentState)
        {
            case CharacterStates.Active:
                UpdateActive();
                break;
            case CharacterStates.Inactive:
                UpdateInactive();
                break;
        }
    }

    private void UpdateActive()
    {
        
    }

    private void UpdateInactive()
    {

    }

    // get list of current enemies from game manager

    // function for damage calculation & coroutine for attack cooldown (based on weapon weight) 
    // add character specific modifiers (weapon proficiency)

    // send out damage to enemy on hit (via event system?) 

    //lock onto closest enemy

    private void UpdateTargetList(List<EnemyBehavior> targetList) // use this in the line before the attack in the attack coroutine
    {
        // check list of all enemies in scene (from game manager)
        // compare it to current target list
        // if there are any enemies not present in the target list, add them to it

        // main attack target will always be the first enemy in the list
    }

    private void OnSwitchToActive()
    {

        CurrentState = CharacterStates.Active;
    }
    private void OnSwitchToInactive()
    {

        CurrentState = CharacterStates.Inactive;
    }

    private void WeaponSlotSetup()
    {
        if (weaponSlotPos.Count < 1)
        {
            Debug.LogError("WeaponSlotSetup failed: No Vector3 positions assigned to 'weaponSlotPos' list in CharacterBehavior.cs. Please assign slot positions in the inspector.");
            return;
        }

        // create number of child weapon slots designated in data
        for (int i = 0; i < characterData.max_weapon_count; i++)
        {
            GameObject newChild = new GameObject("WeaponSlot");

            weaponSlots.Add(newChild);
            newChild.transform.SetParent(this.transform);

            newChild.transform.localPosition = weaponSlotPos[i];
        }
    }

    public void CheckIfHasWeapon()
    {
        if (heldWeapons.Count < 1)
        {
            if (CurrentState == CharacterStates.Active) OnSwitchToInactive();
            return;
        }

        if (CurrentState == CharacterStates.Inactive) OnSwitchToActive();
    }
}
