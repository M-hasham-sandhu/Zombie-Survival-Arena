using UnityEngine;

public class BulletReturner : MonoBehaviour
{
    private float ttl = 2f;
    private float timer = 0f;
    private bool active = false;

    [SerializeField] private Rigidbody rb; 

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

   
    public void Init(float timeToLive)
    {
        ttl = timeToLive;
        timer = 0f;
        active = true;
    }

    private void OnEnable()
    {
        timer = 0f;
        active = true;
    }

    private void OnDisable()
    {
        active = false;
        if (rb != null) rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (!active) return;
        timer += Time.deltaTime;
        if (timer >= ttl)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Zombie"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        active = false;
        ObjectPooler.Instance.ReturnBullet(gameObject);
    }
}
