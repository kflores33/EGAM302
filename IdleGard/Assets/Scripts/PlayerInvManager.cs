using UnityEngine;

public class PlayerInvManager : MonoBehaviour
{
    public static PlayerInvManager instance;

    private void Awake()
    {
        if  (instance == null) instance = this;
        else Destroy(this);

        ownedWeaponDB = SaveManager.instance.RuntimeWeapons; // make sure the correct thing is referenced
    }

    public OwnedWeaponDB ownedWeaponDB;
    public WeaponDatabase weaponDatabase;
    public UniversalValues universalValues;

    public void RegisterKill(string weapon_id)
    {
        var saveData = ownedWeaponDB.GetWeapon(weapon_id);
        if (saveData == null) { Debug.Log("No save data to reference"); return; }

        saveData.killCount++;
        var nextLevel = saveData.currentLevel + 1;
        var reqKills = saveData.weaponData.levels[saveData.currentLevel].kills_to_level;
            float killScaler = universalValues.killReqScaler;
            float toRound = reqKills * killScaler;
        reqKills = (int)Mathf.Round(toRound);

        if (saveData.killCount >= reqKills) // level up if required kill count is reached
        {
            saveData.currentLevel++;
            saveData.killCount = 0;
        }

        SaveManager.instance.Save();
        Debug.Log($"Killed enemy. Kill Count = {saveData.killCount}");
    }
}
