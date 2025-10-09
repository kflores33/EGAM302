using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // used to import directly into a dictionary

// NOTE: this script & related ones were AI assisted

public class WeaponDataImport : MonoBehaviour
{
    // const is used so that the values cannot be changed after compiling (i suppose?---basically, these paths will never be changed)
    private const string JSON_PATH = "Assets/Data/Resources/WeaponList.json";
    private const string OUTPUT_FOLDER = "Assets/Data/Weapons";
    private const string DATABASE_PATH = OUTPUT_FOLDER + "/WeaponDatabase.asset";

    [MenuItem("Tools/Import Weapons From JSON")] // Access this function from UnityEditor's menus
    public static void ImportWeapons() // 
    {
        if (!File.Exists(JSON_PATH))
        {
            Debug.LogError("JSON file not found at " +  JSON_PATH);
            return;
        }
        
        string json = File.ReadAllText(JSON_PATH);

        // JsonConver.DeserializeObject function from Newtonsoft.Json namespace used to directly parse data into a dictionary
        var parsed = JsonConvert.DeserializeObject<Dictionary<string, WeaponJsonData>>(json);

        if (!AssetDatabase.IsValidFolder(OUTPUT_FOLDER))
            AssetDatabase.CreateFolder("Data", "Weapons"); // makes a "Weapons" folder under the "Data" folder if it doesn't already exist

        List<WeaponScriptable> createdWeapons = new List<WeaponScriptable>(); // make a list to keep track of created weapons

        foreach (var weaponEntry in parsed) // populate/create scriptable objects with information from JSON
        {
            string weapon_id = weaponEntry.Key; 
            WeaponJsonData jsonData = weaponEntry.Value;

            string assetPath = $"{OUTPUT_FOLDER}/{weapon_id}.asset"; //define asset path
            WeaponScriptable asset = AssetDatabase.LoadAssetAtPath<WeaponScriptable>(assetPath); // try loading existing scriptable object

            // create new asset if missing
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<WeaponScriptable>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            // update values
            asset.wpnname = jsonData.wpnname;
            asset.weapon_id = weapon_id;
            asset.ability_name = jsonData.ability_name;
            asset.type = jsonData.type;
            asset.game = jsonData.game;
            asset.levels = jsonData.levels;

            EditorUtility.SetDirty(asset); // notify unity editor that asset has changed and needs to be saved
            createdWeapons.Add(asset);
        }

        // build/update database
        WeaponDatabase db = AssetDatabase.LoadAssetAtPath<WeaponDatabase>(DATABASE_PATH); // try loading existing scriptable object
        
        // create new asset if missing
        if (db == null) 
        {
            db = ScriptableObject.CreateInstance<WeaponDatabase>();
            AssetDatabase.CreateAsset (db, DATABASE_PATH);
        }
        db.allWeapons = createdWeapons; // copy the createdWeapons list over to the new db asset

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Imported {createdWeapons.Count} weapons from JSON!");
    }
}

[System.Serializable]
public class WeaponLevel
{
    public int level; // not to be confused with its number in the index---this just sorts the list numerically

    public string story;
    public int attack_strength; // for this game's purposes, damage done per hit
    public string weight; // generalized to one of 4 weight classes

    // ability variables
    public float MP_cost; // amount of "mana" required to cast
    public string ability_desc; // to be displayed to player
    public int ability_count; // if ability has a projectile, the number of projectiles fired
    public float ability_sec; // length of ability duration, if applicable
    public float ability_atk_multi = 1; // this should be 1 unless stated otherwise
    public float ability_damage_multi; // as in, damage taken by player
    public float ability_speed_multi; // speed of player, but in this game it can be considered attack speed
    public int kills_to_level; // amount of kills required to level up to next level 
}
[System.Serializable]
public class WeaponJsonData
{
    public string wpnname; // to be displayed to player
    public List<string> type; // can be of multiple types
    public List<string> game; // can be from multiple games
    public string weapon_id;
    public string ability_name;
    public List<WeaponLevel> levels; // index 0 = level 1, index 1 = level 2, etc.
}