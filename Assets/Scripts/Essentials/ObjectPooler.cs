using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [Header("Zombie Pool Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> zombiePool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (zombiePrefab == null)
        {
            Debug.LogError("ObjectPooler: Zombie prefab not assigned!");
            enabled = false;
            return;
        }
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(zombiePrefab);
            obj.SetActive(false);
            zombiePool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Get a zombie from the pool. If none available, instantiate a new one.
    /// </summary>
    public GameObject GetZombie()
    {
        GameObject obj;
        if (zombiePool.Count > 0)
        {
            obj = zombiePool.Dequeue();
        }
        else
        {
            obj = Instantiate(zombiePrefab);
        }
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Return a zombie to the pool (should be called when the zombie is no longer needed).
    /// </summary>
    public void ReturnZombie(GameObject obj)
    {
        obj.SetActive(false);
        zombiePool.Enqueue(obj);
    }
}
