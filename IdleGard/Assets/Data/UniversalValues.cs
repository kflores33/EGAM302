using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UniversalValues", menuName = "MiscellaneousData/UniversalValues")]
public class UniversalValues : ScriptableObject
{
    public List<Weight> WeightTypes = new List<Weight>(); // there should be 4

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
