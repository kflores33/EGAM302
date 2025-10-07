using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

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
    public UniversalValues universalValues;

    public GameObject weaponPrefab;
    public GameObject characterPrefab;
    public GameObject enemyPrefab;

    private void Update()
    {
        if (shouldSpawnEnemy)
        {
            SpawnEnemy(enemyDataRef, spawnLocationListRef, spawnCountRef);
            shouldSpawnEnemy = false;
        }
    }

    #region Weapon Spawning and Equipping
    public void SpawnWeapon(Vector3 position, WeaponScriptable selectedWeapon, WeaponInvSlot weaponInvSlot)
    {
        GameObject weapon = Instantiate(weaponPrefab, position, Quaternion.identity);
        
        weapon.GetComponent<WeaponBehavior>().weaponData = selectedWeapon; // assign the selected weapon data to the weapon behavior script
        weapon.GetComponent<WeaponBehavior>().universalValues = universalValues;
        weapon.GetComponent<WeaponBehavior>().draggable = true;
        weapon.GetComponent<WeaponBehavior>().weaponInvSlot = weaponInvSlot; // assign the inventory slot this weapon came from
    }
    public void RemoveEquippedWeapon(WeaponBehavior equippedWeapon, GameObject weaponParent, CharacterBehavior selectedCharacter, WeaponInvSlot weaponInvSlot)
    {
        // before doing anything, be sure to communicate with save manager to update saved data

        equippedWeapon.transform.SetParent(null); // unparent currently held weapon
        weaponInvSlot.weaponIsActive = false;
        selectedCharacter.heldWeapons.Remove(equippedWeapon); // remove old weapon from character's held weapons list

        Debug.Log($"removing {equippedWeapon} from {selectedCharacter}");
        Destroy(equippedWeapon.gameObject); // destroy currently held weapon
    }
    public void EquipNewWeapon(WeaponBehavior newWeapon, GameObject weaponParent, CharacterBehavior selectedCharacter)
    {
        newWeapon.transform.SetParent(weaponParent.transform); // set new weapon as child of character
        selectedCharacter.heldWeapons.Add(newWeapon); // add new weapon to character's held weapons list
        newWeapon.transform.localPosition = Vector3.zero; // reset position to be centered in slot
        newWeapon.draggable = false;

        Debug.Log($"equipping {newWeapon} to {selectedCharacter}");
    }
    #endregion

    #region Enemy Spawning
    bool shouldSpawnEnemy;

    EnemyScriptable enemyDataRef;
    Vector3 spawnLocationRef;
    int spawnCountRef;
    List<Vector3> spawnLocationListRef;

    public void RequestSpawnEnemy(EnemyScriptable enemyData, List<Vector3> spawnPoints, int count)
    {
        enemyDataRef = enemyData;
        spawnCountRef = count;
        spawnLocationListRef = spawnPoints;

        shouldSpawnEnemy = true;
    }
    void SpawnEnemy(EnemyScriptable enemyData, List<Vector3> spawnPoints, int count)
    {
        List<Vector3> validSpawnPoints = new List<Vector3>(spawnPoints);

        for (int i = 0; i < count; i++)
        {
            if (validSpawnPoints.Count < 1)
            {
                Debug.LogWarning("Not enough valid spawn points to spawn all requested enemies.");
                return;
            }

            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            Vector3 spawnPos = validSpawnPoints[randomIndex];
            validSpawnPoints.RemoveAt(randomIndex); // remove this spawn point from the list to avoid overlapping spawns

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<EnemyBehavior>().enemyData = enemyData; // assign the selected enemy data to the enemy behavior script

            GameManager.instance.RegisterActiveEnemy(enemy.GetComponent<EnemyBehavior>());
        }
    }

    #endregion
}
