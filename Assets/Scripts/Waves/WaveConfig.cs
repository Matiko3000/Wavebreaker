using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WaveConfig", fileName = "New Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] Transform spawnPointsPrefab;
    [SerializeField] float timeBetweenSpawn = 1f;
    [SerializeField] float spawnTimeVariance = 0f;
    [SerializeField] float minimumSpawnTime = 0.2f;

    List<Transform> spawnPoints = new List<Transform>();

    private void InitializeSpawnPoints()
    {
        foreach (Transform child in spawnPointsPrefab)//generate list with all the spawnpoints
        {
            spawnPoints.Add(child);
            Debug.Log("gerbis");
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0 || spawnPoints == null) InitializeSpawnPoints();//use this to generate the list just once, not every call of the metod
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public int GetEnemyCount()
    {
        return enemyPrefabs.Count;
    }

    public GameObject GetEnemyPrefab(int index)
    {
        return enemyPrefabs[index];
    }

    public float GetRandomSpawnTime()
    {
        float spawnTime = Random.Range(timeBetweenSpawn - spawnTimeVariance, timeBetweenSpawn + spawnTimeVariance);
        return Mathf.Clamp(spawnTime, minimumSpawnTime, 2 * timeBetweenSpawn);
    }
}
