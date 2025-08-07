using System.Collections;
using UnityEngine;

public class ShootManager : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float bulletTTL = 2f;

    [Header("Weapon Positioning")]
    [SerializeField] private Transform weaponTransform; // The gun's transform
    [SerializeField] private Vector3 idlePosition = new Vector3(0.3f, -0.2f, 0.5f);
    [SerializeField] private Vector3 idleRotation = new Vector3(0f, 0f, -45f);
    [SerializeField] private Vector3 aimingPosition = new Vector3(0f, 0f, 0.8f);
    [SerializeField] private Vector3 aimingRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private float transitionSpeed = 8f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public float bulletDamage = 50f; // Default bullet damage

    private Coroutine shootingCoroutine;
    private Coroutine positionCoroutine;
    private bool isAiming = false;

    private void Start()
    {
        // Set initial weapon position to idle
        SetWeaponToIdle();
    }

    private void Update()
    {
        // Check if player is aiming (touching right side of screen)
        bool shouldBeAiming = IsPlayerAiming();
        
        if (shouldBeAiming != isAiming)
        {
            isAiming = shouldBeAiming;
            if (isAiming)
            {
                SetWeaponToAiming();
            }
            else
            {
                SetWeaponToIdle();
            }
        }
    }

    private bool IsPlayerAiming()
    {
        // Check if any touch is on the right half of the screen
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

    public void SetWeaponToIdle()
    {
        if (weaponTransform == null) return;
        
        if (positionCoroutine != null)
        {
            StopCoroutine(positionCoroutine);
        }
        positionCoroutine = StartCoroutine(TransitionWeaponPosition(idlePosition, idleRotation));
    }

    public void SetWeaponToAiming()
    {
        if (weaponTransform == null) return;
        
        if (positionCoroutine != null)
        {
            StopCoroutine(positionCoroutine);
        }
        positionCoroutine = StartCoroutine(TransitionWeaponPosition(aimingPosition, aimingRotation));
    }

    private IEnumerator TransitionWeaponPosition(Vector3 targetPos, Vector3 targetRot)
    {
        Vector3 startPos = weaponTransform.localPosition;
        Vector3 startRot = weaponTransform.localEulerAngles;
        float elapsed = 0f;
        float duration = 1f / transitionSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = transitionCurve.Evaluate(t);
            
            weaponTransform.localPosition = Vector3.Lerp(startPos, targetPos, curveValue);
            weaponTransform.localEulerAngles = Vector3.Lerp(startRot, targetRot, curveValue);
            
            yield return null;
        }
        
        // Ensure we reach the exact target
        weaponTransform.localPosition = targetPos;
        weaponTransform.localEulerAngles = targetRot;
        
        positionCoroutine = null;
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

    // Gizmos for easy setup in editor
    private void OnDrawGizmosSelected()
    {
        if (weaponTransform == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(weaponTransform.position + weaponTransform.TransformDirection(idlePosition), 0.1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(weaponTransform.position + weaponTransform.TransformDirection(aimingPosition), 0.1f);
    }
}
