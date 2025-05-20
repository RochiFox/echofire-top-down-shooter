using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Transform player;
    private Vector3 direction;
    private float flySpeed;
    private float rotationSpeed;
    private float timer = 1;

    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        rotationSpeed = 1600;

        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));
        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;

        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bulletComponent = other.GetComponent<Bullet>();
        Player playerComponent = other.GetComponent<Player>();

        if (bulletComponent || playerComponent)
        {
            GameObject newFx = ObjectPool.instance.GetObject(impactFx);
            newFx.transform.position = transform.position;

            ObjectPool.instance.ReturnObject(gameObject);
            ObjectPool.instance.ReturnObject(newFx, 1f);
        }
    }
}