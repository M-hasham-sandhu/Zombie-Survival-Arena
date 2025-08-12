using System.Collections;
using UnityEngine;

public class ShootManager : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float bulletTTL = 2f;
    public float bulletDamage = 50f;

    [SerializeField] private CharacterMovement characterMovement;

    private Coroutine shootingCoroutine;
    private bool isAiming = false;

    private void Update()
    {
        bool shouldBeAiming = IsPlayerAiming();

        if (shouldBeAiming)
        {
            characterMovement?.PlayWalkAnimation();
        }
        else
        {
            characterMovement?.PlayIdleAnimation();
        }

        if (shouldBeAiming != isAiming)
        {
            isAiming = shouldBeAiming;
            if (isAiming)
            {
                StartShooting();
            }
            else
            {
                StopShooting();
            }
        }
    }

    private bool IsPlayerAiming()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.position.x >= Screen.width / 2f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void StartShooting()
    {
        if (shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(ShootingRoutine());
        }
    }

    public void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(0.5f); // Adjust fire rate as needed
        }
    }

    public void Shoot()
    {
        if (firePoint == null)
        {
            return;
        }
        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }
        GameObject bullet = ObjectPooler.Instance.GetBullet();
        if (bullet == null)
        {
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
        BulletReturner returner = bullet.GetComponent<BulletReturner>();
        if (returner != null)
        {
            returner.Init(bulletTTL, bulletDamage);
        }
    }
}
    