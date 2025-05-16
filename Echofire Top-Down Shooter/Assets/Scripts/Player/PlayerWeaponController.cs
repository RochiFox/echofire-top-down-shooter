using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private const float ReferenceBulletSpeed = 20;

    [SerializeField] private WeaponData defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet details")] [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        AssignInputEvents();

        Invoke(nameof(EquipStartingWeapon), 0.1f);
    }

    private void Update()
    {
        if (isShooting)
            Shoot();
    }

    private void EquipStartingWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);
        EquipWeapon(0);
    }

    #region Slots managment - Pickup\Equip\Drop\Ready Weapon

    private void EquipWeapon(int index)
    {
        if (index >= weaponSlots.Count)
            return;

        SetWeaponReady(false);

        currentWeapon = weaponSlots[index];
        player.WeaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.instance.ChangeCameraDistance(currentWeapon.CameraDistance);
    }

    public void PickupWeapon(WeaponData newWeaponData)
    {
        Weapon newWeapon = new Weapon(newWeaponData);

        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.WeaponVisuals.SwitchOffWeaponModels();

            weaponSlots[weaponIndex] = newWeapon;
            EquipWeapon(weaponIndex);

            return;
        }

        weaponSlots.Add(newWeapon);
        player.WeaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
            return;

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;
    public bool WeaponReady() => weaponReady;

    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.BulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.BurstFireDelay);

            if (i >= currentWeapon.BulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        if (WeaponReady() == false)
            return;

        if (currentWeapon.CanShoot() == false)
            return;

        player.WeaponVisuals.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);

        newBullet.transform.SetPositionAndRotation(GunPoint().position, Quaternion.LookRotation(GunPoint().forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        Bullet bulletScript = newBullet.GetComponent<Bullet>();

        bulletScript.BulletSetup(currentWeapon.GunDistance);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = ReferenceBulletSpeed / bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.WeaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.Aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.Aim.CanAimPrecisely() == false && !player.Aim.Target())
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        return weaponSlots.FirstOrDefault(weapon => weapon.weaponType == weaponType);
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => player.WeaponVisuals.CurrentWeaponModel().gunPoint;

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = player.Controls;

        controls.Character.Fire.performed += _ => isShooting = true;
        controls.Character.Fire.canceled += _ => isShooting = false;

        controls.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += _ => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += _ => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += _ => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += _ => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += _ => DropWeapon();

        controls.Character.Reload.performed += _ =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToggleWeaponMode.performed += _ => currentWeapon.ToggleBurst();
    }

    #endregion
}