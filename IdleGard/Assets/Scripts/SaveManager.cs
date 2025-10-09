using System.IO;
using UnityEngine;

// NOTE: this script & related ones were AI assisted
// handles file management (nothing related to gameplay!!)
public class SaveManager : MonoBehaviour
{
    public OwnedWeaponDB ownedWeapons;
    public WeaponDatabase weaponDatabase;
    private string savePath;
    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "player_save.json");
        LoadSave();
    }

    public void LoadSave()
    {
        if (File.Exists(savePath)) // load save data and update OwnedWeaponDB
        {
            string json = File.ReadAllText(savePath);
            JsonUtility.FromJsonOverwrite(json, ownedWeapons);
        }
        else
        {
            InitializeNewSave();
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(ownedWeapons, true); // saves the list of owned weapons to a json format
        File.WriteAllText(savePath, json);
    }
    private void InitializeNewSave() // clear existing data and give the player a starter weapon
    {
        ownedWeapons.ownedWeapons.Clear();

        var starter = weaponDatabase.GetWeaponById("deathdance");
        if (starter != null) 
        { 
            ownedWeapons.AddWeapon(starter); 
        }
    }
}
