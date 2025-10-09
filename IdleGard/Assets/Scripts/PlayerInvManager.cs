using UnityEngine;

public class PlayerInvManager : MonoBehaviour
{
    public static PlayerInvManager instance;

    private void Awake()
    {
        if  (instance == null) instance = this;
        else Destroy(this);
    }

    public OwnedWeaponDB ownedWeaponDB;
    public WeaponDatabase weaponDatabase;
    public UniversalValues universalValues;

    public void RegisterKill(string weapon_id)
    {
        var saveData = ownedWeaponDB.GetWeapon(weapon_id); // basically referencing itself....shhh
        if (saveData == null) return;

        saveData.killCount++;
        var nextLevel = saveData.currentLevel + 1;
        var reqKills = weaponDatabase.GetWeaponById(weapon_id).levels[saveData.currentLevel].kills_to_level;
            float killScaler = universalValues.killReqScaler;
            float toRound = reqKills * killScaler;
        reqKills = (int)Mathf.Round(toRound);

        if (saveData.killCount >= reqKills) // level up if required kill count is reached
        {
            saveData.currentLevel++;
            saveData.killCount = 0;
        }

        Debug.Log($"Killed enemy. Kill Count = {saveData.killCount}");
    }
}
