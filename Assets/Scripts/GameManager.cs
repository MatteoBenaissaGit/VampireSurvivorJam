
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private float _timer;
    [SerializeField] private List<Transform> _spawnPointsList;
    [SerializeField] private List<SpawnWave> _spawnWaves;
    [SerializeField] private List<SpawnInfo> _spawnList = new List<SpawnInfo>();


    private void Start()
    {
        CalculateWave();
    }

    private void Update()
    {
        TimerSpawning();
    }

    private void TimerSpawning()
    {
        _timer += Time.deltaTime;
        foreach (SpawnInfo spawn in _spawnList.ToArray())
        {
            if (_timer >= spawn.Time)
            {
                Instantiate(spawn.Enemy, GetRandomSpawnPoint(), Quaternion.identity);
                _spawnList.Remove(spawn);
            }
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        return _spawnPointsList[Random.Range(0, _spawnPointsList.Count - 1)].position;
    }

    private void CalculateWave()
    {
        _spawnList.Clear();
        
        float time = 0;
        foreach (SpawnWave wave in _spawnWaves)
        {
            //create a list with each one of the enemies that will be spawnes
            List<EnemyController> enemiesControllerToSpawnList = new List<EnemyController>();
            foreach (EnemyToSpawn enemyToSpawn in wave.EnemiesToSpawn)
            {
                for (int i = 0; i < enemyToSpawn.Number; i++)
                {
                    enemiesControllerToSpawnList.Add(enemyToSpawn.Enemy);
                }
            }

            enemiesControllerToSpawnList.Sort((a, b) => 1 - 2 * UnityEngine.Random.Range(0, 1));

            float timeFrame =
                wave.SecondsFromTo.y - wave.SecondsFromTo.x; //time frame during which there can be spawns 
            float timeSeparateEachSpawn =
                timeFrame / enemiesControllerToSpawnList.Count; //spawn each timeSeparateEachSpawn

            time = wave.SecondsFromTo.x;

            foreach (EnemyController enemy in enemiesControllerToSpawnList)
            {
                _spawnList.Add(new SpawnInfo() { Enemy = enemy, Time = time });
                time += timeSeparateEachSpawn;
            }
        }
    }
    
}

[Serializable]
public struct SpawnWave
{
    public Vector2 SecondsFromTo;
    public List<EnemyToSpawn> EnemiesToSpawn;
}

[Serializable]
public struct EnemyToSpawn
{
    public EnemyController Enemy;
    public int Number;
}

[Serializable]
public struct SpawnInfo
{
    public float Time;
    public EnemyController Enemy;
}
