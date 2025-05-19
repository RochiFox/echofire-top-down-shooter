using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;

    private Rigidbody rb;
    private BoxCollider cd;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float flyDistanceParam, float newImpactForce)
    {
        impactForce = newImpactForce;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        flyDistance = flyDistanceParam + 1;
    }

    private void Update()
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

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        if (enemy)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

            enemy.GetHit();
            enemy.HitImpact(force, collision.contacts[0].point, hitRigidbody);
        }

        CreateImpactFx(collision);
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool() => ObjectPool.instance.ReturnObject(gameObject);

    private void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length <= 0) return;

        ContactPoint contact = collision.contacts[0];

        GameObject newImpactFx = ObjectPool.instance.GetObject((bulletImpactFX));
        newImpactFx.transform.position = contact.point;

        ObjectPool.instance.ReturnObject(newImpactFx, 1);
    }
}