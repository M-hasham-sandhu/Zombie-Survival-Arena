using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [Tooltip("Delay before the first wave starts.")]
    public float initialDelay = 2f;
    [Tooltip("Time between waves.")]
    public float timeBetweenWaves = 5f;
    [Tooltip("Time between each zombie spawn in a wave.")]
    public float timeBetweenSpawns = 1f;
    [Tooltip("Base number of zombies in the first wave.")]
    public int baseZombiesPerWave = 3;
    [Tooltip("How many more zombies to add each wave.")]
    public int zombiesPerWaveIncrement = 2;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Player Reference")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;


    private int currentWave = 0;

    private void Start()
    {
        if (ObjectPooler.Instance == null)
        {
            Debug.LogError("ZombieSpawnManager: ObjectPooler singleton instance not found!");
            enabled = false;
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned to ZombieSpawnManager!");
            enabled = false;
            return;
        }
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("ZombieSpawnManager: Player reference not set and no object with tag 'Player' found.");
        }
        StartCoroutine(WaveSpawnerCoroutine());
    }

    private System.Collections.IEnumerator WaveSpawnerCoroutine()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            currentWave++;
            int zombiesThisWave = baseZombiesPerWave + zombiesPerWaveIncrement * (currentWave - 1);
            Debug.Log($"Spawning wave {currentWave} with {zombiesThisWave} zombies.");
            yield return StartCoroutine(SpawnWave(zombiesThisWave));
            Debug.Log($"Wave {currentWave} complete. Next wave in {timeBetweenWaves} seconds.");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private System.Collections.IEnumerator SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void SpawnZombie()
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No valid spawn point found for zombie!");
            return;
        }
        GameObject zombie = ObjectPooler.Instance.GetZombie();
        zombie.transform.position = spawnPoint.position;
        zombie.transform.rotation = spawnPoint.rotation;
        // Assign player as target if possible
        var zombieMovement = zombie.GetComponent<ZombieMovement>();
        if (zombieMovement != null && player != null)
        {
            zombieMovement.SetTarget(player);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            return null;
        int idx = Random.Range(0, spawnPoints.Count);
        return spawnPoints[idx];
    }
}
