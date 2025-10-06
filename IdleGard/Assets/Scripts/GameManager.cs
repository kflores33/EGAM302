using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// keep track of game state
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    [SerializeField] List<Vector3> EnemyPositionList = new List<Vector3>();
    public EnemyScriptable chosenEnemy; // temporary until enemy database is set up
    List<EnemyBehavior> ActiveEnemies;

    // tell spawner when to spawn
    // house references & inter-script functions
    // manage game state

    private void Update()
    {
        if (ActiveEnemies == null || ActiveEnemies.Count < 1)
        {
            //spawn enemies
            int enemyCount = Random.Range(1, EnemyPositionList.Count);
            //EnemyScriptable chosenEnemy = FindAnyObjectByType<EnemyDatabase>().enemyList[Random.Range(0, FindAnyObjectByType<EnemyDatabase>().enemyList.Count)];

            SpawnerBehavior.instance.RequestSpawnEnemy(chosenEnemy, EnemyPositionList, enemyCount);
        }
    }

    public void RegisterActiveEnemy(EnemyBehavior enemy)
    {
        if (!ActiveEnemies.Contains(enemy))
        {
            ActiveEnemies.Add(enemy);
        }
    }
    public void DeregisterActiveEnemy(EnemyBehavior enemy)
    {
        if (ActiveEnemies.Contains(enemy))
        {
            ActiveEnemies.Remove(enemy);
        }
    }
    public List<EnemyBehavior> ProvideEnemyList()
    {
        return ActiveEnemies;
    }
}
