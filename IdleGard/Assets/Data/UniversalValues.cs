using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UniversalValues", menuName = "MiscellaneousData/UniversalValues")]
public class UniversalValues : ScriptableObject
{
    public Dictionary<string, Weight> WeightTypes = new Dictionary<string, Weight>
    {
        { "Light", new Weight { AttackRate = 0.6f, WeightClass = "Light" } },
        { "Moderate", new Weight { AttackRate = 0.9f, WeightClass = "Moderate" } },
        { "Heavy", new Weight { AttackRate = 1.4f, WeightClass = "Heavy" } },
        { "Very Heavy", new Weight { AttackRate = 4f, WeightClass = "Very Heavy" } }
    }; // there should be 4

    //[Range(0f, 100f)] // Add this attribute to show a slider in the editor
    //public float BloodPerKill;
}

[System.Serializable]
public class Weight
{
    [Tooltip("Number of seconds between attacks")]
    public float AttackRate;
    public string WeightClass;
}
