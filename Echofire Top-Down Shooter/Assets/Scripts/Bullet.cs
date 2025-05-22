using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float impactForce;

    private Rigidbody rb;
    private BoxCollider cd;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float flyDistance = 100, float impactForce = 100)
    {
        this.impactForce = impactForce;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + 1;
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    private void DisableBulletIfNeeded()
    {
        if (!(Vector3.Distance(startPosition, transform.position) > flyDistance) || bulletDisabled) return;

        bulletDisabled = true;
        cd.enabled = false;
        meshRenderer.enabled = false;
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime; // magic number 2 is choosing for testing
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx(collision);
        ReturnBulletToPool();

        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        EnemyShield shield = collision.gameObject.GetComponent<EnemyShield>();

        if (shield)
        {
            shield.ReduceDurability();
            return;
        }

        if (!enemy) return;

        Vector3 force = rb.velocity.normalized * impactForce;
        Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

        enemy.GetHit();
        enemy.DeathImpact(force, collision.contacts[0].point, hitRigidbody);
    }

    protected void ReturnBulletToPool() => ObjectPool.instance.ReturnObject(gameObject);

    protected void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length <= 0) return;

        ContactPoint contact = collision.contacts[0];

        GameObject newImpactFx = ObjectPool.instance.GetObject((bulletImpactFX));
        newImpactFx.transform.position = contact.point;

        ObjectPool.instance.ReturnObject(newImpactFx, 1);
    }
}