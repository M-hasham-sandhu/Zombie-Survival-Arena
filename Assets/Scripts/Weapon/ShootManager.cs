using System.Collections;
using UnityEngine;

public class ShootManager : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float bulletTTL = 2f;

    public float bulletDamage = 50f; // Default bullet damage

    private Coroutine shootingCoroutine;

    public void StartShooting()
    {
        Debug.Log("ShootManager: StartShooting called");
        if (shootingCoroutine == null)
        {
            Debug.Log("ShootManager: Starting ShootingRoutine coroutine");
            shootingCoroutine = StartCoroutine(ShootingRoutine());
        }
        else
        {
            Debug.Log("ShootManager: ShootingRoutine already running");
        }
    }

    public void StopShooting()
    {
        Debug.Log("ShootManager: StopShooting called");
        if (shootingCoroutine != null)
        {
            Debug.Log("ShootManager: Stopping ShootingRoutine coroutine");
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
        else
        {
            Debug.Log("ShootManager: ShootingRoutine was not running");
        }
    }

    private IEnumerator ShootingRoutine()
    {
        Debug.Log("ShootManager: ShootingRoutine started");
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(0.5f); // Adjust fire rate as needed
        }
    }

    public void Shoot()
    {
        Debug.Log("ShootManager: Shoot called");
        if (firePoint == null)
        {
            Debug.LogWarning("ShootManager: firePoint is not assigned!");
            return;
        }
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("ShootManager: Camera.main is null!");
            return;
        }
        GameObject bullet = ObjectPooler.Instance.GetBullet();
        if (bullet == null)
        {
            Debug.LogWarning("ShootManager: ObjectPooler returned null bullet!");
            return;
        }
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        // Raycast from center of screen to get aim point
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f; // Far point
        }
        Vector3 shootDir = (targetPoint - firePoint.position).normalized;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDir * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("ShootManager: Bullet has no Rigidbody!");
        }
        BulletReturner returner = bullet.GetComponent<BulletReturner>();
        if (returner != null)
        {
            returner.Init(bulletTTL, bulletDamage);
        }
        else
        {
            Debug.LogWarning("ShootManager: Bullet has no BulletReturner!");
        }
    }
}
