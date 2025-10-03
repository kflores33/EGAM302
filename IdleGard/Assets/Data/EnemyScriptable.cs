using UnityEngine;

[CreateAssetMenu( menuName = "Enemies/Enemy")]
public class EnemyScriptable : ScriptableObject
{
    public string enemy_name;
    public string enemy_id;
    public float enemy_HP = 30;
    public int atk_strength = 3;
    public string weight_class;
    public float blood_value;
    public Material image;
}
