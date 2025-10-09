using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UniversalValues", menuName = "MiscellaneousData/UniversalValues")]
public class UniversalValues : ScriptableObject
{
    public Dictionary<string, Weight> WeightTypes = new Dictionary<string, Weight>
    {
        { "Light", new Weight { AttackRate = 0.5f, WeightClass = "Light" } },
        { "Moderate", new Weight { AttackRate = 1f, WeightClass = "Moderate" } },
        { "Heavy", new Weight { AttackRate = 1.8f, WeightClass = "Heavy" } },
        { "Very Heavy", new Weight { AttackRate = 4f, WeightClass = "Very Heavy" } }
    }; // there should be 4

    //[Range(0f, 100f)] // Add this attribute to show a slider in the editor
    //public float BloodPerKill;

    [Range(0.01f, 2)]
    public float killReqScaler = 1;

    [Range(0.5f, 20)]
    public float killReqToBlood = 1.5f;

    public float waveReqScaler = 1.2f;
}

[System.Serializable]
public class Weight
{
    [Tooltip("Number of seconds between attacks")]
    public float AttackRate;
    public string WeightClass;
}
