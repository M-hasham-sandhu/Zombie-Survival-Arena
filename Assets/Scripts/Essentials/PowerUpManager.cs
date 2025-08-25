using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("PowerUp Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int powerUpsPerWave = 1;
    [SerializeField] private GameObject[] powerUpPrefabs; // Assign all three types in Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnPowerUpsForWave()
    {
        for (int i = 0; i < powerUpsPerWave; i++)
        {
            SpawnRandomPowerUp();
        }
    }

    private void SpawnRandomPowerUp()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            return;
        }
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int powerUpIndex = Random.Range(0, powerUpPrefabs.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject prefab = powerUpPrefabs[powerUpIndex];
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
} 