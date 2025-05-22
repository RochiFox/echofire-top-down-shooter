using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx(collision);
        ReturnBulletToPool();

        Player player = collision.gameObject.GetComponentInParent<Player>();

        if (player)
            Debug.Log("Shot the player");
    }
}