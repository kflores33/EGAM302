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
    [SerializeField]List<EnemyBehavior> ActiveEnemies = new List<EnemyBehavior>();

    // tell spawner when to spawn
    // house references & inter-script functions
    // manage game state

    bool canSpawnNewWave;
    private void Start()
    {
        canSpawnNewWave = true;
    }

    private void Update()
    {
        if (canSpawnNewWave)
        {
            //spawn enemies
            int enemyCount = Random.Range(1, EnemyPositionList.Count);
            SpawnerBehavior.instance.RequestSpawnEnemy(chosenEnemy, EnemyPositionList, enemyCount);
            Debug.Log("spawning new Wave");

            SaveManager.instance.IncreaseWave();
            canSpawnNewWave = false;
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

            if( ActiveEnemies.Count < 1) { canSpawnNewWave = true; }
        }
    }
    public void PopulateEnemyList(List<EnemyBehavior> targetList)
    {
        targetList.AddRange(ActiveEnemies);
    }
    public List<EnemyBehavior> GetActiveEnemies()
    {
        return ActiveEnemies;
    }
}
