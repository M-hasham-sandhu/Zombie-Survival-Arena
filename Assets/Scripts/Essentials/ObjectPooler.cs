using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [Header("Zombie Pool Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int initialSize = 10;

    [Header("Bullet Pool Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialBulletSize = 20;

    [Header("PowerUp Pool Settings")]
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private int initialPowerUpSize = 5;

    private readonly Queue<GameObject> zombiePool = new Queue<GameObject>();
    private readonly Queue<GameObject> bulletPool = new Queue<GameObject>();
    private readonly Queue<GameObject> powerUpPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Zombie pool initialization
        if (zombiePrefab == null)
        {
            Debug.LogError("ObjectPooler: Zombie prefab not assigned!");
            enabled = false;
            return;
        }
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(zombiePrefab, transform);
            if (!obj.CompareTag("Zombie"))
            {
                Debug.LogWarning($"ObjectPooler: Instantiated zombie does not have 'Zombie' tag. Assigning now.");
                obj.tag = "Zombie";
            }
            var agent = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }
            obj.SetActive(false);
            zombiePool.Enqueue(obj);
        }

        // Bullet pool initialization
        if (bulletPrefab == null)
        {
            Debug.LogWarning("ObjectPooler: Bullet prefab not assigned!");
        }
        else
        {
            for (int i = 0; i < initialBulletSize; i++)
            {
                GameObject obj = Instantiate(bulletPrefab, transform);
                if (!obj.CompareTag("Bullet"))
                {
                    Debug.LogWarning($"ObjectPooler: Instantiated bullet does not have 'Bullet' tag. Assigning now.");
                    obj.tag = "Bullet";
                }
                obj.SetActive(false);
                bulletPool.Enqueue(obj);
            }
        }

        // PowerUp pool initialization
        if (powerUpPrefab == null)
        {
            Debug.LogWarning("ObjectPooler: PowerUp prefab not assigned!");
        }
        else
        {
            for (int i = 0; i < initialPowerUpSize; i++)
            {
                GameObject obj = Instantiate(powerUpPrefab, transform);
                obj.SetActive(false);
                powerUpPool.Enqueue(obj);
            }
        }
    }

    public GameObject GetZombie()
    {
        GameObject obj;
        if (zombiePool.Count > 0)
        {
            obj = zombiePool.Dequeue();
        }
        else
        {
            obj = Instantiate(zombiePrefab, transform);
            if (!obj.CompareTag("Zombie"))
            {
                Debug.LogWarning($"ObjectPooler: Instantiated zombie does not have 'Zombie' tag. Assigning now.");
                obj.tag = "Zombie";
            }
            var agent = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnZombie(GameObject obj)
    {
        obj.SetActive(false);
        zombiePool.Enqueue(obj);
    }

    public GameObject GetBullet()
    {
        GameObject obj;
        if (bulletPool.Count > 0)
        {
            obj = bulletPool.Dequeue();
        }
        else
        {
            obj = Instantiate(bulletPrefab, transform);
            if (!obj.CompareTag("Bullet"))
            {
                Debug.LogWarning($"ObjectPooler: Instantiated bullet does not have 'Bullet' tag. Assigning now.");
                obj.tag = "Bullet";
            }
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnBullet(GameObject obj)
    {
        obj.SetActive(false);
        bulletPool.Enqueue(obj);
    }

    public GameObject GetPowerUp()
    {
        GameObject obj;
        if (powerUpPool.Count > 0)
        {
            obj = powerUpPool.Dequeue();
        }
        else
        {
            obj = Instantiate(powerUpPrefab, transform);
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnPowerUp(GameObject obj)
    {
        obj.SetActive(false);
        powerUpPool.Enqueue(obj);
    }
}