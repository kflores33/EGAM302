using System.IO;
using UnityEngine;
using System.Collections.Generic;

// NOTE: this script & related ones were AI assisted
// handles file management (nothing related to gameplay!!)

[System.Serializable]
public class GeneralPlayerSaveData
{
    public List<OwnedWeapon> ownedWeapons = new List<OwnedWeapon>();
    public float bloodAccumulated;
    public int waveCount;

    public GeneralPlayerSaveData() { }

    public GeneralPlayerSaveData(List<OwnedWeapon> ownedWeapons, float bloodAccumulated, int waveCount)
    { // make sure to use "this." to eliminate ambiguity!
        this.ownedWeapons = ownedWeapons;
        this.bloodAccumulated = bloodAccumulated;
        this.waveCount = waveCount;
    }
}
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public OwnedWeaponDB ownedWeapons; // static asset reference
    private OwnedWeaponDB runtimeWeapons; // in-memory copy (so information can be saved properly)
    public OwnedWeaponDB RuntimeWeapons => runtimeWeapons;

    private float bloodAccumulated;
    public void GainBlood(float blood)
    {
        bloodAccumulated += blood;
        Save();
        GeneralUIHandler.instance.UpdateBloodCounter(bloodAccumulated);
    }
    public float BloodAccumulated => bloodAccumulated;

    private int waveCount;
    public void IncreaseWave()
    {
        waveCount++;
        Save();
        GeneralUIHandler.instance.UpdateWaveCounter(waveCount);
    }
    public int WaveCount => waveCount;

    public WeaponDatabase weaponDatabase;
    private string savePath;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        savePath = Path.Combine(Application.persistentDataPath, "player_save.json");
        Debug.Log($"Save path: {savePath}, Exists: {File.Exists(savePath)}");

        // since scriptable object data is read only, you would need to clone the object to have a copy in memory

        runtimeWeapons = ScriptableObject.CreateInstance<OwnedWeaponDB>(); // creates an instance during runtime, which is stored in memory (can be saved and serialized to json)
        //JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(ownedWeapons), runtimeWeapons);

        LoadSave();
    }

    public void Save()
    {
        //SaveDataWrapper wrapper = new SaveDataWrapper(runtimeWeapons.ownedWeapons);
        //string json = JsonUtility.ToJson(wrapper, true); // saves the list of owned weapons to a json format
        GeneralPlayerSaveData data = new GeneralPlayerSaveData(
            runtimeWeapons.ownedWeapons,
            bloodAccumulated, waveCount);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Saved JSON:\n{json}");
    }

    public void LoadSave()
    {
        if (File.Exists(savePath)) // load save data and update OwnedWeaponDB
        {
            string json = File.ReadAllText(savePath);
            if (json.Length < 5) // if the file is empty or nearly so, initialize a new save
            {
                InitializeNewSave();
                Save();
                return;
            }
            
            GeneralPlayerSaveData data = JsonUtility.FromJson<GeneralPlayerSaveData>(json); 
            runtimeWeapons.ownedWeapons = data.ownedWeapons; // overwrite data with info from json (save) file

            bloodAccumulated = data.bloodAccumulated;
            waveCount = data.waveCount;

            //Debug.Log($"Loaded {runtimeWeapons.ownedWeapons.Count} weapons from save.");
        }
        else
        {
            InitializeNewSave();
            Save();
        }

        foreach (var w in runtimeWeapons.ownedWeapons) // populate references to weapon data in a non serialized field (not written to file)
        {
            w.weaponData = weaponDatabase.GetWeaponById(w.weapon_id);
        }
    }

    private void InitializeNewSave() // clear existing data and give the player a starter weapon
    {
        Debug.Log("Initializing new save file");
        runtimeWeapons.ownedWeapons.Clear();

        var starter = weaponDatabase.GetWeaponById("deathdance");
        if (starter != null) 
        { 
            runtimeWeapons.AddWeapon(starter); 
        }
        else { Debug.Log("No starter weapon found in database"); }
    }
}
//[System.Serializable]
//public class SaveDataWrapper // basically chucks the weapon list into an actual serializable list (inside a class)
//{
//    public List<OwnedWeapon> ownedWeapons;

//    public SaveDataWrapper(List<OwnedWeapon> weapons)
//    {
//        ownedWeapons = weapons;
//    }
//}
