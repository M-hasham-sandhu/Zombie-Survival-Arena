using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    [Header("Wave Settings, Delays in seconds")]
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
    [Tooltip("Total number of waves before game ends. Set to -1 for infinite waves.")]
    public int totalWaves = 10;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Player Reference")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;


    private int currentWave = 0;
    private int activeZombiesCount = 0; // Track active zombies
    
    // Add new event for game completion
    public System.Action onGameComplete;
    public System.Action<int> onWaveStart; // Event for wave start
    public System.Action<int> onWaveComplete; // Event for wave completion

    private void Start()
    {
        if (ObjectPooler.Instance == null)
        {
            enabled = false;
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            enabled = false;
            return;
        }
        foreach (var sp in spawnPoints)
        {
            if (sp == null) continue;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(sp.position, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            {
                Vector3 aligned = sp.position;
                aligned.y = hit.position.y;
                sp.position = aligned;
            }
            else
            {
            }
        }
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
            {
            }
        }
        
        // Subscribe to zombie death/return events
        foreach (var zombie in FindObjectsOfType<ZombieMovement>())
        {
            zombie.onZombieReturn += OnZombieReturned;
        }
        
        StartCoroutine(WaveSpawnerCoroutine());
    }

    private void OnZombieReturned(GameObject zombie)
    {
        activeZombiesCount--;
        if (activeZombiesCount <= 0)
        {
            // All zombies in the wave are defeated
            Debug.Log($"Wave {currentWave} cleared!");
            onWaveComplete?.Invoke(currentWave);
        }
    }

    private System.Collections.IEnumerator WaveSpawnerCoroutine()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            currentWave++;
            
            // Check if we've reached the final wave
            if (totalWaves > 0 && currentWave > totalWaves)
            {
                Debug.Log("All waves complete! Game Over!");
                onGameComplete?.Invoke();
                yield break;
            }

            int zombiesThisWave = baseZombiesPerWave + zombiesPerWaveIncrement * (currentWave - 1);
            Debug.Log($"Starting Wave {currentWave}/{totalWaves} with {zombiesThisWave} zombies");
            onWaveStart?.Invoke(currentWave);
            
            yield return StartCoroutine(SpawnWave(zombiesThisWave));
            
            // Wait until all zombies are defeated
            while (activeZombiesCount > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            Debug.Log($"Wave {currentWave}/{totalWaves} complete. Next wave in {timeBetweenWaves} seconds.");
            
            if (PowerUpManager.Instance != null)
            {
                PowerUpManager.Instance.SpawnPowerUpsForWave();
            }
            
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private System.Collections.IEnumerator SpawnWave(int count)
    {
        activeZombiesCount = count; // Reset counter for new wave
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
            return;

        GameObject zombie = ObjectPooler.Instance.GetZombie();
        zombie.transform.position = spawnPoint.position;
        zombie.transform.rotation = spawnPoint.rotation;
        var agent = zombie.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
        }
        
        var zombieMovement = zombie.GetComponent<ZombieMovement>();
        if (zombieMovement != null && player != null)
        {
            zombieMovement.SetTarget(player);
            zombieMovement.onZombieReturn += OnZombieReturned; // Subscribe to return event
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
