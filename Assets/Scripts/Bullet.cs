using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 50f;
    public float lifeTime = 5f;
    public float damage;

    [Header("Collision")]
    public LayerMask hitMask; 
    private Vector3 prevPos;

    private Rigidbody rb;

    public override void OnStartServer()
    {
        base.OnStartServer();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = true;

        prevPos = transform.position;

        Invoke(nameof(DestroyBullet), lifeTime);
    }

    [ServerCallback]
    void FixedUpdate()
    {
        Vector3 move = rb.position - prevPos;
        if (rb.SweepTest(rb.linearVelocity.normalized, out RaycastHit hit, rb.linearVelocity.magnitude * Time.fixedDeltaTime))
        {
            DestroyBullet();
            HandleHit(hit);
        }
        prevPos = rb.position;
    }

    [Server]
    void HandleHit(RaycastHit hit)
    {
        IDamageable targetHealth = hit.collider.GetComponent<IDamageable>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage); 
        }

        // Optional: spawn hit effects
        // Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
    }

    [Server]
    void DestroyBullet()
    {
        NetworkServer.Destroy(gameObject);
    }
}