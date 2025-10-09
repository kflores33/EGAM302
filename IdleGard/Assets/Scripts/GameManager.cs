using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// keep track of game state
public class GameManager : MonoBehaviour
{
    public UniversalValues universalValues;
    
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }

        wavesLeft = SaveManager.instance.WavesUntilWeapon;
        wavesUntilNewWeapon = SaveManager.instance.WavesBetweenWeapons;
    }

    [SerializeField] List<Vector3> EnemyPositionList = new List<Vector3>();
    public EnemyScriptable chosenEnemy; // temporary until enemy database is set up
    [SerializeField]List<EnemyBehavior> ActiveEnemies = new List<EnemyBehavior>();

    bool canSpawnNewWave;
    public int wavesUntilNewWeapon = 3;
    public int wavesLeft;

    private void Start()
    {
        int enemyCount = Random.Range(1, EnemyPositionList.Count);
        SpawnerBehavior.instance.RequestSpawnEnemy(chosenEnemy, EnemyPositionList, enemyCount);
        Debug.Log("spawning first Wave");
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

            --wavesLeft;
            if (wavesLeft == 0)
            {
                ShopInvManager.instance.AddRandomWeaponToShop();

                float interm = wavesUntilNewWeapon; 
                interm *= universalValues.waveReqScaler;
                wavesUntilNewWeapon = (int)Mathf.Round(interm);

                wavesLeft = wavesUntilNewWeapon;
            }

            canSpawnNewWave = false;
        }
    }

    private void OnApplicationQuit()
    {
        SaveManager.instance.UpdateWavesBtwn(wavesUntilNewWeapon);
        SaveManager.instance.UpdateWavesLeft(wavesLeft);
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
