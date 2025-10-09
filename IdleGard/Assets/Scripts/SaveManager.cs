using System.IO;
using UnityEngine;
using System.Collections.Generic;

// NOTE: this script & related ones were AI assisted
// handles file management (nothing related to gameplay!!)

[System.Serializable]
public class GeneralPlayerSaveData
{
    public List<OwnedWeapon> ownedWeapons = new List<OwnedWeapon>();
    public List<ShopWeapon> shopWeapons = new List<ShopWeapon>();
    public float bloodAccumulated;
    public int waveCount;
    public int wavesBetweenWeapons;
    public int wavesUntilWeapon;

    public GeneralPlayerSaveData() { }

    public GeneralPlayerSaveData(List<OwnedWeapon> ownedWeapons, List<ShopWeapon> shopWeapons,float bloodAccumulated, int waveCount, int wavesBetweenWeapons, int wavesUntilWeapon)
    { // make sure to use "this." to eliminate ambiguity!
        this.ownedWeapons = ownedWeapons;
        this.shopWeapons = shopWeapons;
        this.bloodAccumulated = bloodAccumulated;
        this.waveCount = waveCount;
        this.wavesBetweenWeapons = wavesBetweenWeapons;
        this.wavesUntilWeapon = wavesUntilWeapon;
    }
}
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public OwnedWeaponDB ownedWeapons; // static asset reference
    private OwnedWeaponDB runtimeWeapons; // in-memory copy (so information can be saved properly)
    public OwnedWeaponDB RuntimeWeapons => runtimeWeapons;

    public UniversalValues UniversalValues;

    #region Game State Stuff (pretend its not there)
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

    private int wavesBetweenWeapons;
    public void UpdateWavesBtwn(int waves)
    {
        wavesBetweenWeapons = waves;
        Save();
    }
    public int WavesBetweenWeapons => wavesBetweenWeapons;

    private int wavesUntilWeapon;
    public void UpdateWavesLeft(int waves)
    {
        wavesUntilWeapon = waves;
        Save();
    }
    public int WavesUntilWeapon => wavesUntilWeapon;
    #endregion

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

        runtimeWeapons.UniversalValues = UniversalValues;

        LoadSave();
    }

    public void Save()
    {
        GeneralPlayerSaveData data = new GeneralPlayerSaveData(
            runtimeWeapons.ownedWeapons, runtimeWeapons.shopWeapons,
            bloodAccumulated, waveCount, wavesBetweenWeapons, wavesUntilWeapon);

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
            runtimeWeapons.shopWeapons = data.shopWeapons;

            bloodAccumulated = data.bloodAccumulated;
            waveCount = data.waveCount;
            wavesUntilWeapon = data.wavesUntilWeapon;
            wavesBetweenWeapons = data.wavesBetweenWeapons;

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
        foreach (var s in runtimeWeapons.shopWeapons)
        {
            s.weaponData = weaponDatabase.GetWeaponById(s.weapon_id);
        }
    }

    private void InitializeNewSave() // clear existing data and give the player a starter weapon
    {
        Debug.Log("Initializing new save file");
        runtimeWeapons.ownedWeapons.Clear();
        runtimeWeapons.shopWeapons.Clear();

        bloodAccumulated = 0;
        waveCount = 0;
        wavesBetweenWeapons = 5;
        wavesUntilWeapon = wavesBetweenWeapons;

        var starter = weaponDatabase.GetWeaponById("deathdance");
        if (starter != null) 
        { 
            runtimeWeapons.AddWeapon(starter); 
        }
        var starterShop = weaponDatabase.GetWeaponById("moonfire");
        if (starterShop != null)
        {
            runtimeWeapons.AddShopWeapon(starterShop);
        }

        else { Debug.Log("No starter weapon found in database"); }
    }

    public void ClearData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Deleted save file.");
        }
        InitializeNewSave();
        Save();

        // restart scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
