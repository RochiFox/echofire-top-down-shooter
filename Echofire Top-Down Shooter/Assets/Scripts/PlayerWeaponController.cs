using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
