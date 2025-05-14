using UnityEngine;

public class Bullet : MonoBehaviour
{
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
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float flyDistanceParam)
    {
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistanceParam + 1;
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
            ObjectPool.instance.ReturnBullet(gameObject);
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
            trailRenderer.time -= 2 * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx(collision);
        ObjectPool.instance.ReturnBullet(gameObject);
    }

    private void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length <= 0) return;

        ContactPoint contact = collision.contacts[0];

        GameObject newImpactFx =
            Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

        Destroy(newImpactFx, 1f);
    }
}