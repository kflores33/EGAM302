using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;

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
        CheckIfHasWeapon();

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

    EnemyBehavior currentTarget;
    bool canAttack;
    private void UpdateActive()
    {
        if (canAttack)
        {
            foreach (var weapon in heldWeapons)
            { 
                StartCoroutine(AttackCoroutine(weapon));
            }
        }
        if (!CheckIfWaveActive()) SwitchStates();
    }

    public void DeregisterTarget(EnemyBehavior target)
    {
        if (currentTarget = target) currentTarget = null;
        if (targetList.Contains(target))targetList.Remove(target);
        if (targetList.Count == 0) { Debug.Log("Wave defeated"); StopAllCoroutines(); SwitchStates(); }
    }

    void UpdateTargetSelection()
    {
        if (currentTarget == null)
        {
            if ( targetList.Count == 0) GameManager.instance.PopulateEnemyList(targetList); // make sure target list is populated
            
            for(int i = 0; i < targetList.Count; i++)
            {
                EnemyBehavior target = targetList[i];
                //Debug.Log($"{target} is no. {i} in {targetList}");
            }
            if (targetList.Count == 0) { Debug.LogError("No targets in targetList!"); }

            currentTarget = targetList[0];
            //Debug.Log($"{currentTarget} selected. Preparing to attack");
        }
    }
    IEnumerator AttackCoroutine(WeaponBehavior chosenWeapon)
    {
        canAttack = false;

        UpdateTargetSelection();

        yield return new WaitForSeconds(chosenWeapon.attackRate);

        if(currentTarget == null) { currentTarget = targetList[0]; }
        currentTarget.TakeDamage(chosenWeapon.weaponData.levels[chosenWeapon.currentLevel].attack_strength, this);

        canAttack = true;
    }

    private void UpdateInactive()
    {
        if (CheckIfWaveActive())
        {
            SwitchStates();
        }
    }

    private void OnSwitchToActive()
    {
        CurrentState = CharacterStates.Active;
        canAttack = true;
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

    public bool CheckIfHasWeapon()
    {
        if (heldWeapons.Count > 0)
        {
            return true;
        }
        return false;
    }
    public bool CheckIfWaveActive()
    {
        if (GameManager.instance.GetActiveEnemies().Count > 0)
        {
            return true ;
        }
        return false;
    }

    public void SwitchStates()
    {
        if (CheckIfWaveActive() && CheckIfHasWeapon())
        { if (CurrentState == CharacterStates.Inactive) OnSwitchToActive(); }
        else
        { if (CurrentState == CharacterStates.Active) OnSwitchToInactive();}
    }
}
