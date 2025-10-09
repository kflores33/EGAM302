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
        Debug.Log($"Save path: {savePath}");
        LoadSave();
    }

    public void LoadSave()
    {
        if (File.Exists(savePath)) // load save data and update OwnedWeaponDB
        {
            string json = File.ReadAllText(savePath);
            if (json.Length < 3) // if the file is empty or nearly so, initialize a new save
            {
                InitializeNewSave();
                Save();
                return;
            }
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

        Debug.Log("Game saved");
    }
    private void InitializeNewSave() // clear existing data and give the player a starter weapon
    {
        Debug.Log("Initializing new save file");
        ownedWeapons.ownedWeapons.Clear();

        var starter = weaponDatabase.GetWeaponById("deathdance");
        if (starter != null) 
        { 
            ownedWeapons.AddWeapon(starter); 
        }
        else { Debug.Log("No starter weapon found in database"); }
    }
}
