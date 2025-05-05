using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        newBullet.GetComponent<Rigidbody>().velocity = gunPoint.forward * bulletSpeed;

        Destroy(newBullet, 5f);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
