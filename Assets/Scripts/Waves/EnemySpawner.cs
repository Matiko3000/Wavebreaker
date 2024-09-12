using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] float timeBetweenWaves = 1f;
    [SerializeField] bool isLooping = true;
    WaveConfig currentWave;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    public WaveConfig GetCurrentWave()
    {
        return currentWave;
    }

    IEnumerator SpawnWaves()
    {
        do
        {
            foreach (WaveConfig wave in waveConfigs) //go through every wave
            {
                currentWave = wave;
                for (int i = 0; i < currentWave.GetEnemyCount(); i++)//spawn the enemies for each wave
                {
                    Instantiate(currentWave.GetEnemyPrefab(i), currentWave.GetRandomSpawnPoint().position, Quaternion.Euler(0,0,0), transform);
                    yield return new WaitForSeconds(currentWave.GetRandomSpawnTime());
                }
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        } while (isLooping); //loop all the waves
    }
}
